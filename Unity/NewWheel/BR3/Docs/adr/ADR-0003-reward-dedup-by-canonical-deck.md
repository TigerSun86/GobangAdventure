# ADR-0003: Reward Deduplication by Canonical Resulting Deck State

## Status

Accepted

---

## Context

The reward system for the current demo generates one reward offer at a time.

Each reward offer contains:

* exactly 4 options
* exactly 1 `Skip`
* exactly 3 non-skip options

The non-skip options are composed from:

* `Upgrade`
* `Replace`

The reward system must prevent duplicate options from appearing in the same offer.

At first glance, deduplication could be implemented by comparing raw reward action parameters, such as:

* target card instance id
* added trait
* replacement target card instance id
* replacement card spec

However, the gameplay rules and card model create cases where two reward actions look different at the raw parameter level but still lead to the same gameplay-relevant result.

A stronger deduplication rule is needed.

---

## Decision

Reward options will be deduplicated by the canonical resulting deck state after the option is applied.

Two non-skip reward options are considered identical if applying them would produce the same canonical resulting deck signature.

This deduplication rule ignores:

* card instance identity
* deck order
* trait order

This rule applies within a single reward offer.

---

## Detailed Decision

## Core rule

A reward candidate is not deduplicated by raw action identity.

It is deduplicated by:

* simulate applying the candidate to the current deck
* compute the canonical resulting deck signature
* compare that canonical resulting deck signature against the signatures of other candidates

If two candidates produce the same canonical resulting deck signature, only one of them may remain in the final offer.

---

## What This Means for Upgrade

Two upgrade options may be considered duplicates even if:

* they target different card instances
* they add different traits to different cards at the raw action level

If the resulting deck, after canonicalization, is functionally the same, then the two upgrade options are duplicates.

### Example

Assume the current deck contains two functionally identical cards.

If:

* option A upgrades the first card
* option B upgrades the second card

and both upgrades produce the same canonical resulting deck state, then only one of them should be kept.

---

## What This Means for Replace

Two replace options may be considered duplicates even if:

* they target different identical cards
* they were generated as different action instances
* their trait ordering differs in the replacement card spec

If the resulting deck, after canonicalization, is functionally the same, then the two replace options are duplicates.

---

## Why Raw Action Deduplication Is Not Enough

The following cases show why raw action comparison is too weak.

### Case 1: different target instance, same gameplay result

Two identical cards exist in the deck.
Upgrading either one leads to the same effective final deck.

### Case 2: trait order difference only

Two replacement specs contain the same trait set but in different order.

### Case 3: deck order difference only

Two options produce the same multiset of cards but leave them in different positions.

In the current design, deck order is ignored for reward-equivalence purposes.
So these options should not both appear.

---

## Canonical Card Signature

Each card is reduced to a canonical gameplay signature.

The canonical card signature includes:

* `RpsType`
* `BasePower`
* `PermanentPowerBonus`
* sorted trait set

It does not include:

* runtime instance identity
* deck index
* object reference identity

The sorted trait set rule ensures that trait order is not treated as meaningful.

---

## Canonical Deck Signature

The canonical deck signature is built by:

1. converting each card in the resulting deck into its canonical card signature
2. sorting the set of canonical card signatures in a stable order
3. building a deck-level signature from that sorted list

This means the canonical deck signature ignores:

* which specific runtime instance was changed
* where in the deck the card appears
* trait ordering inside a card

This is intentional.

---

## Why Deck Order Is Ignored

In the current gameplay design, reward deduplication is meant to reflect gameplay variety rather than presentation variety.

Deck order is not currently treated as meaningful gameplay identity for reward selection.

Because of that:

* two reward candidates that differ only by deck position should not appear as separate options

If future gameplay makes deck order mechanically important, this decision should be revisited.

---

## Why Instance Identity Is Ignored

Runtime card instance identity is important for gameplay execution, battle use tracking, and reward targeting.

However, reward deduplication is not about preserving runtime identity distinctions.
It is about preserving meaningful choice variety.

Therefore:

* instance identity is ignored during reward deduplication
* gameplay equivalence matters more than object identity for offer variety

---

## Why Trait Order Is Ignored

Trait order currently has no gameplay meaning.

A card with:

* `[Empower, AdjacentAid]`

is functionally equivalent to a card with:

* `[AdjacentAid, Empower]`

Therefore:

* trait order must not create distinct reward options

---

## Deduplication Procedure

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

## Consequences for Reward Generation

This decision has several direct consequences.

### Consequence 1

Reward generation must simulate candidate application before final selection.

### Consequence 2

Reward generation needs canonical signature helpers.

### Consequence 3

Some apparently different upgrade candidates will intentionally collapse into one.

### Consequence 4

Some apparently different replace candidates will intentionally collapse into one.

### Consequence 5

The reward system becomes more aligned with actual gameplay choice variety.

---

## Why This Is Worth the Extra Complexity

The extra complexity is small for the current demo because:

* deck size is only 6
* reward offer size is only 4
* trait set is small
* candidate generation space is manageable

In return, the system avoids fake variety.

This is a good trade for the current project.

---

## Alternatives Considered

## Alternative 1: Deduplicate only by raw action payload

Rejected.

### Why

This allows options that look different but lead to the same effective deck result.
That weakens the actual variety of the reward offer.

---

## Alternative 2: Deduplicate only by target card and resulting card spec

Rejected.

### Why

This still fails when multiple functionally identical cards exist in the deck and the target instance differs.

---

## Alternative 3: Do not deduplicate at all beyond exact string equality

Rejected.

### Why

This would produce clearly redundant offers and reduce meaningful player choice.

---

## Consequences

## Positive consequences

* stronger reward variety
* fewer redundant reward options
* better alignment between reward presentation and gameplay significance
* clearer reward-system correctness
* easier reasoning about whether an offer really contains distinct choices

## Negative consequences

* reward generation must simulate candidate application
* canonical signature helpers are required
* implementation is more sophisticated than simple payload comparison

These tradeoffs are accepted.

---

## Testing Implications

This decision requires dedicated tests.

Important test areas include:

* duplicate upgrade removal for identical resulting deck states
* duplicate replace removal for identical resulting deck states
* trait-order-insensitive equivalence
* deck-order-insensitive equivalence
* instance-identity-insensitive equivalence
* non-equivalent options remaining distinct

These tests are important because reward duplication bugs may not crash the game, but they do reduce gameplay quality.

---

## Revisit Conditions

This decision should be revisited if:

* deck order becomes mechanically meaningful
* trait order becomes mechanically meaningful
* reward offers need to preserve identity-sensitive distinctions for new gameplay reasons

Until then, canonical resulting deck deduplication remains the accepted rule.

---

## Summary

Reward options are deduplicated by canonical resulting deck state, not by raw action parameters.

This means:

* instance identity is ignored
* deck order is ignored
* trait order is ignored

The purpose is to ensure that one reward offer presents genuinely different gameplay choices rather than superficial variations of the same outcome.