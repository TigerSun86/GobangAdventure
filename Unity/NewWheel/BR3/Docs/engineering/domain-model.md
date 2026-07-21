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
* `config-and-content.md`

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
├── PlayerMaxHp
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
* `BattleOutcome`
* `BattleCommandResult`
* `RunCommandResult`

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
* `PlayerMaxHp`
* `PlayerDeck`
* `CurrentEnemyIndex`
* `CurrentEnemy`
* `ActiveBattle`
* `PendingRewardOffer`
* `FlowStage`

### Notes

* `RunState` should not contain round-local combat values.
* `RunState` should not contain UI-only state.
* `RunState` should carry `PlayerMaxHp` as runtime state.
* runtime systems should not repeatedly query authored config to determine max HP.
* run termination can be derived from `FlowStage`, so separate `IsVictory` or `IsRunOver` fields are not required.

---

## EnemyProgressState

### Purpose

`EnemyProgressState` represents the progression state of the current enemy across multiple battles against that enemy, bounded by the enemy's configured battle limit.

### Responsibilities

It tracks:

* which enemy is currently being fought
* current enemy HP
* current enemy max HP
* how many battles have been played against this enemy
* how many rewards from this enemy have already been claimed

### Recommended Fields

* `Config`
* `CurrentHp`
* `MaxHp`
* `BattlesPlayed`
* `RewardsClaimed`

### Notes

* `EnemyProgressState` exists because enemy HP persists across battles.
* `MaxHp` is part of runtime state, not something that should be looked up repeatedly from config during battle flow.
* `BattlesPlayed` must be interpreted against `Config.battleLimit`, not against a hard-coded constant.
* `RewardsClaimed` tracks progress toward the reward total derived from `Config.battleLimit`.
* the current demo should not introduce a second independent authored or runtime reward-total source of truth for the enemy when that total can be derived from `Config.battleLimit`.
* `Config` refers to the static authored enemy config and should not be mutated at runtime.

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
* any completed battle outcome that has already been fixed but is still waiting for round-result presentation to finish

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
* `PendingBattleOutcome`

### PendingBattleOutcome

`PendingBattleOutcome` is nullable.

It is used when:

* the latest round has already been fully resolved and applied
* final player and enemy HP have already been established
* the battle completion reason has already been classified
* the battle is still in `PresentingRoundResult`

This field allows gameplay outcome classification to be completed before presentation finishes without allowing Presentation to decide or alter the result.

Expected behavior:

* it is `null` while the battle can continue normally
* it becomes non-null when the resolved round completes the battle
* it remains stable during `PresentingRoundResult`
* it remains available after the battle moves to `BattleComplete`
* it is consumed only when `RunService.AcceptCompletedBattle(...)` accepts the completed battle and clears the active battle
* it must not be recalculated by Presentation code

### Notes

* `BattleState` should not own player deck state.
* `BattleState` should not own run-level reward state.
* `PendingBattleOutcome` is battle-scoped state, not run progression state.
* a dedicated mutable `IsBattleComplete` field is not required.
* battle completion readiness is represented through battle flow stage and the presence of a pending or completed outcome.
* `FinishRoundPresentation(...)` may return the pending outcome through `BattleCommandResult`, but it must not clear the authoritative copy stored in `BattleState`.

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

## CardSpec

### Purpose

`CardSpec` is the static card specification format used by authored config and generated replacement specs.

It describes what a card is before it becomes a runtime `CardInstance`.

### Recommended Fields

* `RpsType`
* `BasePower`
* `Traits`

### Notes

* `CardSpec` is configuration data, not runtime state.
* the same `CardSpec` shape is used for:

  * player starting deck entries
  * enemy fixed deck entries
  * generated replacement card specs
* `CardSpec` does not carry runtime identity or mutable run progression data.

### What Does Not Belong Here

Do not store:

* instance identity
* current HP
* permanent growth accumulated during a run
* board position
* battle usage state
* current round combat values

Those belong to runtime objects.

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

* `CardInstance` is created from `CardSpec`.
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

`CardSpec` is static config.
`CardInstance` is the persistent runtime deck object.
`BoardCard` is the battle-time projection of that runtime card on the board.

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
* `ReplacementCardSpec`

### Notes

Replace means removing one card from the deck and inserting a new card instance into the same deck position.

The replacement card specification uses `CardSpec`.

---

## RoundResult

### Purpose

`RoundResult` is the complete result summary of one fully resolved and applied round.

It contains both:

* raw round consequences produced by `RoundResolver`
* authoritative HP application values finalized by `BattleService`

### Responsibilities

It records:

* which round was resolved
* raw merged damage totals
* raw post-resolve healing totals
* HP before round consequence application
* HP immediately after merged damage application
* actual healing applied after max-HP clamp
* final HP after all round effects and clamp
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
* `PlayerHpAfterMergedDamage`
* `PlayerHealingApplied`
* `PlayerHpAfter`
* `EnemyHpBefore`
* `EnemyHpAfterMergedDamage`
* `EnemyHealingApplied`
* `EnemyHpAfter`
* `SlotResults`
* `Logs`
* `Snapshots`

### Raw Consequence Fields

The following values are produced from round-rule resolution:

* `DamageToPlayer`
* `DamageToEnemy`
* `HealToPlayer`
* `HealToEnemy`
* `SlotResults`
* `Logs`
* `Snapshots`

`HealToPlayer` and `HealToEnemy` represent raw healing produced by round rules before max-HP clamp.

For example:

```
Player HP before healing: 29
Player max HP: 30
Raw healing: 4
Actual healing applied: 1
Final player HP: 30
```

In that case:

```
HealToPlayer = 4
PlayerHealingApplied = 1
PlayerHpAfter = 30
```

### Authoritative Application Fields

The following values are finalized by `BattleService` while applying the round consequences:

* `PlayerHpBefore`
* `PlayerHpAfterMergedDamage`
* `PlayerHealingApplied`
* `PlayerHpAfter`
* `EnemyHpBefore`
* `EnemyHpAfterMergedDamage`
* `EnemyHealingApplied`
* `EnemyHpAfter`

`PlayerHpAfterMergedDamage` and `EnemyHpAfterMergedDamage` may be zero or negative.

That intermediate state does not by itself mean that player death or enemy defeat has already been classified.

Only the final values:

* `PlayerHpAfter`
* `EnemyHpAfter`

are used for round-end survival and battle-completion classification.

### Temporary Zero or Below

A temporary-zero recovery can be derived from the existing fields.

For example:

```
PlayerHpAfterMergedDamage <= 0
PlayerHpAfter > 0
```

This means the player temporarily reached zero or below after merged damage but survived because post-resolve healing restored final HP above zero.

A separate mutable field such as `WasTemporarilyDefeated` is not required.

### Notes

* `RoundResult` is a first-class output object, not optional debug metadata.
* `RoundResolver` does not authoritatively mutate player or enemy HP.
* `BattleService` owns authoritative HP mutation and max-HP clamp.
* clamp logic must not be independently duplicated inside both `RoundResolver` and `BattleService`.
* the final `PlayerHpAfter` and `EnemyHpAfter` values must match authoritative runtime state.
* the seven accepted round phases remain unchanged.
* player death, enemy defeat, and battle completion classification occur after the final HP values have been established.
* `RoundResult` supports debugging, validation, presentation, and future replay-friendly tooling.
* `RoundResult` may be created by `RoundResolver`, but it is not considered finalized for battle history or presentation until `BattleService` has populated all authoritative HP application fields.

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

## BattleCompletionReason

### Purpose

`BattleCompletionReason` identifies the single official gameplay reason why one battle completed.

### Values

* `PlayerDefeated`
* `EnemyDefeated`
* `AllRoundsCompleted`

### Meaning

#### `PlayerDefeated`

The player's final HP after complete round application is zero or below.

This reason takes priority even if the enemy's final HP is also zero or below.

#### `EnemyDefeated`

The player's final HP is above zero and the enemy's final HP is zero or below.

#### `AllRoundsCompleted`

Both the player and enemy remain above zero HP after the final round of the battle.

### Important Distinction

`AllRoundsCompleted` describes completion of the current three-round battle.

It does not mean that the current enemy's configured multi-battle limit has been exhausted.

Enemy battle-limit exhaustion is interpreted later by `RunService` using:

* `CurrentEnemy.BattlesPlayed`
* `CurrentEnemy.Config.battleLimit`

### Invalid Representation to Avoid

The model should not represent official battle outcome using two independently mutable values such as:

```
PlayerDefeated = true
EnemyDefeated = true
```

The final HP fields may show that both sides reached zero or below, but the official completion reason must remain unique.

---

## BattleOutcome

### Purpose

`BattleOutcome` is the immutable gameplay summary passed from battle-level orchestration to run-level orchestration after a battle finishes.

### Responsibilities

It summarizes the completed battle in a run-friendly and unambiguous form.

### Recommended Fields

* `BattleIndexForEnemy`
* `RoundsPlayed`
* `CompletionReason`
* `PlayerHpAfterBattle`
* `EnemyHpAfterBattle`

### Recommended Derived Properties

The implementation may expose convenience properties such as:

```
PlayerDefeated =
    CompletionReason == BattleCompletionReason.PlayerDefeated

EnemyDefeated =
    CompletionReason == BattleCompletionReason.EnemyDefeated
```

These should be derived read-only properties rather than independently assigned state.

### Outcome Invariants

#### PlayerDefeated

If:

```
CompletionReason == BattleCompletionReason.PlayerDefeated
```

then:

```
PlayerHpAfterBattle <= 0
```

`EnemyHpAfterBattle` may be either:

* above zero
* zero
* below zero

This allows simultaneous zero to preserve the raw final HP facts while still producing one official gameplay result.

#### EnemyDefeated

If:

```
CompletionReason == EnemyDefeated
```

then:

```
PlayerHpAfterBattle > 0
EnemyHpAfterBattle <= 0
```

#### AllRoundsCompleted

If:

```
CompletionReason == AllRoundsCompleted
```

then:

```
PlayerHpAfterBattle > 0
EnemyHpAfterBattle > 0
```

and the final round of the current battle has completed.

### Simultaneous Zero

When both final HP values are zero or below:

```
PlayerHpAfterBattle <= 0
EnemyHpAfterBattle <= 0
```

the official result is:

```
CompletionReason = PlayerDefeated
```

The enemy must not also be reported as officially defeated.

### Notes

* `BattleOutcome` keeps `RunService` from depending on deep internal details of `BattleState`.
* it represents gameplay facts, not command success or failure.
* it should be fixed after authoritative final HP application.
* it must not be reclassified by Presentation.
* it does not decide reward generation, next enemy progression, victory, or run defeat by itself.
* `RunService` interprets the outcome and performs run-level progression.

---

## BattleCommandResult

### Purpose

`BattleCommandResult` is the application-level result object returned by battle service commands.

### Responsibilities

It reports:

* whether the command succeeded
* the current battle flow stage
* any produced round result
* whether the battle is complete
* any produced battle outcome

### Recommended Fields

* `Success`
* `FailureReason`
* `BattleFlowStage`
* `RoundResult`
* `IsBattleComplete`
* `BattleOutcome`

### Notes

* this is an application command result, not a pure gameplay outcome
* it is distinct from `RoundResult` and `BattleOutcome`
* `IsBattleComplete` means the battle has reached the `BattleComplete` handoff state.
* a completed gameplay outcome may already exist in `BattleState.PendingBattleOutcome` while the battle is still in `PresentingRoundResult`.
* `SubmitPlayerCard(...)` may therefore return a resolved `RoundResult` while the fixed outcome remains pending presentation completion.
* `FinishRoundPresentation(...)` may return the already-fixed `BattleOutcome` when moving the battle to `BattleComplete`.
* `FinishRoundPresentation(...)` must not recompute or reclassify the outcome.
* `FailureReason` represents command failure and must not be used to encode gameplay defeat.

---

## RunCommandResult

### Purpose

`RunCommandResult` is the application-level result object returned by run service commands.

### Responsibilities

It reports:

* whether the command succeeded
* the current run flow stage
* whether an active battle now exists
* whether a pending reward now exists
* whether the run is complete

### Recommended Fields

* `Success`
* `FailureReason`
* `FlowStage`
* `ActiveBattle`
* `PendingRewardOffer`
* `IsRunComplete`

### Notes

* this is an application command result, not a pure gameplay outcome
* it exists to make run-level orchestration explicit and inspectable

---

## Persistent vs Temporary Data Ownership

This section is critical.

### Persistent Run Data

Belongs in:

* `RunState`
* `EnemyProgressState`
* `CardInstance`

Examples:

* player current HP
* player max HP
* enemy current HP
* enemy max HP
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
* a fixed pending battle outcome waiting for presentation completion

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

## HP Application, Healing, and Final HP

The domain model assumes:

* current HP is authoritative runtime state
* max HP is authoritative runtime state
* healing cannot raise current HP above max HP
* merged damage may temporarily reduce current HP to zero or below
* temporary zero or below does not interrupt the accepted seven-phase round pipeline
* only final HP after complete application is used for player death and enemy defeat classification

These rules apply symmetrically to:

* player HP
* enemy HP

### Responsibility Boundary

`RoundResolver` computes:

* raw merged damage totals
* raw post-resolve healing totals
* other round-rule consequences
* logs
* snapshots
* slot combat results

`BattleService` owns:

* reading HP before application
* applying merged damage to authoritative runtime HP
* recording HP after merged damage
* applying post-resolve healing
* enforcing max-HP clamp
* recording actual healing applied
* establishing final authoritative HP
* classifying whether the battle has completed

### Application Order

The conceptual application order is:

```
HP before
→ apply merged damage
→ record HP after merged damage
→ apply post-resolve healing
→ clamp to max HP
→ record actual healing applied
→ establish final HP
→ classify player defeat, enemy defeat, or battle continuation
```

### Final HP Rule

Only:

* `PlayerHpAfter`
* `EnemyHpAfter`

are used for official round-end survival and battle completion.

HP after merged damage is an intermediate diagnostic and application value.

It must not cause:

* Post Resolve to be skipped
* premature player death
* premature enemy defeat
* premature battle completion

### No Duplicate Clamp Logic

`RoundResolver` and `BattleService` must not each maintain their own independent max-HP clamp calculation.

The authoritative clamp occurs once during battle-layer application.

Any final HP fields stored in `RoundResult` must match the authoritative runtime state after that application.

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

1. `CardSpec`, `CardInstance`, and `BoardCard` must remain separate concepts
2. `RunState` is the root authoritative runtime state
3. `BattleState` is battle-scoped, not run-scoped
4. `RewardOffer` is preferred over a raw pending option list
5. `RoundResult` is a required result object, not optional debug metadata
6. slot-level combat should be represented explicitly through `SlotCombatResult`
7. phase-level board inspection should be supported through `PhaseSnapshot`
8. max HP belongs in runtime state, not only in authored config
9. authoritative HP mutation and healing clamp belong to battle-layer application
10. temporary HP at zero or below does not interrupt the seven-phase round pipeline
11. battle completion uses one explicit `BattleCompletionReason`
12. simultaneous zero preserves both final HP facts but resolves officially as `PlayerDefeated`
13. Presentation must not compute or alter gameplay outcome

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
* future mechanics that alter max HP

These extensions should build on the current object boundaries.

---

## Summary

The domain model is built around explicit ownership of state.

Key ideas:

* `RunState` owns the run
* `EnemyProgressState` owns current enemy progression
* `BattleState` owns battle progression
* `CardSpec` owns static authored card shape
* `CardInstance` owns persistent runtime deck card state
* `BoardCard` owns temporary on-board battle values
* `RewardOffer` owns one reward selection event
* `RoundResult` owns one round's explicit output
* `BattleOutcome` owns one completed battle summary with one unambiguous completion reason
* `BattleCommandResult` and `RunCommandResult` own application command outcomes
* `BattleState.PendingBattleOutcome` preserves a fixed outcome while round-result presentation is still in progress

This structure is intended to keep gameplay logic understandable, testable, and easy to inspect during early development.