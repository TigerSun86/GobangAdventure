# Reward Generation

## Purpose

This document defines the reward generation and reward application design for the Unity 6.4 2D turn-based card roguelike demo.

It explains:

* the structure of one reward offer
* how upgrade and replace options are generated
* how fallback from missing upgrades works
* how reward options are deduplicated
* how canonical deck signatures are used
* how selected rewards are applied to the player deck
* how authored reward-generation config participates in offer structure and replacement generation
* how locked reward invariants differ from default reward-scale values

This document focuses on reward logic. Reward timing and run progression are described in `run-battle-reward-flow.md`.

---

## Reward System Goals

The reward system is designed to satisfy the following goals:

1. every reward selection should be small and readable
2. every option should be executable in one step
3. generated options should always be legal
4. duplicate options should not appear
5. the system should preserve meaningful variety
6. the current demo should remain simple to implement and test
7. future expansion and gameplay testing should remain possible without redesigning the whole structure

---

## Locked Reward Invariants vs Default Reward Scale

The current gameplay design distinguishes between:

1. **locked reward invariants**
2. **default reward-scale values**

### Locked reward invariants include

* `Skip` is always present
* reward options must be one-step actions
* non-skip options in the same offer must not be duplicates
* deduplication uses canonical resulting deck state
* upgrade generation remains legality-based
* replace generation remains legality-based
* replace fills remaining non-skip capacity after upgrade selection

### Default reward-scale values include

* default total offer size
* default upgrade target
* default replacement trait count

These defaults are useful for the v1 baseline and test expectations, but they are not the same thing as permanent hard-coded system constants.

---

## Core Reward Structure

Each reward offer contains:

* exactly one `Skip`
* a configurable total number of options
* a set of non-skip options composed from:

  * `Upgrade`
  * `Replace`

### Source of truth

The total offer size is controlled by:

* `RewardGenerationConfig.rewardOfferTotalOptions`

The generator targets up to:

* `RewardGenerationConfig.upgradeTarget`

The actual replace count is not configured as a separate source of truth.

Instead, actual replace count is derived as:

`rewardOfferTotalOptions - 1 - actualUpgradeCount`

where:

* the `1` is the always-present `Skip`
* `actualUpgradeCount` is the number of legal deduplicated upgrades actually selected for this offer

### Important design rule

The system remains:

* upgrade-first
* replace-fallback

That means:

1. generate legal upgrade candidates
2. deduplicate them
3. select up to the configured upgrade target
4. fill the remaining non-skip slots with replace options
5. add exactly one `Skip`

---

## Default Baseline for v1 Reward Structure

The current v1 demo uses the following **default baseline values**:

* default reward offer total options: 4
* default upgrade target: 2
* default replacement trait count: 2

Under that default baseline, the most common final offer structures are:

* `2 Upgrade + 1 Replace + 1 Skip`
* `1 Upgrade + 2 Replace + 1 Skip`
* `0 Upgrade + 3 Replace + 1 Skip`

These are useful default examples, but they are not the only structures the generalized reward system can support.

---

## Why Reward Options Must Be One-Step Actions

The gameplay design explicitly avoids multi-step reward selection.

That means:

* the player should not first choose a reward category and then choose a target
* the target must already be bound inside the option
* the option itself must contain enough data to be applied immediately

Examples:

### Valid upgrade option

* add `Suppress` to card `X`

### Invalid upgrade option

* choose one card to upgrade

### Valid replace option

* replace card `Y` with generated card spec `Z`

### Invalid replace option

* choose one card to replace

This one-step rule strongly shapes the design of `RewardOption`.

---

## Main Reward Objects

The reward system uses these main objects:

* `RewardOffer`
* `RewardOption`
* `UpgradePayload`
* `ReplacePayload`
* `CardSpec`

### RewardOffer

Represents one reward selection event.

Recommended data:

* `OfferId`
* `Options`
* `RewardIndexForCurrentEnemy`

### RewardOption

Represents one directly executable reward action.

Recommended data:

* `OptionId`
* `Type`
* payload appropriate to the type

### UpgradePayload

Represents one in-place card upgrade.

Recommended data:

* `TargetCardInstanceId`
* `AddedTrait`

### ReplacePayload

Represents one card replacement.

Recommended data:

* `TargetCardInstanceId`
* `ReplacementCardSpec`

### CardSpec

Represents the generated specification of a replacement card before it becomes a runtime `CardInstance`.

Recommended data:

* `RpsType`
* `BasePower`
* `Traits`

---

## RewardService Responsibilities

`RewardService` is responsible for:

* generating reward offers
* enumerating legal upgrade candidates
* enumerating legal replace candidates
* deduplicating candidates
* applying a chosen reward to the deck

`RewardService` is not responsible for:

* deciding when rewards happen
* deciding whether the next step is next battle, next enemy, victory, or defeat
* changing run-level flow stage directly

Those responsibilities belong to `RunService`.

---

## Upgrade Generation

## Purpose

Upgrade generation creates legal one-step trait additions for existing player cards.

Each upgrade candidate is an action of the form:

* add trait `T` to card instance `C`

---

## Upgrade Legality Rules

A generated upgrade candidate is legal only if all of the following are true:

1. the target card exists in the current deck
2. the target card currently has fewer than 3 traits
3. the target card does not already have the added trait
4. the resulting trait set does not contain both `ShiftLeft` and `ShiftRight`

These legality checks are mandatory.

---

## Upgrade Candidate Enumeration

Upgrade generation should begin by enumerating all legal upgrade actions from the current deck state.

Conceptually:

* inspect each card in the deck
* inspect each trait that could potentially be added
* keep only the legal resulting actions

This produces a candidate action set such as:

* add `AdjacentAid` to card A
* add `Suppress` to card A
* add `Growth` to card B
* add `Regrow` to card C

---

## Upgrade Candidate Selection

After legal candidates are enumerated and deduplicated, the system selects up to the configured upgrade target.

### Selection rule

Let:

* `upgradeTarget = RewardGenerationConfig.upgradeTarget`

Then:

* if legal deduplicated upgrade count is greater than or equal to `upgradeTarget`, select `upgradeTarget`
* if legal deduplicated upgrade count is less than `upgradeTarget`, select all legal deduplicated upgrades
* if no legal upgrade option exists, select 0

The remaining non-skip slots are filled by replace options.

### Important note

Under the current v1 default baseline:

* `upgradeTarget = 2`

So the old familiar behavior still appears by default, but it is now treated as a default configuration value rather than as a permanently hard-coded reward invariant.

---

## Upgrade Randomization Strategy

For the current demo, upgrade selection should use:

* legal candidate enumeration
* deduplication
* lightweight diversity preference
* random selection from the remaining candidate set

### Recommended current approach

Use simple random selection from the legal deduplicated pool.

If possible:

* prefer not to select too many upgrades that target the same card
* prefer some variety across the selected upgrades

This diversity preference should remain light.
Do not introduce a heavy weighted balancing system in the current demo phase.

---

## Replace Generation

## Purpose

Replace generation creates legal one-step replacement actions for existing player cards.

Each replace candidate is an action of the form:

* replace card instance `C` with generated card spec `S`

---

## Replace Design Choice

The replacement system should not rely on a fully hand-authored list of every possible new card.

Instead, it should use:

* configuration-driven generation rules
* runtime enumeration of legal generated card specs
* candidate deduplication
* random selection from the legal candidate set

This approach preserves randomness while keeping legality deterministic and inspectable.

---

## RewardGenerationConfig

Replacement generation and offer structure are driven by authored config.

The main relevant config object is:

* `RewardGenerationConfig`

The current design expects it to provide:

* `rewardOfferTotalOptions`
* `upgradeTarget`
* `allowedReplacementRpsTypes`
* `allowedReplacementBasePowers`
* `allowedReplacementTraits`
* `replacementTraitCount`

### Important rules

* `Skip` remains fixed at exactly 1 and is not configured separately
* replace count is derived, not authored independently
* default total offer size and default upgrade target come from config
* replacement trait count comes from config

### Current demo rule for base power

For the current demo:

* replacement player cards use a configured allowed base power set

That means the architecture supports a set of allowed base powers,
but in the current demo configuration the allowed set may still contain only one value.

This matches the current gameplay direction where player cards are intended to share a fixed base power and derive identity mainly through traits.

---

## Replace Card Trait Count

A generated replacement card must have exactly `replacementTraitCount` traits.

This value is expected to come from:

* `RewardGenerationConfig.replacementTraitCount`

### Default baseline

For the current v1 baseline:

* `replacementTraitCount = 2`

### Current allowed test range

For the current test-oriented phase:

* `replacementTraitCount` may vary from 0 to 3 inclusive

This means reward generation should not assume that replacement cards are permanently fixed to exactly 2 traits in all test scenarios.

---

## Replace Card Legality Rules

A generated replacement card spec is legal only if all of the following are true:

1. it has a valid RPS type
2. it uses an allowed base power
3. it has exactly `replacementTraitCount` traits
4. all traits in the generated trait set are distinct
5. the generated trait set does not contain both `ShiftLeft` and `ShiftRight`
6. the generated trait count does not exceed the current per-card trait legality limit

These checks apply before the spec is combined with a target card to form a replace action.

---

## Replace Candidate Enumeration

Replace generation should work in two stages.

### Stage 1: Enumerate legal replacement card specs

Generate all legal `CardSpec` values based on:

* allowed RPS types
* allowed base powers
* all legal `k`-trait combinations

where:

* `k = replacementTraitCount`

### Stage 2: Combine generated specs with replacement targets

Combine each legal generated `CardSpec` with each legal target card instance in the current deck to form raw replace candidates.

### Stage 3: Filter and deduplicate

Filter illegal or meaningless candidates, then deduplicate by canonical resulting deck state.

---

## Replace Candidate Selection

After legal replace candidates are enumerated and deduplicated, the system selects enough replace options to fill the remaining non-skip slots.

### Selection rule

Let:

* `totalOptions = RewardGenerationConfig.rewardOfferTotalOptions`
* `skipCount = 1`
* `actualUpgradeCount = number of selected upgrades`

Then:

* `replaceSlotsToFill = totalOptions - skipCount - actualUpgradeCount`

The generator should then select up to `replaceSlotsToFill` distinct legal replace options.

### Important rule

Replace count is a derived consequence of total offer size and actual selected upgrade count.

It is not an independently authored count in the current design.

---

## Meaningless Replace Filtering

Not every legal generated replacement spec is necessarily a meaningful replace candidate.

The generator should filter candidates that are equivalent to leaving the target card effectively unchanged.

### Example

If a replace option would produce a card that is functionally identical to the target card under canonical comparison, that option should not remain in the final replace candidate pool.

This keeps offers more meaningful.

---

## Reward Deduplication Rule

The accepted deduplication rule remains:

* deduplicate by canonical resulting deck state after the option is applied

This rule ignores:

* card instance identity
* deck order
* trait order

This rule applies within a single reward offer.

This principle is locked and is described in detail in ADR-0003. 

---

## Canonical Deduplication Procedure

The reward generation pipeline should apply deduplication like this:

### Step 1

Generate legal raw reward candidates.

### Step 2

For each candidate:

* apply it to a temporary copy of the current deck
* produce the resulting deck state

### Step 3

Convert the resulting deck into a canonical deck signature.

### Step 4

Use the canonical deck signature as the deduplication key.

### Step 5

Discard any candidate whose deduplication key has already been seen.

### Step 6

Select final reward options from the remaining deduplicated candidate pool.

This procedure should be used for:

* `Upgrade`
* `Replace`

`Skip` is always unique by category and does not participate in non-skip equivalence comparison.

---

## Why Canonical Deduplication Still Matters

The shift from fixed default offer size to configurable offer size does not change the need for strong deduplication.

The system still needs to prevent:

* duplicate upgrades targeting functionally identical cards
* duplicate replacement results that differ only by target identity
* duplicate replacement specs that differ only by trait order
* duplicate results that differ only by deck position

Canonical resulting deck comparison remains the correct rule.

---

## Skip Option

Every reward offer always includes exactly one `Skip`.

### Purpose

`Skip` exists because:

* the player should always have a no-change option
* reward choice should remain readable
* forcing a deck change every time is not always desirable

### Important rule

`Skip` is a real reward option.

Selecting `Skip`:

* spends that reward opportunity
* does not modify the deck
* does not remain available later as a deferred reward token

---

## Reward Application

Once a reward option has been selected, `RewardService` applies it to the player deck.

### Upgrade application

Upgrade application should:

* find the target card instance
* add the specified trait
* preserve the card's existing runtime identity
* preserve the card's existing permanent power bonus

### Replace application

Replace application should:

* find the target card instance
* create a new `CardInstance` from the replacement `CardSpec`
* assign the new card a new runtime instance id
* set its permanent power bonus to 0
* replace the old card in the deck

### Skip application

Skip application should:

* leave the deck unchanged

---

## Important Invariants

The reward system should preserve the following invariants:

1. each reward offer has exactly `rewardOfferTotalOptions` options
2. exactly one option is `Skip`
3. non-skip options are pairwise distinct by canonical resulting deck state
4. every generated option is legal and executable in one step
5. applying the selected option results in a legal deck
6. actual non-skip composition is upgrade-first with replace fallback
7. replace count is derived rather than configured independently
8. replacement card trait count must match configured `replacementTraitCount`

---

## Default-Config Composition Examples

The following examples assume the current default baseline values:

* `rewardOfferTotalOptions = 4`
* `upgradeTarget = 2`

### Case A: at least 2 legal deduplicated upgrades exist

Final offer structure:

* `2 Upgrade + 1 Replace + 1 Skip`

### Case B: exactly 1 legal deduplicated upgrade exists

Final offer structure:

* `1 Upgrade + 2 Replace + 1 Skip`

### Case C: no legal deduplicated upgrades exist

Final offer structure:

* `0 Upgrade + 3 Replace + 1 Skip`

These examples remain useful for the v1 baseline, but they are examples of the default configuration, not the only generalized system behavior.

---

## What the Current Design Intentionally Avoids

The current reward design intentionally avoids:

* multi-step reward targeting
* separate replace-count source-of-truth config
* fully hand-authored replacement catalogs
* reward options that are visually different but canonically identical
* heavy weighted reward bias systems
* late-phase overengineering of reward generation

These are not needed for the current demo phase.

---

## Testing Implications

Reward generation remains one of the highest-value test targets in the project.

High-value tests include two layers.

### Layer 1: generalized invariant tests

These should verify:

* exactly one `Skip`
* total option count equals configured `rewardOfferTotalOptions`
* actual upgrade count never exceeds configured `upgradeTarget`
* actual replace count fills remaining non-skip slots
* non-skip options are canonically deduplicated
* all generated options are legal
* replacement card trait count matches configured `replacementTraitCount`
* replacement trait legality still rejects duplicate traits and left/right conflict
* applying the chosen option produces a legal deck

### Layer 2: default-baseline example tests

These should verify current baseline behavior when:

* `rewardOfferTotalOptions = 4`
* `upgradeTarget = 2`
* `replacementTraitCount = 2`

For that default baseline, tests should still verify:

* `2 Upgrade + 1 Replace + 1 Skip`
* `1 Upgrade + 2 Replace + 1 Skip`
* `0 Upgrade + 3 Replace + 1 Skip`

This keeps tests aligned with both:

* the generalized system
* the current v1 baseline

---

## Relationship to Run Flow

This document defines:

* what one reward offer looks like
* how its options are generated
* how those options are applied

It does not decide:

* when rewards happen
* how many reward opportunities an enemy stage grants
* how early settlement on enemy defeat is orchestrated
* when the next battle or next enemy begins

Those timing and flow questions belong to `run-battle-reward-flow.md`.

---

## Summary

The current reward design is:

* one-step
* legality-driven
* deduplicated by canonical resulting deck state
* upgrade-first with replace fallback
* explicit about the difference between locked reward invariants and configurable default offer scale

The key current rules are:

* exactly one `Skip`
* configurable total offer size
* configurable upgrade target
* replace count derived from remaining non-skip capacity
* configurable replacement trait count with current allowed test range 0..3
* canonical resulting deck deduplication

This keeps the reward system simple enough for the current demo while making gameplay testing more flexible and more consistent with the newer config-driven design direction.
