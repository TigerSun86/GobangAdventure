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
* `RewardService` generates and applies reward choices

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

### Ownership Summary

* `RunState` owns run-level state
* `EnemyProgressState` owns current enemy progression
* `BattleState` owns one active battle
* `RewardOffer` owns one pending reward selection
* `BattleOutcome` summarizes one completed battle
* `RoundResult` summarizes one resolved round

---

## Run-Level Flow Stages

The exact enum names may still be finalized later, but the run-level flow concept is already stable.

The run must always be in one major stage at a time.

Recommended conceptual stages:

* `ReadyForNextBattle`
* `InBattle`
* `ChoosingReward`
* `Victory`
* `Defeat`

Additional transitional stages may be introduced later if useful, but the core idea should remain simple.

### Important Rule

At run level:

* an active battle and a pending reward must not coexist
* victory and defeat must be terminal states
* the run stage must always match the actual runtime state

---

## Battle-Level Flow Stages

A battle also has its own internal flow.

Recommended conceptual stages:

* `WaitingForPlayerCard`
* `ResolvingRound`
* `PresentingRoundResult`
* `BattleComplete`

These battle-level stages are distinct from gameplay resolution phases.

### Important Distinction

Do not confuse:

* battle flow stage
  with
* round resolution phase

Examples:

* `WaitingForPlayerCard` is a battle flow state
* `Movement` is a round resolution phase

They exist at different levels.

---

## RunService

## Responsibility

`RunService` is the high-level orchestrator for the entire run.

It is responsible for:

* creating a new run
* preparing the current enemy
* deciding when a battle may begin
* accepting a completed battle outcome
* deciding whether to enter reward selection
* deciding when to continue to the next battle
* deciding when to move to the next enemy
* deciding victory or defeat

It is not responsible for:

* low-level round resolution
* battle-internal round progression
* reward candidate generation internals

---

## BattleService

## Responsibility

`BattleService` is the orchestrator for one battle.

It is responsible for:

* starting a battle
* preparing enemy sequence data
* validating player card selection
* invoking `RoundResolver`
* applying round results to battle-related runtime state
* advancing battle-level flow
* producing `BattleOutcome`

It is not responsible for:

* deciding run-level progression
* generating reward offers
* deciding victory or defeat for the entire run

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

## Start of Run

A run begins when `RunService` creates a new `RunState`.

### RunService responsibilities at run start

It should:

* create the player deck
* set the starting player HP
* set the current enemy index to the first enemy
* create the first `EnemyProgressState`
* clear any active battle
* clear any pending reward
* set the run flow to a stage that allows the first battle to begin

### Result

After initialization:

* the player has a valid deck
* the first enemy is active
* there is no active battle
* there is no pending reward
* the system is ready to begin the first battle

---

## Current Enemy Lifecycle

Each enemy is represented at runtime by `EnemyProgressState`.

It tracks:

* enemy definition
* current HP
* number of battles played against this enemy
* number of rewards already claimed from this enemy

This object exists because enemy HP persists across multiple battles.

It is created when entering an enemy and discarded when the system moves to the next enemy or the run ends.

---

## Starting a Battle

A new battle begins only when `RunService` decides it is allowed.

### Conditions required to start a battle

A battle may start only if:

* the run is not already over
* there is a current enemy
* there is no active battle
* there is no pending reward
* the current enemy still requires more battles or can still be fought

### RunService action

When the conditions are satisfied:

* `RunService` calls `BattleService.StartBattle(...)`

### BattleService action

`BattleService` creates a new `BattleState` and initializes:

* battle index for the current enemy
* round index
* empty lanes
* used player card tracking
* enemy sequence for this battle
* battle flow stage as `WaitingForPlayerCard`

### RunState update

After battle creation:

* `RunState.ActiveBattle` is set
* run flow becomes `InBattle`

---

## Enemy Sequence Preparation

At battle start, the enemy uses its fixed six-card set.

For the current battle:

* the six enemy cards are shuffled
* the first three are stored as the battle's enemy sequence
* round `n` uses enemy sequence element `n`

This sequence belongs to `BattleState`, not `RunState`.

---

## Battle Runtime Flow

Once a battle has started, `BattleService` becomes the main active flow orchestrator until the battle completes.

The typical battle loop is:

1. wait for player card input
2. validate the input
3. resolve one round
4. apply round result
5. present round result
6. either advance to the next round or finish the battle

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

When the player selects a card, the application passes the selection to `BattleService`.

### BattleService validation responsibilities

It should validate:

* an active battle exists
* battle flow is `WaitingForPlayerCard`
* the selected card exists in the player deck
* the card has not already been used in this battle
* the current round index is valid
* a matching enemy card exists in the prepared enemy sequence

If validation fails:

* no state should be advanced
* a failure result should be returned to the caller

If validation succeeds:

* the battle proceeds into round resolution

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
* appending the round result to battle history
* appending round logs to battle logs
* appending round snapshots to battle snapshots

### Important Rule

Battle results should be applied immediately after the round resolves.
They should not be delayed until the end of the battle.

This keeps runtime state authoritative and keeps debug UI honest.

---

## Presenting a Round Result

After a round result is applied:

* battle flow becomes `PresentingRoundResult`

This stage exists so the UI can:

* display round logs
* inspect slot results
* inspect phase snapshots
* later play animations if needed

### Important Rule

`RoundResolver` calculates the full round first.
Presentation consumes the result afterward.
Rule calculation should not be paused mid-phase for UI.

---

## Advancing After Presentation

Once round presentation is complete, the application signals the battle flow to continue.

At this point `BattleService` decides:

### If the battle is not yet complete

* increment round index
* set battle flow back to `WaitingForPlayerCard`

### If the battle is complete

* set battle flow to `BattleComplete`
* produce a `BattleOutcome`

---

## When a Battle Is Complete

A battle is complete if either of these is true:

* the current enemy HP is now zero or below
* the third round of the battle has already been resolved

This means:

* a battle can end early if the enemy dies
* a battle also ends after round three if the enemy survives

---

## BattleOutcome

`BattleOutcome` is the handoff object from battle-level orchestration to run-level orchestration.

It should summarize:

* battle index for the current enemy
* number of rounds played
* whether the enemy was defeated
* player HP after the battle
* enemy HP after the battle

This object exists to keep `RunService` from depending on the full internal details of `BattleState`.

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

After a battle is complete, `RunService` must choose one of three branches.

### Branch A: Enemy defeated

If the enemy HP is zero or below:

* the current enemy is defeated

RunService must then determine reward settlement for this enemy.

### Branch B: Enemy not defeated, but all three battles used

If:

* enemy HP is still above zero
* `BattlesPlayed` has reached three

Then:

* the run enters `Defeat`

### Branch C: Enemy not defeated, and more battles remain

If:

* enemy HP is above zero
* fewer than three battles have been played

Then:

* reward handling for the completed battle must occur
* afterward the run should become ready for the next battle

---

## Reward Timing Rule

The implementation discussion assumes this interpretation of the gameplay rules:

* each enemy provides three rewards total
* normally, one reward is obtained after each completed battle
* if the enemy is defeated before all three battles are used, all remaining rewards are settled immediately

This interpretation should remain aligned with the locked gameplay documents.

---

## Entering Reward Flow

Reward flow begins only when `RunService` decides it should happen.

Typical situations:

* one battle has completed and one normal reward is due
* the enemy has been defeated early and multiple remaining rewards must now be settled

### RunService action

RunService requests a reward offer from `RewardService`.

### RewardService action

RewardService generates one `RewardOffer` according to the reward rules.

### RunState update

* `RunState.PendingRewardOffer` is set
* run flow becomes `ChoosingReward`

### Important Rule

At this point:

* there must be no active battle
* there must be exactly one pending reward offer

---

## Choosing a Reward

When the player selects a reward option:

* the request should be routed through run-level flow, not directly through battle flow

Recommended conceptual entry:

* `RunService.AcceptRewardChoice(...)`

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

After one reward is chosen, `RunService` checks whether more rewards are immediately owed.

### Case 1: enemy already defeated and more rewards remain

Generate another reward offer immediately and stay in `ChoosingReward`.

### Case 2: enemy not defeated and the expected reward for the completed battle is now resolved

Move toward `ReadyForNextBattle`.

### Case 3: enemy defeated and all rewards have been settled

Move toward next enemy or victory.

---

## Moving to the Next Battle

If:

* the current enemy is not defeated
* fewer than three battles have been played
* no reward is pending

Then:

* run flow becomes `ReadyForNextBattle`

At that point the next battle may be started by `RunService`.

---

## Moving to the Next Enemy

If:

* the current enemy is defeated
* all rewards for that enemy have been settled
* more enemies remain

Then `RunService` should:

* increment `CurrentEnemyIndex`
* create a new `EnemyProgressState`
* clear active battle and pending reward
* preserve player HP
* preserve the current deck
* set run flow to `ReadyForNextBattle`

### Important Rule

Player HP persists to the next enemy.
The player deck also persists.

---

## Victory

If:

* the final enemy is defeated
* all rewards for that enemy have been settled

Then:

* run flow becomes `Victory`

At this point:

* no active battle should remain
* no pending reward should remain

---

## Defeat

If:

* the current enemy is still alive
* three battles against that enemy have been used

Then:

* run flow becomes `Defeat`

At this point:

* no active battle should remain
* no pending reward should remain

---

## Global Flow Invariants

The run and battle flow must maintain these invariants:

### Run-level invariants

1. `ActiveBattle` and `PendingRewardOffer` must not coexist
2. `InBattle` requires a valid active battle
3. `ChoosingReward` requires a valid pending reward offer
4. `Victory` and `Defeat` must be terminal states
5. terminal states must not leave intermediate flow objects active

### Battle-level invariants

1. battle round progression belongs to `BattleService`
2. battle flow stage must always reflect actual battle state
3. `RoundResolver` does not advance battle flow on its own

### Reward-level invariants

1. reward application does not decide run progression by itself
2. reward generation happens only when run flow requests it
3. one reward offer is resolved before another begins

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

1. create run
2. prepare current enemy
3. start battle
4. resolve rounds until battle completes
5. accept battle outcome
6. enter reward flow when required
7. apply reward choice
8. either:

   * go to next battle
   * go to next enemy
   * enter victory
   * enter defeat

This loop repeats until the run ends.

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

The whole system is designed so that:

* state is explicit
* transitions are explicit
* rewards are explicit
* battle results are explicit
* responsibility boundaries remain clean