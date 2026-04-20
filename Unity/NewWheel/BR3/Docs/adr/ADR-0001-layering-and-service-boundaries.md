# ADR-0001: Layering and Service Boundaries

## Status

Accepted

---

## Context

The project is a Unity 6.4 2D turn-based card roguelike demo.

The gameplay system includes:

* run-level progression across multiple enemies
* battle-level progression across multiple rounds
* round-level resolution with a fixed phase pipeline
* reward generation and reward application
* a debug-first presentation approach

Without clear architectural boundaries, the codebase would likely drift toward one or more of these failure modes:

* gameplay rules embedded directly in UI code
* services holding hidden mutable game state
* duplicated rule logic across layers
* battle flow and run flow becoming mixed together
* configuration becoming tightly coupled to Unity-specific asset types too early
* reward generation logic spreading into unrelated modules

A clean separation is needed so the demo can:

* remain understandable
* support fast iteration
* support Codex-assisted implementation
* remain testable
* remain debuggable

---

## Decision

The project will use a three-layer architecture:

1. Domain
2. Application
3. Presentation

Within that architecture, the main service and rule boundaries are:

* `RoundResolver` resolves one round of gameplay rules
* `BattleService` orchestrates one battle
* `RunService` orchestrates one run
* `RewardService` generates and applies rewards

State should live in explicit runtime state objects, not inside services.

Services should be long-lived and mostly stateless, operating on explicit state passed to them.

---

## Detailed Layering Decision

## Domain

### Domain contains

* runtime state objects
* value objects
* result objects
* round resolution logic
* reward legality rules
* reward deduplication rules
* canonical signature logic

### Domain examples

* `RunState`
* `EnemyProgressState`
* `BattleState`
* `LaneState`
* `BoardSlotState`
* `CardInstance`
* `BoardCard`
* `RewardOffer`
* `RewardOption`
* `RoundResult`
* `SlotCombatResult`
* `PhaseSnapshot`
* `BattleOutcome`
* `RoundResolver`

### Domain does not contain

* UI logic
* scene logic
* controller logic
* Unity-specific presentation behavior
* battle orchestration flow
* run orchestration flow

---

## Application

### Application contains

* run-level orchestration
* battle-level orchestration
* reward timing orchestration
* command entry points
* state transition orchestration
* coordination between domain objects and presentation

### Application examples

* `RunService`
* `BattleService`
* `RewardService`

### Application does not contain

* duplicated low-level gameplay rule logic
* UI rendering
* direct player-facing presentation formatting as its primary responsibility

---

## Presentation

### Presentation contains

* debug UI
* scene wiring
* input forwarding
* view models
* display formatting
* presentation-only helpers

### Presentation does not contain

* gameplay rules
* reward legality logic
* run progression rules
* battle progression rules

---

## Main Boundary Decisions

## RoundResolver

### Responsibility

`RoundResolver` resolves exactly one round of gameplay.

It owns:

* the fixed seven-phase round pipeline
* round-local combat recalculation
* movement application
* board-derived power application
* slot combat resolution
* round-local damage and healing totals
* post-resolve trait effects
* generation of `RoundResult`
* generation of round logs and phase snapshots

### It does not own

* waiting for player input
* battle flow progression
* run flow progression
* reward generation
* reward timing
* UI transitions

### Reason

Round resolution is rule evaluation, not flow orchestration.

---

## BattleService

### Responsibility

`BattleService` orchestrates one battle.

It owns:

* battle startup
* enemy sequence preparation
* player card submission validation
* invocation of `RoundResolver`
* application of round results to battle-related runtime state
* battle-level flow transitions
* generation of `BattleOutcome`

### It may modify

* `RunState.PlayerHp`
* `EnemyProgressState.CurrentHp`
* `BattleState`
* battle-level logs and snapshots

### It does not own

* next enemy selection
* reward entry decisions
* final run victory or defeat decisions
* run-level stage transitions beyond battle responsibility

### Reason

BattleService is allowed to apply battle consequences immediately, but it must not decide what those consequences mean for the full run.

---

## RunService

### Responsibility

`RunService` orchestrates the full run.

It owns:

* creating a new run
* determining when a battle may start
* accepting a completed battle outcome
* deciding when rewards should begin
* deciding when the next battle should begin
* deciding when to move to the next enemy
* deciding victory or defeat
* maintaining run-level flow invariants

### It does not own

* low-level round rule calculation
* battle-internal round transitions
* reward legality and candidate generation internals

### Reason

Run-level progression is distinct from battle-level progression and should remain centralized.

---

## RewardService

### Responsibility

`RewardService` owns:

* reward offer generation
* upgrade legality checks
* replace legality checks
* reward deduplication
* canonical resulting deck comparison
* reward application to the player deck

### It does not own

* deciding when rewards happen
* deciding post-reward progression
* battle flow
* run flow

### Reason

Reward content and reward timing are separate concerns and should remain separate.

---

## State Ownership Decision

State must live in explicit state objects rather than being hidden inside services.

### Main runtime roots

* `RunState`
* `EnemyProgressState`
* `BattleState`
* `CardInstance`
* `BoardCard`

### Important rule

Services should not become hidden state containers.

### Why

This keeps:

* testing easier
* debugging easier
* state inspection easier
* boundaries easier to enforce
* Codex implementation tasks easier to scope

---

## Result Object Decision

The architecture distinguishes between:

* domain result objects
* application command result objects

### Domain result objects

These describe gameplay facts:

* `RoundResult`
* `BattleOutcome`

### Application command result objects

These describe service command execution:

* `BattleCommandResult`
* `RunCommandResult`

### Why this distinction exists

A command execution result is not the same thing as a gameplay outcome.
Keeping those concepts separate improves clarity.

---

## Config Boundary Decision

Authored config is not runtime state.

The system uses:

* JSON-authored config
* pure C# config objects
* a thin Unity loader
* explicit conversion into runtime state

### Why this belongs in the layering ADR

Because config ownership is part of architecture:

* authored config is not Domain runtime state
* loading config is not the same as running gameplay
* runtime state should not repeatedly consult authored config for active gameplay values such as max HP

A separate ADR records the JSON-over-ScriptableObject decision in more detail.

---

## Factory and Service Boundary Decision

The project distinguishes between:

* construction helpers
* flow orchestrators

### Construction helper

* `RuntimeStateFactory`

### Flow orchestrators

* `RunService`
* `BattleService`
* `RewardService`

### Specific decision

`RunService.CreateNewRun(...)` is the use-case entry point for starting a run, but it delegates initial runtime object construction to `RuntimeStateFactory`.

### Why

This avoids forcing services to manually assemble object graphs while still keeping flow ownership in the service layer.

---

## Flow Ownership Decision

### Run-level flow

Owned by `RunService`

### Battle-level flow

Owned by `BattleService`

### Round phase flow

Owned by `RoundResolver`

### Presentation sequencing

Owned by Presentation / controller-side UI flow

### Why

These flows exist at different scopes and should not be collapsed into one state machine.

---

## Debug Visibility Decision

Debug visibility is treated as a first-class architectural concern.

The architecture explicitly includes:

* `RoundResult`
* `SlotCombatResult`
* `PhaseSnapshot`
* battle logs
* round logs

### Why

The demo prioritizes correctness and inspectability before polished presentation.

This decision affects:

* result object design
* resolver output design
* UI planning
* testing strategy

---

## Alternatives Considered

## Alternative 1: Put most gameplay logic in MonoBehaviours

Rejected.

### Reasons

* harder to test
* harder to inspect
* harder to review in code
* encourages hidden state
* mixes gameplay rules with Unity lifecycle concerns too early

---

## Alternative 2: Merge battle and run flow into one large service

Rejected.

### Reasons

* weakens boundaries
* makes future maintenance harder
* makes battle logic and run logic harder to reason about
* increases the risk of duplicated responsibilities

---

## Alternative 3: Use a generic event bus for major rule execution

Rejected for the current demo phase.

### Reasons

* adds complexity too early
* makes rule order harder to reason about
* weakens explicitness
* is not needed for the current scale

---

## Alternative 4: Fully data-drive trait behavior from config

Rejected for the current demo phase.

### Reasons

* overengineered for current trait count and behavior complexity
* increases config complexity
* increases serialization complexity
* weakens clarity during early implementation

---

## Consequences

### Positive consequences

* clearer boundaries
* better testability
* better debugability
* easier Codex task decomposition
* easier review of implementation correctness
* lower risk of flow/rule confusion
* cleaner future path for growth

### Negative consequences

* more explicit objects and result types
* more up-front design work
* more responsibility to keep documents and boundaries synchronized
* some indirection between layers that a smaller toy implementation might skip

These tradeoffs are acceptable and intentional.

---

## Follow-Up Decisions

This ADR establishes the base architectural direction for later decisions such as:

* JSON config instead of ScriptableObject as primary authored content
* reward deduplication by canonical resulting deck state
* debug-first UI planning
* lightweight Edit Mode tests around domain logic

---

## Summary

The project adopts a layered architecture with explicit service boundaries:

* Domain owns rules and runtime state structures
* Application owns orchestration and transitions
* Presentation owns display and input forwarding

Within that architecture:

* `RoundResolver` resolves rounds
* `BattleService` orchestrates battles
* `RunService` orchestrates runs
* `RewardService` owns reward generation and application

This is the accepted foundation for the current demo.