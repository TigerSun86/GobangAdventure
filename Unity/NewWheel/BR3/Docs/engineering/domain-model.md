# Domain Model

## Purpose

This document defines the core runtime state objects and result objects used by the Unity 6.4 2D turn-based card roguelike demo.

It explains:

* what each domain object represents
* which data belongs to which object
* which data is persistent versus temporary
* how the main domain objects relate to each other
* which objects are authoritative state and which are derived or short-lived

This document focuses on structure and ownership of data. Behavior details are covered in:

* `round-resolution.md`
* `run-battle-reward-flow.md`
* `reward-generation.md`

---

## Domain Modeling Principles

The domain model follows these principles:

1. state lives in state objects, not in services
2. persistent runtime state is separated from temporary round computation
3. player deck state is separated from board-projected battle state
4. result objects are explicit and inspectable
5. debug visibility is a first-class concern
6. domain objects should remain as Unity-agnostic as practical

---

## State Hierarchy Overview

At runtime, the main state hierarchy is:

```
RunState
├── PlayerHp
├── PlayerDeck : List<CardInstance>
├── CurrentEnemyIndex
├── CurrentEnemy : EnemyProgressState
├── ActiveBattle : BattleState?
├── PendingRewardOffer : RewardOffer?
└── FlowStage : RunFlowStage
```

`RunState` is the root of the authoritative runtime state for the current run.

---

## Main Object Categories

The domain model contains four major categories of objects:

### 1. Persistent runtime state

Objects that survive across multiple steps of the run:

* `RunState`
* `EnemyProgressState`
* `CardInstance`

### 2. Battle runtime state

Objects that survive during a battle:

* `BattleState`
* `LaneState`
* `BoardSlotState`
* `BoardCard`

### 3. Reward runtime state

Objects that exist while a reward choice is pending:

* `RewardOffer`
* `RewardOption`
* reward payload objects

### 4. Result and debug objects

Objects that describe what happened:

* `RoundResult`
* `SlotCombatResult`
* `PhaseSnapshot`

---

## RunState

### Purpose

`RunState` is the authoritative root object for the entire run.

If the current run must be understood, displayed, debugged, or later saved, `RunState` is the starting point.

### Responsibilities

`RunState` owns:

* player-wide persistent combat state
* player deck state
* current enemy progression
* current battle reference
* current pending reward reference
* run-level flow state

### Recommended Fields

* `PlayerHp`
* `PlayerDeck`
* `CurrentEnemyIndex`
* `CurrentEnemy`
* `ActiveBattle`
* `PendingRewardOffer`
* `FlowStage`

### Notes

* `RunState` should not contain round-local combat values.
* `RunState` should not contain UI-only state.
* run termination can be derived from `FlowStage`, so separate `IsVictory` or `IsRunOver` fields are not required.

---

## EnemyProgressState

### Purpose

`EnemyProgressState` represents the progression state of the current enemy across up to three battles.

### Responsibilities

It tracks:

* which enemy is currently being fought
* current enemy HP
* how many battles have been played against this enemy
* how many rewards from this enemy have already been claimed

### Recommended Fields

* `Definition`
* `CurrentHp`
* `BattlesPlayed`
* `RewardsClaimed`

### Notes

* `EnemyProgressState` exists because enemy HP persists across battles.
* reward count is stored here because rewards are per enemy, not per battle.

---

## BattleState

### Purpose

`BattleState` represents one battle against the current enemy.

### Responsibilities

It tracks:

* which battle number this is for the current enemy
* which round is currently active
* current board state for both sides
* which player cards have already been used this battle
* the enemy card sequence for this battle
* battle-level flow state
* accumulated battle results and debug information

### Recommended Fields

* `BattleIndexForEnemy`
* `RoundIndex`
* `PlayerLane`
* `EnemyLane`
* `UsedPlayerCardIds`
* `EnemySequence`
* `RoundResults`
* `Logs`
* `Snapshots`
* `BattleFlowStage`

### Notes

* `BattleState` should not own player deck state.
* `BattleState` should not own run-level reward state.
* battle completion can be derived from flow and outcome conditions rather than requiring a dedicated boolean.

---

## LaneState

### Purpose

`LaneState` represents one side of the three-slot board.

### Responsibilities

It holds the slots for one side of the battle:

* player side
* enemy side

### Recommended Fields

* `Slots`

### Notes

* `LaneState` should be simple.
* the important logic lives in how `BoardSlotState` and `BoardCard` are used.

---

## BoardSlotState

### Purpose

`BoardSlotState` represents a specific board position.

### Responsibilities

It tracks:

* which slot it is
* whether this slot has been opened by the battle's current round progression
* which `BoardCard`, if any, currently occupies it

### Recommended Fields

* `Index`
* `IsOpen`
* `Occupant`

### Notes

* `IsOpen` must be explicit.
* `Occupant == null` is not the same as a closed slot.
* position is part of gameplay logic, not just presentation.

---

## CardInstance

### Purpose

`CardInstance` represents one real card in the player's deck at runtime.

### Responsibilities

It stores persistent card data that can change during the run:

* card identity
* RPS type
* base power
* current traits
* permanent power growth

### Recommended Fields

* `InstanceId`
* `RpsType`
* `BasePower`
* `Traits`
* `PermanentPowerBonus`

### Notes

* `CardInstance` is a runtime instance, not a static definition.
* two cards may share the same RPS type and base power but still be different instances.
* `Traits` should represent the current actual trait set of the card.
* `PermanentPowerBonus` exists so growth effects do not overwrite base power.

### What Does Not Belong Here

Do not store:

* current board position
* whether the card was used this battle
* current round combat power
* current round damage dealt

Those belong elsewhere.

---

## BoardCard

### Purpose

`BoardCard` represents the temporary on-board projection of a card during a battle.

### Responsibilities

It stores battle-local and round-local values for a card that has entered the board.

### Recommended Fields

* `SourceCard`
* `Side`
* `EnterRoundIndex`
* `FixedSelfPower`
* `CurrentPower`
* `DamageDealtThisRound`

### Notes

* `BoardCard` should reference the persistent `CardInstance`.
* `BoardCard` should not duplicate permanent card state unnecessarily.
* `CurrentPower` is recalculated each round.
* `DamageDealtThisRound` is a round-local value and must be reset each round.

### Important Distinction

`CardInstance` is the persistent deck object.
`BoardCard` is the battle-time projection of that card on the board.

This distinction is essential.

---

## RewardOffer

### Purpose

`RewardOffer` represents one reward selection event currently presented to the player.

### Responsibilities

It groups the options for a single reward choice into one explicit object.

### Recommended Fields

* `OfferId`
* `Options`
* `RewardIndexForCurrentEnemy`

### Notes

* this object is preferred over storing only a raw options list
* it provides a stable container for reward generation, display, and choice application

---

## RewardOption

### Purpose

`RewardOption` represents one directly executable reward choice.

### Responsibilities

It describes exactly one reward action.

### Recommended Fields

* `OptionId`
* `Type`
* payload data appropriate to the option type

### Option Types

Expected option types:

* `Upgrade`
* `Replace`
* `Skip`

### Notes

A reward option is not just a label.
It must contain enough data to be applied in one step.

---

## UpgradePayload

### Purpose

`UpgradePayload` stores the data needed to apply one upgrade reward.

### Recommended Fields

* `TargetCardInstanceId`
* `AddedTrait`

### Notes

Upgrade means modifying an existing card instance in place.

---

## ReplacePayload

### Purpose

`ReplacePayload` stores the data needed to apply one replace reward.

### Recommended Fields

* `TargetCardInstanceId`
* `NewCardSpec`

### Notes

Replace means removing one card from the deck and inserting a new card instance into the same deck position.

---

## NewCardSpec

### Purpose

`NewCardSpec` describes the generated specification for a new replacement card before it becomes a runtime `CardInstance`.

### Recommended Fields

* `RpsType`
* `BasePower`
* `Traits`

### Notes

* this is a generated specification, not yet a runtime deck instance
* in the current demo, replacement player cards use a fixed configured base power
* the architecture should still allow future expansion to multiple legal base powers

---

## RoundResult

### Purpose

`RoundResult` is the complete result summary of one resolved round.

### Responsibilities

It records:

* which round was resolved
* damage and healing totals
* HP before and after
* per-slot combat output
* logs
* phase snapshots

### Recommended Fields

* `RoundIndex`
* `PlayerCardInstanceId`
* `EnemyCardReference`
* `DamageToPlayer`
* `DamageToEnemy`
* `HealToPlayer`
* `HealToEnemy`
* `PlayerHpBefore`
* `PlayerHpAfter`
* `EnemyHpBefore`
* `EnemyHpAfter`
* `SlotResults`
* `Logs`
* `Snapshots`

### Notes

* `RoundResult` is a first-class output object, not an optional debug add-on
* it supports debugging, validation, presentation, and future replay-friendly tooling

---

## SlotCombatResult

### Purpose

`SlotCombatResult` describes what happened at one board slot during one round.

### Responsibilities

It records slot-level combat facts.

### Recommended Fields

* `SlotIndex`
* `PlayerCardInstanceId`
* `EnemyCardReference`
* `PlayerPower`
* `EnemyPower`
* `WinnerSide`
* `DamageToPlayer`
* `DamageToEnemy`

### Notes

* `RoundResult.SlotResults` should be a collection of `SlotCombatResult`
* this object exists to make slot-level debugging and presentation explicit

---

## PhaseSnapshot

### Purpose

`PhaseSnapshot` captures the board state after a specific resolution phase.

### Responsibilities

It stores a readable or structured snapshot of the current battle state at one point in the round pipeline.

### Recommended Fields

* `Phase`
* `PlayerLaneStateText` or equivalent structured representation
* `EnemyLaneStateText` or equivalent structured representation

### Notes

* snapshots are mainly for debugging and step-by-step presentation
* snapshots are recorded per phase, not only once at the end of the round
* snapshots are not authoritative game state and do not need to be part of future save data by default

---

## BattleOutcome

### Purpose

`BattleOutcome` is the summary passed from battle-level orchestration to run-level orchestration after a battle finishes.

### Responsibilities

It summarizes the battle result in a run-friendly form.

### Recommended Fields

* `BattleIndexForEnemy`
* `RoundsPlayed`
* `EnemyDefeated`
* `PlayerHpAfterBattle`
* `EnemyHpAfterBattle`

### Notes

* `BattleOutcome` helps keep `RunService` from depending on deep internal details of `BattleState`

---

## Persistent vs Temporary Data Ownership

This section is critical.

### Persistent Run Data

Belongs in:

* `RunState`
* `EnemyProgressState`
* `CardInstance`

Examples:

* player HP
* enemy HP across battles
* current deck composition
* permanent growth
* current enemy reward count

### Battle-Persistent Data

Belongs in:

* `BattleState`
* `LaneState`
* `BoardSlotState`
* `BoardCard`

Examples:

* current board positions
* opened slots
* used player cards this battle
* enemy card sequence for this battle

### Round-Temporary Data

Belongs in:

* `RoundResult`
* round-local context inside `RoundResolver`
* board-projected fields such as `CurrentPower` and `DamageDealtThisRound`

Examples:

* current round combat values
* damage totals this round
* derived deltas
* per-phase debug information

---

## Equality and Identity Considerations

The domain model distinguishes between:

* object identity
* gameplay equivalence

### Card identity

`CardInstance.InstanceId` identifies a specific runtime card.

### Gameplay equivalence

Some systems, especially reward deduplication, must ignore:

* card instance identity
* deck order
* trait order

This is why reward deduplication uses a canonical resulting deck signature rather than raw action identity.

This canonical logic belongs to the domain model.

---

## Important Modeling Rules

The following rules are considered part of the domain model design:

1. `CardInstance` and `BoardCard` must remain separate concepts
2. `RunState` is the root authoritative runtime state
3. `BattleState` is battle-scoped, not run-scoped
4. `RewardOffer` is preferred over a raw pending option list
5. `RoundResult` is a required result object, not optional debug metadata
6. slot-level combat should be represented explicitly through `SlotCombatResult`
7. phase-level board inspection should be supported through `PhaseSnapshot`

---

## What the Domain Model Intentionally Avoids

The domain model intentionally avoids:

* embedding UI state
* embedding MonoBehaviour lifecycle concerns
* using services as hidden state containers
* conflating persistent and temporary values
* relying on implicit state transitions hidden inside unrelated objects

---

## Future Extensions

The current model is designed to remain compatible with future additions such as:

* more enemy types
* more traits
* richer reward generation profiles
* save/load support
* more advanced presentation
* replay or battle inspection tools

These extensions should build on the current object boundaries.

---

## Summary

The domain model is built around explicit ownership of state.

Key ideas:

* `RunState` owns the run
* `EnemyProgressState` owns current enemy progression
* `BattleState` owns battle progression
* `CardInstance` owns persistent deck card state
* `BoardCard` owns temporary on-board battle values
* `RewardOffer` owns one reward selection event
* `RoundResult` owns one round's explicit output

This structure is intended to keep gameplay logic understandable, testable, and easy to inspect during early development.