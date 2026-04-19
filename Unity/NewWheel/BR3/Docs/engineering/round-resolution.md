# Round Resolution

## Purpose

This document defines the design of `RoundResolver`, the component responsible for resolving exactly one round of gameplay.

It explains:

* the responsibility and boundaries of `RoundResolver`
* the expected inputs and outputs
* the fixed round pipeline
* what each phase reads and writes
* which values persist and which are recalculated each round
* the role of logs and phase snapshots

This document is one of the most important engineering references for the current demo because `RoundResolver` is the core rule engine.

---

## Role of RoundResolver

`RoundResolver` resolves one round of gameplay.

It is the component that applies the fixed gameplay sequence for a round and produces a complete `RoundResult`.

### RoundResolver is responsible for:

* entering the newly played cards into the board
* recalculating round-local combat values
* applying movement
* applying board-derived effects
* resolving slot combat
* calculating round damage and healing totals
* applying post-resolve effects that belong to this round
* producing logs and phase snapshots

### RoundResolver is not responsible for:

* deciding whether a battle should start
* waiting for player input
* selecting the enemy card sequence
* advancing battle-level flow state
* advancing run-level flow state
* generating rewards
* deciding victory or defeat

Those responsibilities belong elsewhere.

---

## Why the Name Is RoundResolver

The game uses simultaneous play and round-based resolution.

The gameplay concept here is a round, not an alternating turn in the traditional sense.

For that reason:

* `RoundResolver` is preferred over `TurnResolver`
* `Resolver` is preferred over `Service`

This reflects the intended responsibility:

* `RunService`, `BattleService`, and `RewardService` orchestrate flow
* `RoundResolver` resolves rules

---

## Inputs and Outputs

### Conceptual Inputs

`RoundResolver` needs access to:

* the current `BattleState`
* the newly selected player card for this round
* the enemy card for this round
* current player HP
* current enemy HP

The exact method signature may evolve slightly during implementation, but the conceptual inputs are stable.

### Conceptual Output

`RoundResolver` returns a `RoundResult`.

`RoundResult` must contain:

* round index
* damage totals
* healing totals
* HP before and after
* per-slot combat results
* logs
* per-phase snapshots

### Important Design Choice

`RoundResolver` should return the resolved round result rather than own higher-level flow progression.

Battle and run progression remain the responsibility of higher-level services.

---

## Fixed Round Pipeline

The round pipeline is locked and must not be reordered.

The seven phases are:

1. Enter
2. Fixed Self
3. Movement
4. Board Derived
5. Resolve Open Slots
6. Apply Merged Damage
7. Post Resolve

The order is part of the gameplay design and is not an implementation detail.

---

## Resolution Philosophy

The round system follows these core ideas:

### 1. Board position persists across rounds

Cards remain on the board during the battle unless future gameplay rules say otherwise.

### 2. Combat values do not persist across rounds

Combat power is recalculated every round from persistent data plus current board state.

### 3. Damage is accumulated before being applied

Slot combat does not immediately reduce HP. Damage is first accumulated and then applied simultaneously.

### 4. Post-resolve effects only apply to cards newly played this round

Cards already on the board do not retrigger these effects in later rounds.

### 5. Debug visibility is part of the design

The resolver should produce enough information to inspect each phase clearly.

---

## Round-Local Context

Inside `RoundResolver`, it is useful to maintain a temporary round-local context object.

This is not authoritative runtime state. It is only a temporary working structure used during one round resolution.

It should conceptually track:

* round index
* HP before resolution
* references to the newly entered player and enemy board cards
* accumulated damage totals
* accumulated healing totals
* slot combat results
* logs
* snapshots
* temporary power deltas for board-derived effects

This object does not need to become a long-lived public domain object unless implementation later benefits from it.

---

## Phase 1: Enter

### Purpose

This phase places the newly selected player card and the enemy card for the current round onto the board.

### Reads

* `BattleState.RoundIndex`
* player selected `CardInstance`
* enemy selected card
* `BattleState.PlayerLane`
* `BattleState.EnemyLane`
* `BattleState.UsedPlayerCardIds`

### Writes

* current slot `IsOpen = true` on both sides
* current slot `Occupant`
* `UsedPlayerCardIds`
* references to the new board cards in round-local context
* logs
* snapshot

### Behavior

The round index determines which slot is opened this round.

The player and enemy cards enter the matching slot index for this round.

The player card is marked as used for this battle.

### Important Notes

* This phase does not calculate combat power.
* This phase does not move cards.
* This phase does not resolve combat.

---

## Phase 2: Fixed Self

### Purpose

This phase recalculates fixed self-based power for all cards currently on the board.

### Reads

* every occupied `BoardCard`
* `BoardCard.SourceCard.BasePower`
* `BoardCard.SourceCard.PermanentPowerBonus`
* `BoardCard.SourceCard.Traits`

### Writes

* `BoardCard.FixedSelfPower`
* `BoardCard.CurrentPower`
* `BoardCard.DamageDealtThisRound = 0`
* logs
* snapshot

### Behavior

All occupied board cards are recalculated from persistent source data.

The current formula is:

base power

* permanent power bonus
* fixed-self trait effects

At the current design stage, the main fixed-self effect is:

* `Empower`: +3

### Important Rule

This phase recalculates values for all on-board cards every round.

This means:

* board presence persists
* board position persists
* current combat power does not persist

### Important Reason

Without full recalculation here, power values from previous rounds would leak incorrectly into later rounds.

---

## Phase 3: Movement

### Purpose

This phase applies board movement effects.

### Reads

* current board occupancy on each side
* card traits on occupied slots

### Writes

* slot occupancy order on each lane
* logs
* snapshot

### Behavior

Movement is processed separately for the player lane and enemy lane.

For each lane:

* inspect slots from right to left
* if a card has `ShiftLeft`, swap with the left adjacent ally if one exists
* otherwise, if a card has `ShiftRight`, swap with the right adjacent ally if one exists

### Locked Rule

Movement is processed from right to left.

This is a gameplay rule and must not be replaced by a collect-then-apply batch approach.

### Important Notes

* movement swaps occupied allied positions
* movement does not change `EnterRoundIndex`
* movement does not directly change combat power
* movement result persists to the next round because slot occupancy has changed

---

## Phase 4: Board Derived

### Purpose

This phase applies board-dependent derived effects after movement has finished.

### Reads

* moved board positions
* current occupied slots
* traits on occupied cards
* current `BoardCard.CurrentPower`

### Writes

* updated `BoardCard.CurrentPower`
* logs
* snapshot

### Current Effects

The current board-derived effects are:

* `AdjacentAid`
* `Suppress`

### Behavior

Derived effects should be calculated in two steps:

#### Step 1

Collect deltas into temporary arrays for each side.

#### Step 2

Apply the deltas to `CurrentPower`.

### Why Use Deltas

Directly modifying `CurrentPower` while scanning would create order-dependent behavior inside the same phase.

Using temporary deltas keeps this phase easier to reason about and debug.

### Current Trait Rules

#### AdjacentAid

A card with `AdjacentAid` gives +2 power to each adjacent allied card.

#### Suppress

A card with `Suppress` gives -2 power to the opposing enemy card in the same slot.

### Important Notes

* this phase happens after movement
* this phase does not apply damage
* no artificial lower bound should be introduced unless gameplay rules explicitly require one

---

## Phase 5: Resolve Open Slots

### Purpose

This phase resolves rock-paper-scissors combat at every open slot.

### Reads

* all open slots
* both sides' occupied board cards at those slots
* each card's `CurrentPower`
* each card's `RpsType`

### Writes

* per-slot combat results
* accumulated damage totals in round-local context
* `BoardCard.DamageDealtThisRound`
* logs
* snapshot

### Behavior

For each open slot:

* compare player and enemy RPS types
* determine winner or tie
* if tie, deal no damage
* if one side wins, deal:

max(1, winner power - loser power)

Damage is accumulated, not immediately applied to HP.

### Important Notes

* slot combat should be represented explicitly using `SlotCombatResult`
* damage dealt by the specific winning board card must be recorded
* this phase must not directly modify player or enemy HP

### Why BoardCard Damage Tracking Matters

`Lifesteal` depends on damage dealt by the newly played card itself, not by the team total.

---

## Phase 6: Apply Merged Damage

### Purpose

This phase applies accumulated damage totals simultaneously.

### Reads

* accumulated damage to player
* accumulated damage to enemy
* current player HP
* current enemy HP

### Writes

* player HP after damage
* enemy HP after damage
* HP before/after values in `RoundResult`
* logs
* snapshot

### Behavior

This phase applies:

* total damage to player HP
* total damage to enemy HP

Both are treated as simultaneous consequences of the round.

### Important Rule

Damage must be accumulated first and applied together.

The resolver must not reduce HP slot-by-slot during combat resolution.

---

## Phase 7: Post Resolve

### Purpose

This phase applies post-resolve effects to cards newly played this round.

### Reads

* newly entered player board card
* newly entered enemy board card
* each new card's traits
* `DamageDealtThisRound`

### Writes

* player healing total
* enemy healing total
* player HP after healing
* enemy HP after healing
* source card permanent growth when applicable
* logs
* snapshot

### Current Effects

The current post-resolve effects are:

* `Regrow`
* `Lifesteal`
* `Growth`

### Behavior

This phase should only inspect the two cards that entered this round.

#### Regrow

Heal 2 HP.

#### Lifesteal

If the card dealt damage this round, heal that amount.

#### Growth

Increase the source card's permanent power bonus by 1.

### Important Rule

`Growth` modifies persistent card state, not current-round board combat power.

That means it should update:

* `BoardCard.SourceCard.PermanentPowerBonus`

It must not retroactively affect already resolved combat this round.

### Suggested Internal Order

A stable internal order is recommended:

1. `Regrow`
2. `Lifesteal`
3. `Growth`

This keeps behavior deterministic and easy to inspect.

---

## Logs

### Purpose

Round logs are intended to support:

* debugging
* implementation verification
* future step-by-step presentation

### Expectations

Logs should be:

* concise
* phase-aware
* useful for understanding what happened

### Suggested Content

Typical log entries may describe:

* entered cards
* movement swaps
* derived buffs and debuffs
* slot combat results
* post-resolve healing and growth

Logs are not the authoritative state, but they are an important inspection tool.

---

## Phase Snapshots

### Purpose

Phase snapshots capture board state after each resolution phase.

### Why They Exist

They support:

* debugging
* round inspection
* future presentation sequencing
* clearer failure analysis during testing

### Scope

Snapshots should be captured after each of the seven phases, not only once at the end.

That means a round can produce snapshots for:

* Enter
* Fixed Self
* Movement
* Board Derived
* Resolve Open Slots
* Apply Merged Damage
* Post Resolve

### Important Note

Snapshots are not authoritative gameplay state.
They are derived inspection data and normally should not be treated as mandatory save data.

---

## What Persists After a Round

The following should persist after the round finishes:

* player HP
* enemy HP
* current board positions
* slot open state
* cards already used this battle
* permanent power growth from `Growth`

---

## What Does Not Persist After a Round

The following should be recalculated or reset next round:

* `CurrentPower`
* `FixedSelfPower`
* `DamageDealtThisRound`
* board-derived temporary deltas
* accumulated damage totals
* accumulated healing totals

---

## Important Summary Rule

A good mental model is:

position persists
combat values recalculate
growth persists
damage totals are temporary

This is one of the most important conceptual rules in the whole combat system.

---

## Responsibilities That Must Stay Outside RoundResolver

The following tasks must remain outside `RoundResolver`:

* waiting for the player to choose a card
* deciding whether the battle is complete
* advancing `BattleState.RoundIndex`
* generating enemy card sequences
* creating reward offers
* deciding next battle, next enemy, victory, or defeat
* UI transitions and animations

---

## Common Failure Risks

The most important implementation risks are:

1. carrying `CurrentPower` across rounds instead of recalculating it
2. applying `Growth` to current-round power instead of persistent source card state
3. using team total damage instead of per-card damage for `Lifesteal`
4. evaluating board-derived effects before movement
5. applying HP damage slot-by-slot instead of simultaneously
6. retriggering post-resolve effects for cards that were not newly played this round

Tests should be designed to catch these failures early.

---

## Testing Implications

The resolver is one of the best targets for Edit Mode tests.

High-value tests include:

* RPS outcome rules
* minimum damage rule
* empower recalculation
* movement order
* adjacent aid behavior
* suppress behavior
* simultaneous damage application
* regrow
* lifesteal
* growth
* snapshot count and phase order

---

## Summary

`RoundResolver` is the rule engine for exactly one round.

It:

* follows a locked seven-phase pipeline
* recalculates round-local combat values every round
* updates battle-relevant state
* returns a complete `RoundResult`
* supports debugging through logs and snapshots

It does not manage battle progression or run progression.
Those remain the responsibility of higher-level services.