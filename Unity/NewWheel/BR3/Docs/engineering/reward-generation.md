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
* how authored reward-generation config participates in replacement generation
* how the current demo constraints differ from possible future extensions

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
7. future expansion should remain possible without redesigning the whole structure

---

## Core Reward Structure

Each reward offer contains exactly 4 options.

The structure is:

* 3 non-skip options
* 1 skip option

The non-skip options are composed from:

* `Upgrade`
* `Replace`

### Intended composition rule

The system should use up to 2 different legal `Upgrade` options.

If fewer than 2 different legal `Upgrade` options exist, the remaining non-skip slots are filled with `Replace` options.

This means a reward offer can have one of the following structures:

* `2 Upgrade + 1 Replace + 1 Skip`
* `1 Upgrade + 2 Replace + 1 Skip`
* `0 Upgrade + 3 Replace + 1 Skip`

This structure is locked for the current implementation design.

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

After legal candidates are enumerated and deduplicated, the system selects up to 2 different upgrade options.

### Selection rule

* if 2 or more different legal upgrade options exist, select 2
* if exactly 1 different legal upgrade option exists, select 1
* if no legal upgrade option exists, select 0

The remaining non-skip slots are filled by replace options.

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

* prefer not to select two upgrades that target the same card
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

Replacement generation is driven by authored config.

The main relevant config object is:

* `RewardGenerationConfig`

The current design expects it to provide:

* `allowedReplacementRpsTypes`
* `allowedReplacementBasePowers`
* `allowedReplacementTraits`
* `replacementTraitCount`

### Current demo rule

For the current demo:

* replacement player cards use a single configured base power value

That means the architecture supports a set of allowed base powers,
but in the current demo configuration the allowed set contains only one value.

This matches the current gameplay direction where player cards are intended to share a fixed base power and derive identity mainly through traits.

---

## Replace Card Trait Count

A generated replacement card must have exactly 2 traits in the current design.

This is part of the locked reward behavior for replacement cards.

This count is expected to come from:

* `RewardGenerationConfig.replacementTraitCount`

For the current demo, that value is fixed to 2.

---

## Replace Card Legality Rules

A generated replacement card spec is legal only if all of the following are true:

1. it has a valid RPS type
2. it uses an allowed base power
3. it has exactly 2 traits
4. the 2 traits are distinct
5. the trait pair does not contain both `ShiftLeft` and `ShiftRight`

These checks apply before the spec is combined with a target card to form a replace action.

---

## Replace Candidate Enumeration

Replace generation should work in two stages.

### Stage 1: Enumerate legal replacement card specs

Generate all legal `CardSpec` values based on:

* allowed RPS types
* allowed base powers
* all legal 2-trait combinations

### Stage 2: Combine with target deck cards

For each card in the current deck:

* pair the target card with each legal generated card spec
* keep only meaningful replacements

A replacement is meaningful only if it actually changes the deck in a gameplay-relevant way.

---

## Meaningless Replace Candidates

A replace candidate should be discarded if the generated replacement `CardSpec` is functionally identical to the target card being replaced.

For this comparison, use gameplay-relevant card properties:

* `RpsType`
* `BasePower`
* `Traits`

Do not consider:

* `InstanceId`
* deck slot index
* current object identity

If replacement would produce the same card specification, it should not be offered.

---

## Why Pure Rejection Sampling Is Not Preferred

The system could theoretically generate random replacement cards and retry when illegal.

However, this is not the preferred design.

### Reasons

1. the legal candidate space is small enough to enumerate directly
2. direct enumeration is easier to test
3. direct enumeration is easier to debug
4. direct enumeration avoids unclear retry behavior
5. direct enumeration gives a stable base for deduplication

For the current demo, enumeration is the cleaner design.

---

## Skip Option

Each reward offer always includes exactly one `Skip` option.

`Skip`:

* changes nothing in the deck
* does not require legality filtering
* does not require candidate generation

It should still be represented explicitly as a normal `RewardOption` with type `Skip`.

---

## Reward Option Deduplication

This is one of the most important parts of the reward system.

Reward options must not be deduplicated only by raw action parameters.

Instead, they must be deduplicated by the resulting deck state after the option is applied.

### Core Rule

Two reward options are considered identical if applying them would produce the same canonical resulting deck signature.

This rule applies to:

* upgrade options
* replace options
* comparisons across options in the same reward offer

For the detailed architectural reasoning behind this rule, see:

* `Docs/adr/ADR-0003-reward-dedup-by-canonical-deck.md`

---

## Why Raw Action Deduplication Is Not Enough

Raw action comparison is not sufficient because different-looking actions can still produce equivalent deck outcomes.

Examples:

* two upgrades target different but functionally identical cards and produce equivalent final deck states
* two replacement specs contain the same traits in different orders
* two replace actions target different identical cards and produce equivalent final deck states

Therefore deduplication must use resulting deck equivalence, not just action payload equality.

---

## Canonical Card Signature

To support reward deduplication, each card must be reducible to a canonical gameplay signature.

The canonical card signature should conceptually include:

* `RpsType`
* `BasePower`
* `PermanentPowerBonus`
* sorted trait set

For reward-result comparison involving generated replacement results, the important concept is gameplay equivalence, not runtime identity.

Trait order must not matter.

---

## Canonical Deck Signature

A deck should be converted into a canonical deck signature by:

1. converting each card into its canonical card signature
2. sorting the card signatures in a stable way
3. building a canonical deck-level representation from the sorted signatures

This means canonical deck comparison ignores:

* card instance identity
* deck order
* trait order

This rule is intentional and locked for reward deduplication.

---

## Reward Deduplication Procedure

For each generated reward candidate:

1. simulate applying it to a temporary copy of the current deck
2. compute the canonical deck signature of the resulting deck
3. use that canonical signature as the deduplication key

If another candidate already produced the same canonical deck signature:

* discard the new candidate as a duplicate

Otherwise:

* keep the candidate

This procedure should be applied before random selection of final options.

---

## Final Reward Offer Construction

The final offer should be built in this order:

### Step 1

Enumerate and deduplicate all legal upgrade candidates.

### Step 2

Select up to 2 different upgrade options.

### Step 3

Determine how many non-skip slots remain.

### Step 4

Enumerate and deduplicate all legal replace candidates.

### Step 5

Select enough different replace options to fill the remaining non-skip slots.

### Step 6

Add exactly 1 skip option.

This produces exactly 4 total options:

* 3 non-skip
* 1 skip

---

## Current Demo Composition Examples

### Case A: many upgrades available

* 2 upgrades
* 1 replace
* 1 skip

### Case B: only one distinct legal upgrade exists

* 1 upgrade
* 2 replaces
* 1 skip

### Case C: no legal upgrades exist

* 3 replaces
* 1 skip

These are all valid outcomes.

---

## Reward Application

After the player selects one reward option, `RewardService` applies it to the current deck.

---

## Applying Upgrade

Applying an upgrade means:

* find the target card instance
* add the specified new trait
* keep the same card instance identity
* keep the same deck position

### Important Rule

Upgrade is an in-place modification of an existing card instance.

It does not create a new card instance.

---

## Applying Replace

Applying a replace means:

* find the target card's current deck position
* create a new runtime `CardInstance` from the selected replacement `CardSpec`
* replace the old card at the same deck position
* assign a new `InstanceId`
* start with no inherited permanent growth from the replaced card

### Important Rule

Replace is not a mutation of the old card.
It is a true replacement.

The old card's:

* traits
* permanent power bonus
* instance identity

should not be inherited by the new card.

---

## Applying Skip

Applying skip means:

* do not change the deck

It is still a valid resolved reward choice and still counts as the player's selected option for that reward event.

---

## Deck Legality After Reward Application

After applying any selected reward, the resulting player deck must still be legal.

That means:

* no card has duplicate traits
* no card has more than 3 traits
* no card has both `ShiftLeft` and `ShiftRight`

Reward generation is expected to prevent illegal options in advance, but reward application should still assume legality matters and may validate or assert this during implementation.

---

## Important Invariants

The reward system should always maintain the following invariants:

1. each reward offer has exactly 4 options
2. exactly 1 option is `Skip`
3. non-skip options are pairwise distinct by resulting canonical deck state
4. every generated option is legal and executable in one step
5. applying the selected option results in a legal deck
6. replacement cards in the current demo use the configured fixed player replacement base power from `RewardGenerationConfig`

---

## Testing Implications

The reward system should be covered by targeted Edit Mode tests.

High-value test areas include:

* upgrade legality checks
* replace legality checks
* fallback from missing upgrades to extra replaces
* reward offer composition
* canonical deck signature behavior
* trait-order-insensitive equivalence
* deck-order-insensitive equivalence
* duplicate upgrade removal
* duplicate replace removal
* upgrade application
* replace application
* skip application

These tests are especially valuable because reward bugs may not crash the game but can still silently reduce option variety or violate gameplay rules.

---

## Common Failure Risks

Important implementation risks include:

1. treating raw action difference as meaningful when resulting deck states are equivalent
2. forgetting to ignore trait order in canonical comparison
3. forgetting to ignore deck order in canonical comparison
4. allowing duplicate non-skip options into one reward offer
5. allowing illegal trait combinations in replacement card generation
6. accidentally inheriting replaced card growth or identity
7. generating meaningless replacements that do not actually change the deck
8. ignoring authored replacement config and hardcoding replacement generation values in code

These should be tested early.

---

## Summary

The reward system is based on:

* fixed offer structure
* legal one-step options
* upgrade-first composition with replace fallback
* configuration-driven replacement generation
* canonical resulting deck deduplication
* explicit reward application

This design keeps the system:

* readable
* deterministic in legality
* random in meaningful content
* easy to test
* easy to debug
* extensible for future balancing work