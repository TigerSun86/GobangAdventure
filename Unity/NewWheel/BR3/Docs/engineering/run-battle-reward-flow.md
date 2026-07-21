# Run, Battle, and Reward Flow

## Purpose

This document defines the high-level runtime flow across the three main orchestration components:

* `RunService`
* `BattleService`
* `RewardService`

It explains:

* how a run starts
* how an enemy is progressed
* how a battle starts and ends
* how battle outcomes are interpreted
* when rewards are generated
* how rewards are consumed
* how the system moves to the next battle, next enemy, victory, or defeat
* how run-level and battle-level flow stages are used
* how application command result objects are used

This document focuses on flow orchestration, not low-level round rules. Low-level round behavior is defined in `round-resolution.md`.

---

## Architectural Intent

The system separates flow responsibilities into three layers of orchestration:

### RunService

Responsible for run-level progression.

### BattleService

Responsible for battle-level progression.

### RewardService

Responsible for reward generation and reward application.

This split is intentional.

The key rule is:

* `RoundResolver` resolves one round
* `BattleService` orchestrates one battle
* `RunService` orchestrates the full run
* `RewardService` generates and applies rewards

---

## Why This Separation Exists

The gameplay naturally operates at three different scopes:

### Round scope

One round of simultaneous play and resolution.

### Battle scope

One three-round battle against the current enemy.

### Run scope

The full progression across enemies, battles, rewards, victory, and defeat.

Trying to manage all three scopes in one component would create unclear boundaries and harder debugging.

---

## Main Runtime Objects Involved

The flow uses these runtime objects:

* `RunState`
* `EnemyProgressState`
* `BattleState`
* `RewardOffer`
* `BattleOutcome`
* `RoundResult`
* `RunCommandResult`
* `BattleCommandResult`

### Ownership Summary

* `RunState` owns run-level state
* `EnemyProgressState` owns current enemy progression
* `BattleState` owns one active battle
* `RewardOffer` owns one pending reward selection
* `BattleOutcome` summarizes one completed battle
* `RoundResult` summarizes one resolved round
* `RunCommandResult` summarizes one run-level command execution
* `BattleCommandResult` summarizes one battle-level command execution

---

## Run-Level Flow Stages

The run-level flow is intentionally small and explicit.

The accepted run flow stages are:

* `ReadyForNextBattle`
* `InBattle`
* `ChoosingReward`
* `Victory`
* `Defeat`

### Stage meanings

#### `ReadyForNextBattle`

The run is active and can begin the next battle.

Expected conditions:

* there is a current enemy
* there is no active battle
* there is no pending reward offer

#### `InBattle`

A battle is currently active.

Expected conditions:

* `RunState.ActiveBattle != null`
* the active battle may be in any non-consumed battle flow stage from `WaitingForPlayerCard` through `BattleComplete`

The run remains in `InBattle` while the final round result is being presented and while a completed battle is waiting for `RunService.AcceptCompletedBattle(...)`.

The run-level stage does not change merely because a battle outcome has already been fixed.

#### `ChoosingReward`

A reward offer is currently pending and must be resolved before the run can continue.

Expected conditions:

* `RunState.ActiveBattle == null`
* `RunState.PendingRewardOffer != null`

#### `Victory`

The run has ended in success.

Expected conditions:

* no active battle
* no pending reward offer

#### `Defeat`

The run has ended in failure.

Expected conditions:

* no active battle
* no pending reward offer

### Important Rule

At run level:

* an active battle and a pending reward must not coexist
* victory and defeat must be terminal states
* the run stage must always match the actual runtime state

---

## Battle-Level Flow Stages

A battle has its own internal flow.

The accepted battle flow stages are:

* `WaitingForPlayerCard`
* `ResolvingRound`
* `PresentingRoundResult`
* `BattleComplete`

### Important Distinction

Do not confuse:

* battle flow stage

with

* round resolution phase

Examples:

* `WaitingForPlayerCard` is a battle flow state
* `Movement` is a round resolution phase

They exist at different levels.

### Stage meanings

#### `WaitingForPlayerCard`

The battle is waiting for player input.

#### `ResolvingRound`

A valid player action has been received and the round is currently being resolved and applied.

#### `PresentingRoundResult`

The round has been resolved and applied, and presentation is showing the result.

#### `BattleComplete`

The battle has ended and is ready to hand control back to run-level flow.

---

## RunService

## Responsibility

`RunService` is the high-level orchestrator for the entire run.

It is responsible for:

* creating a new run
* preparing the current enemy
* determining when a battle may begin
* accepting and interpreting completed battle outcomes
* deciding when rewards should begin
* deciding when the next battle should begin
* deciding when to move to the next enemy
* deciding victory or defeat
* maintaining run-level flow invariants

It is not responsible for:

* low-level round rule calculation
* battle-internal round progression
* reward legality and candidate generation internals

---

## BattleService

## Responsibility

`BattleService` is the orchestrator for one battle.

It is responsible for:

* starting a battle
* preparing enemy sequence data
* validating player card selection
* invoking `RoundResolver`
* applying raw round consequences to authoritative runtime HP
* enforcing max-HP clamp during HP application
* finalizing authoritative HP application fields in `RoundResult`
* classifying whether the resolved round completes the battle
* storing any fixed completed outcome in `BattleState.PendingBattleOutcome`
* advancing battle-level flow
* producing the completed `BattleOutcome` handoff

It is not responsible for:

* deciding next enemy progression
* generating reward offers
* deciding whether the run enters `Victory` or `Defeat`
* deciding any run-level reward progression

### Important distinction

`BattleService` may classify a battle completion reason such as:

* `PlayerDefeated`
* `EnemyDefeated`
* `AllRoundsCompleted`

This is a battle-level gameplay fact.

`RunService` decides what that completed battle means for the full run.

---

## RewardService

## Responsibility

`RewardService` is responsible for:

* generating reward offers
* validating legal reward candidates
* deduplicating reward options
* applying the chosen reward to the player deck

It is not responsible for:

* deciding when reward selection happens
* deciding whether the next step is next battle, next enemy, victory, or defeat

---

## Runtime Construction and New Run Creation

Run creation is intentionally split between:

* `RuntimeStateFactory`
* `RunService`

### RuntimeStateFactory responsibility

`RuntimeStateFactory` constructs the initial runtime object graph from authored config.

Examples:

* create `RunState` from `GameConfig`
* create `EnemyProgressState` from `EnemyConfig`
* create `CardInstance` from `CardSpec`

### RunService responsibility

`RunService.CreateNewRun(...)` is the run-level entry point for starting a new run.

It delegates initial runtime construction to `RuntimeStateFactory`.

This is a deliberate separation:

* factory constructs
* service orchestrates

---

## Start of Run

A run begins when `RunService.CreateNewRun(...)` is called.

### Inputs

* `GameConfig`
* `RuntimeStateFactory`

### Behavior

`RunService.CreateNewRun(...)` should:

1. delegate construction to `RuntimeStateFactory`
2. receive a fully initialized `RunState`
3. return that `RunState` to the caller

### Expected initial state

The returned `RunState` should have:

* `PlayerHp` initialized from `playerMaxHp`
* `PlayerMaxHp` initialized from `playerMaxHp`
* a fully created player deck
* `CurrentEnemyIndex = 0`
* the first `EnemyProgressState` already created
* `ActiveBattle = null`
* `PendingRewardOffer = null`
* `FlowStage = ReadyForNextBattle`

---

## Current Enemy Lifecycle

Each enemy is represented at runtime by `EnemyProgressState`.

It tracks:

* the current enemy config reference
* current HP
* max HP
* number of battles played against this enemy
* number of rewards already claimed from this enemy

This object exists because enemy HP persists across multiple battles.

It is created when entering an enemy and discarded when the system moves to the next enemy or the run ends.

### Important interpretation rule

`BattlesPlayed` must be interpreted against `CurrentEnemy.Config.battleLimit`, not against a hard-coded constant.

Reward total for the current enemy should likewise be derived from that same configured battle limit.

`RewardsClaimed` therefore tracks progress toward the enemy's configured reward total rather than toward a separately authored fixed reward-count field.

---

## RunService Public Commands

The current recommended run-level command surface is:

* `CreateNewRun(...)`
* `CanStartNextBattle(...)`
* `StartNextBattle(...)`
* `AcceptCompletedBattle(...)`
* `ChooseReward(...)`

Internal helper methods may exist, but they do not need to be part of the public command surface.

---

## CreateNewRun(...)

### Purpose

Create a new `RunState`.

### Responsibility

This is the run-level entry point for starting a run.

### Important design rule

It should delegate initial runtime graph construction to `RuntimeStateFactory`.

### Typical result

* returns a new `RunState`

This method does not need `RunCommandResult` because it is a root creation entry point rather than a command against an existing run.

---

## CanStartNextBattle(...)

### Purpose

Determine whether the current run is allowed to begin a new battle.

### Why it exists

Although the caller could inspect raw state, the run service should remain the source of truth for run-level gating conditions.

### Typical logic

A next battle may start only if:

* the run is not in `Victory`
* the run is not in `Defeat`
* `FlowStage == ReadyForNextBattle`
* `ActiveBattle == null`
* `PendingRewardOffer == null`

A simple boolean result is acceptable for the current demo.

---

## StartNextBattle(...)

### Purpose

Start the next battle for the current enemy.

### Inputs

* `RunState`
* `BattleService`

### Behavior

This method should:

1. validate that the run may start a battle
2. call `BattleService.StartBattle(...)`
3. assign the returned `BattleState` to `RunState.ActiveBattle`
4. set `RunState.FlowStage = InBattle`

### Output

Returns `RunCommandResult`.

### Expected result fields

On success:

* `Success = true`
* `FlowStage = InBattle`
* `ActiveBattle != null`

On failure:

* `Success = false`
* `FailureReason` should explain why battle start was rejected

---

## AcceptCompletedBattle(...)

### Purpose

Accept the authoritative completed battle outcome and perform run-level progression.

### Inputs

* `RunState`
* `BattleOutcome`
* `RewardService`

### Validation

Before changing progression, `RunService` should validate:

* `RunState.FlowStage == InBattle`
* `RunState.ActiveBattle != null`
* `RunState.ActiveBattle.BattleFlowStage == BattleComplete`
* `RunState.ActiveBattle.PendingBattleOutcome != null`
* `RunState.PendingRewardOffer == null`
* the supplied `BattleOutcome` matches the authoritative pending outcome stored in the active battle

The implementation may instead read the outcome directly from the active battle, but there must not be two conflicting outcome sources.

If validation fails:

* no progression counter should change
* the active battle should not be cleared
* a failed `RunCommandResult` should be returned

### Shared acceptance steps

After successful validation:

1. capture the authoritative pending outcome
2. increment `CurrentEnemy.BattlesPlayed` exactly once
3. clear `RunState.ActiveBattle`
4. interpret the captured outcome using the branch order below

The completed fatal battle still counts as a played battle.

This historical increment must not be used to grant a reward after player death.

### Branch order

The run-level branch order is:

1. `PlayerDefeated`
2. `EnemyDefeated`
3. `AllRoundsCompleted` with battle limit exhausted
4. `AllRoundsCompleted` with more battles remaining

### Branch 1: PlayerDefeated

If:

```
CompletionReason ==
    BattleCompletionReason.PlayerDefeated
```

then:

* clear `RunState.PendingRewardOffer`
* set `RunState.FlowStage = Defeat`
* return immediately

Do not:

* generate a reward offer
* increment `RewardsClaimed`
* settle remaining enemy rewards
* move to the next enemy
* enter `Victory`

This branch also handles simultaneous zero.

### Branch 2: EnemyDefeated

If:

```
CompletionReason ==
    BattleCompletionReason.EnemyDefeated
```

then the player is known to be alive.

If the current enemy is the final enemy:

* ignore unresolved final-enemy reward opportunities
* keep `PendingRewardOffer = null`
* set `FlowStage = Victory`

If more enemies remain:

* enter reward flow
* begin settling the completed battle's reward opportunity
* continue settling remaining rewards for this defeated enemy through the normal reward loop

### Branch 3: AllRoundsCompleted and battle limit exhausted

If:

```
CompletionReason ==
    BattleCompletionReason.AllRoundsCompleted
```

and:

```
CurrentEnemy.BattlesPlayed >=
    CurrentEnemy.Config.battleLimit
```

then:

* clear `RunState.PendingRewardOffer`
* set `FlowStage = Defeat`

No reward is generated for the battle that exhausted the allowed battle limit without defeating the enemy.

### Branch 4: AllRoundsCompleted and more battles remain

If:

```
CompletionReason ==
    BattleCompletionReason.AllRoundsCompleted
```

and:

```
CurrentEnemy.BattlesPlayed <
    CurrentEnemy.Config.battleLimit
```

then:

* enter reward flow for the completed battle
* after that reward is resolved, the run becomes ready for the next battle against the same enemy

### Important rules

`RunService` must interpret `CompletionReason`.

It must not reconstruct official battle outcome by checking enemy HP first.

`RewardsClaimed` is incremented only when a reward choice is actually resolved through `ChooseReward(...)`.

---

## ChooseReward(...)

### Purpose

Accept the player's chosen reward option and advance run-level progression afterward.

### Inputs

* `RunState`
* `optionId`
* `RewardService`

### Behavior

This method should:

1. validate that reward selection is currently allowed
2. apply the selected reward through `RewardService`
3. increment `CurrentEnemy.RewardsClaimed`
4. clear the current `PendingRewardOffer`
5. decide what comes next:

   * another reward immediately
   * `ReadyForNextBattle`
   * next enemy
   * `Victory`

### Output

Returns `RunCommandResult`.

---

## BattleService Public Commands

The current recommended battle-level command surface is:

* `StartBattle(...)`
* `SubmitPlayerCard(...)`
* `FinishRoundPresentation(...)`

Internal helper methods may exist, but they do not need to be part of the public command surface.

---

## StartBattle(...)

### Purpose

Create a new battle for the current enemy.

### Inputs

* `RunState`
* `EnemyProgressState`
* enemy config or enemy config reference
* battle RNG or enemy sequence source if needed

### Behavior

This method should:

1. create a new `BattleState`
2. initialize lanes
3. initialize round index
4. initialize used player card tracking
5. generate enemy sequence for this battle
6. initialize `PendingBattleOutcome = null`
7. set battle flow stage to `WaitingForPlayerCard`

### Output

Returns `BattleState`

### Important Rule

`BattleService.StartBattle(...)` constructs battle state but does not own run-level flow transitions.
`RunService.StartNextBattle(...)` is responsible for attaching the new battle to `RunState`.

---

## SubmitPlayerCard(...)

### Purpose

Submit the player's selected card, fully resolve and apply the current round, and fix any battle-completion outcome produced by that round.

### Inputs

* `RunState`
* `EnemyProgressState`
* `BattleState`
* `cardInstanceId`
* `RoundResolver`

### Validation

Before any mutation, `BattleService` should validate:

* the run has the supplied active battle
* `BattleFlowStage == WaitingForPlayerCard`
* `PendingBattleOutcome == null`
* the selected card exists in the player deck
* the selected card has not already been used in this battle
* the current round index is valid
* the prepared enemy sequence contains a card for the current round

If validation fails:

* no battle state should advance
* no HP should change
* no card should become used
* a failed `BattleCommandResult` should be returned

### Behavior

After successful validation, the method should:

1. set `BattleFlowStage = ResolvingRound`
2. select the enemy card for the current round
3. call `RoundResolver.ResolveRound(...)`
4. allow `RoundResolver` to complete all seven accepted round phases
5. read authoritative player and enemy HP before consequence application
6. apply merged damage to authoritative runtime HP
7. record HP immediately after merged damage
8. apply raw post-resolve healing
9. enforce max-HP clamp
10. record actual healing applied
11. establish final authoritative player and enemy HP
12. populate all authoritative HP application fields in `RoundResult`
13. append the finalized round result, logs, and snapshots to battle history
14. classify whether the resolved round completes the battle
15. create and store `PendingBattleOutcome` when the battle is complete
16. set `BattleFlowStage = PresentingRoundResult`
17. return a successful `BattleCommandResult`

### Battle-completion classification

Classification must use final HP only, in this exact priority:

```
if PlayerHpAfter <= 0
    CompletionReason = BattleCompletionReason.PlayerDefeated
else if EnemyHpAfter <= 0
    CompletionReason = BattleCompletionReason.EnemyDefeated
else if the third round has just been resolved
    CompletionReason = BattleCompletionReason.AllRoundsCompleted
else
    the battle continues after presentation
```

### Simultaneous zero

If:

```
PlayerHpAfter <= 0
EnemyHpAfter <= 0
```

then:

```
CompletionReason =
    BattleCompletionReason.PlayerDefeated
```

The enemy is not officially reported as defeated.

### Pending outcome behavior

When classification produces a completion reason, `BattleService` should create one immutable `BattleOutcome` and store it in:

* `BattleState.PendingBattleOutcome`

That outcome must contain:

* actual battle index
* actual rounds played
* the unique completion reason
* final player HP
* final enemy HP

When the battle can continue:

* `PendingBattleOutcome` remains `null`

### Command result behavior

`SubmitPlayerCard(...)` returns the finalized `RoundResult`.

At this point:

* `BattleFlowStage = PresentingRoundResult`
* `IsBattleComplete = false`, because the presentation handoff has not finished
* the authoritative fixed outcome, when one exists, is stored in `BattleState.PendingBattleOutcome`

`SubmitPlayerCard(...)` must not directly call `RunService`.

### Important rule

Gameplay outcome classification is completed during battle-layer application.

It is not delayed until the player presses Continue, and it is not performed by Presentation code.

---

## HP Application Rule

`BattleService` is the authoritative owner of runtime HP mutation.

For each side, application follows this order:

```
HP before
→ subtract merged damage
→ record HP after merged damage
→ add raw post-resolve healing
→ clamp to max HP
→ record actual healing applied
→ establish final HP
```

This applies to:

* `RunState.PlayerHp` against `RunState.PlayerMaxHp`
* `EnemyProgressState.CurrentHp` against `EnemyProgressState.MaxHp`

### RoundResult field mapping

`RoundResolver` produces:

* `DamageToPlayer`
* `DamageToEnemy`
* `HealToPlayer`
* `HealToEnemy`

`BattleService` finalizes:

* `PlayerHpBefore`
* `PlayerHpAfterMergedDamage`
* `PlayerHealingApplied`
* `PlayerHpAfter`
* `EnemyHpBefore`
* `EnemyHpAfterMergedDamage`
* `EnemyHealingApplied`
* `EnemyHpAfter`

### Temporary zero or below

HP after merged damage may be zero or negative.

That intermediate value must not cause:

* Post Resolve to be skipped
* premature player death
* premature enemy defeat
* premature battle completion

Only final HP after healing and clamp participates in completion classification.

### No duplicate HP calculation

`RoundResolver` and `BattleService` must not independently calculate two competing authoritative final-HP results.

The authoritative HP mutation and clamp occur once in `BattleService`.

---

## FinishRoundPresentation(...)

### Purpose

Finish presentation of the already-resolved round and advance battle flow without changing gameplay results.

### Recommended inputs

* `BattleState`

`EnemyProgressState` is no longer required to decide whether the battle completed.

If an existing implementation temporarily retains that parameter, this method must not use it to re-read HP or reclassify the outcome.

### Validation

The method should validate:

* `BattleFlowStage == PresentingRoundResult`

If validation fails:

* no state should change
* a failed `BattleCommandResult` should be returned

### If a pending battle outcome exists

If:

* `BattleState.PendingBattleOutcome != null`

then:

1. set `BattleFlowStage = BattleComplete`
2. return the same stored `BattleOutcome`
3. set `IsBattleComplete = true`

The stored authoritative copy must remain in `BattleState.PendingBattleOutcome`.

It is not cleared here.

### If no pending battle outcome exists

If:

* `BattleState.PendingBattleOutcome == null`

then:

1. increment `RoundIndex`
2. set `BattleFlowStage = WaitingForPlayerCard`
3. return a successful result with `IsBattleComplete = false`

### Prohibited behavior

`FinishRoundPresentation(...)` must not:

* read current HP to decide who won
* recreate `BattleOutcome`
* change the completion reason
* decide simultaneous-zero priority
* enter reward flow
* enter run victory or defeat
* clear the pending outcome

### Important rule

Continue is a presentation gate, not a gameplay-decision command.

Delaying or pressing Continue cannot change the already-fixed battle result.

---

## Battle Runtime Flow

Once a battle has started, the battle-level flow is:

1. wait for a legal player card
2. validate the submitted action
3. resolve all seven round phases
4. apply damage, healing, and clamp
5. finalize `RoundResult`
6. classify and store any completed battle outcome
7. present the finalized round result
8. after presentation:

   * continue to the next round
   * or expose the already-fixed completed outcome to `RunService`

### Important timing rule

Battle completion may already be fixed while:

* `BattleFlowStage == PresentingRoundResult`
* `RunState.FlowStage == InBattle`

This is intentional.

The battle remains attached to the run until `RunService.AcceptCompletedBattle(...)` accepts the completed outcome.

---

## Waiting for Player Input

When battle flow is `WaitingForPlayerCard`:

* the system should allow the player to choose one legal unused card
* no round resolution is currently running
* the UI should present valid options clearly

### Important Rule

The system does not pause inside a round phase.
Instead, it remains in the battle flow stage `WaitingForPlayerCard` until input arrives.

---

## Submitting a Player Card

When the player selects a card, the application passes the selection to `BattleService.SubmitPlayerCard(...)`.

### Validation responsibilities

BattleService should validate:

* an active battle exists
* battle flow is `WaitingForPlayerCard`
* the selected card exists in the player deck
* the card has not already been used in this battle
* the current round index is valid
* a matching enemy card exists in the prepared enemy sequence

If validation fails:

* no state should be advanced
* a failure result should be returned

If validation succeeds:

* round resolution begins

---

## Resolving a Round

After validation:

* `BattleService` sets battle flow to `ResolvingRound`
* it selects the enemy card for the current round
* it calls `RoundResolver.ResolveRound(...)`

`RoundResolver` performs the fixed seven-phase round pipeline and returns a `RoundResult`.

The internals of round calculation are defined in `round-resolution.md`.

---

## Applying a Round Result

After `RoundResult` is returned, `BattleService` applies the battle-related consequences immediately.

This includes:

* updating player HP
* updating current enemy HP
* clamping healing to max HP
* appending the round result to battle history
* appending round logs to battle logs
* appending round snapshots to battle snapshots

If `RoundResult` stores final HP-after values for presentation and inspection, those values should be finalized here so that they match the authoritative runtime state after damage, healing, and clamp are applied.

### Important Rule

Battle results should be applied immediately after the round resolves.
They should not be delayed until the end of the battle.

This keeps runtime state authoritative and keeps debug UI honest.

---

## Presenting a Round Result

After the round has been fully resolved, applied, finalized, and classified:

* `BattleFlowStage` becomes `PresentingRoundResult`

This stage allows Presentation to:

* display final HP
* display HP after merged damage
* display raw and actual healing
* inspect slot results
* inspect logs
* inspect phase snapshots
* show the final round even when it caused player defeat

Presentation may inspect `PendingBattleOutcome` for display purposes.

It must not alter or reinterpret it.

---

## When a Battle Is Complete

A battle completes after a fully applied round when exactly one of the following official reasons is produced:

### Player defeated

* final player HP is zero or below
* this may occur in round 1, 2, or 3
* this reason takes priority over all other completion facts

### Enemy defeated

* final player HP is above zero
* final enemy HP is zero or below
* this may occur before round 3

### All rounds completed

* both final HP values are above zero
* the third round has completed

The battle does not complete merely because HP was temporarily zero or below after merged damage.

---

## BattleOutcome

`BattleOutcome` is the immutable handoff object from battle-level orchestration to run-level orchestration.

It contains:

* `BattleIndexForEnemy`
* `RoundsPlayed`
* `CompletionReason`
* `PlayerHpAfterBattle`
* `EnemyHpAfterBattle`

The official result is represented by one `BattleCompletionReason`.

Raw final HP values may show simultaneous zero, but the official reason remains:

* `PlayerDefeated`

---

## BattleCommandResult

`BattleCommandResult` is the battle-level command return object.

It should report:

* whether the command succeeded
* why it failed if it failed
* the current `BattleFlowStage`
* any produced `RoundResult`
* whether the battle is complete
* any produced `BattleOutcome`

This object is intentionally separate from `RoundResult` and `BattleOutcome`.

---

## Accepting a Battle Outcome

When a battle reaches `BattleComplete`, `RunService` takes over again.

### RunService responsibilities

It should:

* increment `CurrentEnemy.BattlesPlayed`
* clear `RunState.ActiveBattle`
* inspect the battle outcome and current enemy state
* decide the next run-level step

This handoff is one of the most important boundaries in the architecture.

---

## Interpreting Battle Outcome

The official completed battle result is interpreted in this exact order.

### Branch A: Player defeated

If:

```
CompletionReason ==
    BattleCompletionReason.PlayerDefeated
```

then:

* the run enters `Defeat`
* no reward flow begins
* no enemy-defeat progression occurs
* no next enemy is created
* no victory check occurs

The enemy's final HP may also be zero or below.

That raw HP fact does not change the official result.

### Branch B: Enemy defeated while player survives

If:

```
CompletionReason ==
    BattleCompletionReason.EnemyDefeated
```

then:

* the current enemy is officially defeated
* non-final-enemy reward settlement proceeds normally
* final-enemy unresolved rewards are ignored
* progression may eventually move to the next enemy or `Victory`

### Branch C: All rounds completed and the configured battle limit is exhausted

If:

* `CompletionReason == BattleCompletionReason.AllRoundsCompleted`
* `CurrentEnemy.BattlesPlayed >= CurrentEnemy.Config.battleLimit`

then:

* the run enters `Defeat`
* no reward flow begins

### Branch D: All rounds completed and more configured battles remain

If:

* `CompletionReason == BattleCompletionReason.AllRoundsCompleted`
* `CurrentEnemy.BattlesPlayed < CurrentEnemy.Config.battleLimit`

then:

* one normal post-battle reward becomes due
* after reward resolution, the run becomes `ReadyForNextBattle`

### Important note

The branch order is gameplay-significant.

Enemy HP must not be checked before the official `PlayerDefeated` reason.

---

## Reward Timing Rule

The implementation should interpret reward timing like this:

* each enemy provides reward opportunities totaling that enemy's configured `battleLimit`
* normally, one reward is obtained after each reward-eligible completed battle
* if the enemy is defeated before all of its configured reward opportunities have been resolved, the remaining rewards are settled immediately
* if the defeated enemy is the final enemy, any unresolved remaining rewards are ignored

A completed battle is not automatically reward-eligible. Player defeat and battle-limit exhaustion short-circuit reward flow as defined below.

### Important design rule

Reward total for an enemy is derived from that enemy's configured `battleLimit`.

The system should not introduce a separate authored reward-count source of truth for the current demo.

### Default baseline note

Under the current default baseline, many examples still appear as "three rewards per enemy" because the default enemy battle limit is 3.

That is a default content value, not a permanent implementation constant.

---

## Reward Eligibility Gate

Reward flow may begin only after `RunService` has accepted an eligible completed battle.

### Reward flow is prohibited when:

* `CompletionReason == BattleCompletionReason.PlayerDefeated`
* simultaneous zero has resolved as `BattleCompletionReason.PlayerDefeated`
* `CompletionReason == BattleCompletionReason.AllRoundsCompleted` and the configured battle limit has been exhausted
* the run is already in `Victory`
* the run is already in `Defeat`

### Reward flow may begin when:

* the player survived and the current enemy was officially defeated, provided the current enemy is not the final enemy
* the player and enemy survived the completed three-round battle and more configured battles remain

### Important rule

`RewardService` does not need player-death logic.

`RunService` prevents the call entirely when the completed battle is not reward-eligible.

`RewardsClaimed` must not change until the player resolves an actual pending reward option.

---

## Entering Reward Flow

Reward flow should begin whenever one or more reward resolutions are immediately due.

This can happen in two main situations:

### Situation 1: normal eligible post-battle reward

One reward-eligible completed battle normally makes one reward immediately due.

### Situation 2: early-defeat remainder settlement

If the current enemy has been defeated before all of that enemy's reward opportunities have been resolved, additional rewards become immediately due until:

* `CurrentEnemy.RewardsClaimed >= CurrentEnemy.Config.battleLimit`

### Entering reward flow behavior

When reward flow begins:

1. `RunState.ActiveBattle` must already be cleared
2. `RunState.FlowStage` becomes `ChoosingReward`
3. `RewardService.GenerateOffer(...)` is called
4. the returned `RewardOffer` is stored in `RunState.PendingRewardOffer`

### Important note

If the defeated enemy is the final enemy, unresolved remaining rewards should not be entered.
The run should move toward `Victory` instead of generating meaningless post-run reward offers.

### RunService action

RunService requests a reward offer from `RewardService`.

### RewardService action

RewardService generates one `RewardOffer` according to the reward rules.

### RunState update

* `RunState.PendingRewardOffer` is set
* `RunState.FlowStage = ChoosingReward`

### Important Rule

At this point:

* there must be no active battle
* there must be exactly one pending reward offer

---

## Choosing a Reward

When the player selects a reward option:

* the request should be routed through run-level flow, not directly through battle flow

Recommended conceptual entry:

* `RunService.ChooseReward(...)`

### Why RunService owns this step

Because reward selection affects:

* enemy reward progression
* next-step run progression
* whether another reward must be generated immediately
* whether the next step is next battle, next enemy, victory, or defeat

These are run-level decisions.

---

## Applying a Reward Choice

Once a reward option is selected:

* `RunService` delegates application to `RewardService`

### RewardService responsibilities during application

#### Upgrade

* modify the target card instance in place

#### Replace

* replace the target deck card with a new card instance in the same deck position

#### Skip

* leave the deck unchanged

### RunService responsibilities after application

* increment `CurrentEnemy.RewardsClaimed`
* clear `PendingRewardOffer`
* decide what comes next

---

## Continuing Reward Settlement

After one reward choice is applied, the run must decide whether another reward is immediately due or whether normal progression should continue.

### Case 1: enemy already defeated and unresolved rewards remain

If:

* the current enemy has already been defeated
* `CurrentEnemy.RewardsClaimed < CurrentEnemy.Config.battleLimit`

then:

* generate another reward offer immediately
* keep the run in `ChoosingReward`

### Case 2: enemy not defeated and the expected reward for the completed battle is now resolved

If:

* the current enemy is not defeated
* the normal post-battle reward has now been resolved

then:

* clear `PendingRewardOffer`
* set the run to `ReadyForNextBattle`

### Case 3: enemy defeated and all rewards for that enemy have been settled

If:

* the current enemy has been defeated
* `CurrentEnemy.RewardsClaimed >= CurrentEnemy.Config.battleLimit`

then:

* clear `PendingRewardOffer`
* if another enemy remains, advance to that enemy and set the run to `ReadyForNextBattle`
* otherwise enter `Victory`

### Special rule for the final enemy

If the final enemy has been defeated, unresolved remaining rewards are ignored rather than continued.

The run should proceed directly to `Victory`.

---

## Moving to the Next Battle

If:

* the current enemy is not defeated
* `CurrentEnemy.BattlesPlayed < CurrentEnemy.Config.battleLimit`
* no reward is pending

Then:

* run flow becomes `ReadyForNextBattle`

At that point the next battle may be started by `RunService`.

---

## Moving to the Next Enemy

If:

* the current enemy was officially defeated while the player survived
* the current enemy is not the final enemy
* `CurrentEnemy.RewardsClaimed >= CurrentEnemy.Config.battleLimit`
* no reward is pending

Then `RunService` should:

* increment `CurrentEnemyIndex`
* create the next `EnemyProgressState`
* preserve player HP
* preserve player max HP
* preserve the current deck
* keep `ActiveBattle = null`
* keep `PendingRewardOffer = null`
* set `FlowStage = ReadyForNextBattle`

---

## Victory

The run enters `Victory` only when:

* the player survived
* the final enemy was officially defeated
* the completed outcome reason was `EnemyDefeated`

Unresolved reward opportunities for the final enemy are ignored.

At this point:

* `ActiveBattle == null`
* `PendingRewardOffer == null`
* no further gameplay command is legal

Simultaneous zero must never enter this branch.

---

## Defeat

The current run has two official defeat sources.

### Source 1: Player death

If:

```
CompletionReason ==
    BattleCompletionReason.PlayerDefeated
```

then:

* `FlowStage = Defeat`
* `ActiveBattle = null`
* `PendingRewardOffer = null`

This may happen after round 1, 2, or 3.

It also includes simultaneous zero.

### Source 2: Battle-limit exhaustion

If:

* `CompletionReason == BattleCompletionReason.AllRoundsCompleted`
* `CurrentEnemy.BattlesPlayed >= CurrentEnemy.Config.battleLimit`

then:

* `FlowStage = Defeat`
* `ActiveBattle = null`
* `PendingRewardOffer = null`

### Terminal behavior

After entering `Defeat`:

* no next battle may start
* no player card may be submitted
* no reward may be generated or chosen
* no next enemy may be entered
* the run must remain terminal

---

## RunCommandResult

`RunCommandResult` is the run-level command return object.

It should report:

* whether the command succeeded
* why it failed if it failed
* the current `RunFlowStage`
* whether an active battle now exists
* whether a pending reward offer now exists
* whether the run is complete

Typical fields:

* `Success`
* `FailureReason`
* `FlowStage`
* `ActiveBattle`
* `PendingRewardOffer`
* `IsRunComplete`

This object is intentionally separate from `BattleOutcome`.

---

## Global Flow Invariants

The run and battle flow must maintain these invariants:

### Run-level invariants

1. `ActiveBattle` and `PendingRewardOffer` must not coexist
2. `InBattle` requires a valid active battle
3. `ChoosingReward` requires a valid pending reward offer
4. `Victory` and `Defeat` must be terminal states
5. terminal states must not leave intermediate flow objects active
6. player defeat must be interpreted before enemy defeat
7. a player-defeat outcome must never create a reward offer
8. simultaneous zero must never enter enemy-defeat, next-enemy, or victory progression
9. `RewardsClaimed` changes only when a pending reward choice is resolved
10. a completed fatal battle may increment `BattlesPlayed`, but that increment must not make the battle reward-eligible

### Battle-level invariants

1. battle round progression belongs to `BattleService`
2. battle flow stage must always reflect actual battle state
3. `RoundResolver` does not advance battle flow on its own
4. authoritative final HP is established before battle completion is classified
5. `PendingBattleOutcome` is fixed before round-result presentation begins
6. `FinishRoundPresentation(...)` does not calculate or alter gameplay outcome
7. `BattleComplete` requires a non-null authoritative pending outcome
8. the pending outcome remains available until run-level acceptance clears the active battle

### Reward-level invariants

1. reward application does not decide run progression by itself
2. reward generation happens only when run flow requests it
3. one reward offer is resolved before another begins

### Presentation-level invariant

1. Presentation may display gameplay results and forward commands
2. Presentation must not determine player death, enemy defeat, simultaneous-zero priority, reward eligibility, victory, or defeat

---

## Responsibilities That Must Remain Separate

### RoundResolver must not:

* start battles
* finish battles
* generate rewards
* advance run progression

### BattleService must not:

* decide next enemy
* decide victory or defeat for the full run
* generate rewards directly
* silently change run-level flow outside its responsibility

### RunService must not:

* compute low-level round rules
* manually reimplement battle resolution
* manually reimplement reward legality

### RewardService must not:

* decide when rewards happen
* decide what run stage comes next
* change unrelated run state

---

## Recommended Runtime Loop Summary

A complete run follows this pattern:

1. create the run

2. prepare the current enemy

3. start a battle

4. wait for player input

5. fully resolve all seven round phases

6. apply damage, healing, and clamp

7. establish final HP

8. fix any completed battle outcome

9. present the finalized round result

10. finish the presentation gate

11. if the battle continues, begin the next round

12. if the battle completed, accept the fixed outcome at run level

13. interpret the outcome in this order:

    * player defeated
    * enemy defeated
    * battle limit exhausted
    * more battles remain

14. enter reward flow only when eligible

15. proceed to:

    * the next battle
    * the next enemy
    * `Victory`
    * `Defeat`

This loop repeats until the run reaches a terminal stage.

---

## Why This Flow Design Works

This design keeps each scope separate:

* round logic stays local to `RoundResolver`
* battle progression stays local to `BattleService`
* run progression stays local to `RunService`
* reward generation and application stay local to `RewardService`

This gives the system:

* clearer debugging
* cleaner tests
* lower coupling
* easier iteration
* a better path toward a debug-first UI

---

## Summary

The run, battle, and reward flow is intentionally layered.

Key rules:

* `RunService` owns run progression
* `BattleService` owns battle progression
* `RewardService` owns reward generation and reward application
* `RoundResolver` owns round rule resolution

The system also uses explicit flow stages:

* `ReadyForNextBattle`
* `InBattle`
* `ChoosingReward`
* `Victory`
* `Defeat`

and:

* `WaitingForPlayerCard`
* `ResolvingRound`
* `PresentingRoundResult`
* `BattleComplete`

It also uses explicit application result objects:

* `RunCommandResult`
* `BattleCommandResult`

The whole system is designed so that:

* state is explicit
* transitions are explicit
* rewards are explicit
* battle results are explicit
* responsibility boundaries remain clean

The player-death flow also requires that:

* all seven round phases complete before final HP is classified
* `BattleService` fixes one authoritative `BattleCompletionReason` after final HP application
* `PendingBattleOutcome` remains stable through round-result presentation
* `FinishRoundPresentation(...)` does not calculate gameplay outcome
* `RunService` interprets `PlayerDefeated` before any enemy-defeat or reward branch
* player defeat and simultaneous zero never enter reward, next-enemy, or victory progression
* battle-limit exhaustion remains the second official run-defeat source