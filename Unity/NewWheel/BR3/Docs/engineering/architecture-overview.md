# Architecture Overview

## Purpose

This document describes the high-level software architecture for the Unity 6.4 2D turn-based card roguelike demo.

It explains:

* the main architectural layers
* the core modules and their responsibilities
* the main runtime state objects
* the direction of dependencies
* the implementation priorities for the current demo phase

This document is a high-level engineering reference. More detailed behavior is specified in:

* `domain-model.md`
* `round-resolution.md`
* `run-battle-reward-flow.md`
* `reward-generation.md`

---

## Design Goals

The current architecture is optimized for:

1. gameplay rule correctness
2. clean separation of responsibilities
3. fast iteration during early development
4. debug visibility
5. easy verification through lightweight tests
6. future expansion without heavy refactoring

The current goal is not final production polish.

---

## Architecture Summary

The project is split into three layers:

1. Domain
2. Application
3. Presentation

This split is intentional.

* Domain defines what the game state is and how core rules work.
* Application defines how the game flow is orchestrated.
* Presentation defines how state is shown and how player input is collected.

---

## Layer 1: Domain

### Responsibilities

The Domain layer contains:

* core runtime state objects
* enums and value objects
* result objects
* round resolution logic
* reward legality rules
* reward deduplication rules
* canonical signature logic

### Examples

Typical Domain objects include:

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
* `RoundResolver`

### Domain Principles

* Domain logic should be as Unity-agnostic as practical.
* State should live in state objects, not in services.
* Core rule logic should be explicit and testable.
* Temporary round computation should not pollute persistent runtime state.

---

## Layer 2: Application

### Responsibilities

The Application layer contains:

* run-level flow orchestration
* battle-level flow orchestration
* reward flow orchestration
* command handling
* state transitions
* coordination between domain logic and presentation

### Examples

Typical Application objects include:

* `RunService`
* `BattleService`
* `RewardService`
* controller-style entry points that translate UI input into application commands

### Application Principles

* Application code should orchestrate, not reimplement domain rules.
* Application flow should be explicit and state-driven.
* The layer should coordinate existing state objects rather than hide them behind unnecessary abstractions.

---

## Layer 3: Presentation

### Responsibilities

The Presentation layer contains:

* debug UI
* view models
* display formatting
* input wiring
* scene-level wiring
* presentation-only helpers

### Examples

Typical Presentation objects include:

* debug scene components
* debug UI controllers
* view model builders
* UI formatting helpers

### Presentation Principles

* Presentation must not contain gameplay rule logic.
* UI should read state and send commands.
* The first implementation target is a debug-oriented text-heavy UI, not a polished final UI.

---

## Core Runtime State Tree

The runtime state is organized around a single run-level root.

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

This state tree is the authoritative source of the current run.

### Important Rule

Core runtime state must not be fragmented across unrelated MonoBehaviours or hidden inside services.

---

## Main Architectural Components

### RoundResolver

`RoundResolver` is the rule engine for one round.

It is responsible for:

* resolving the fixed round pipeline
* updating round-local battle values
* producing `RoundResult`
* generating logs and per-phase snapshots

It is not responsible for:

* battle flow progression
* run flow progression
* reward generation
* UI interaction

---

### BattleService

`BattleService` orchestrates one battle.

It is responsible for:

* starting a battle
* preparing enemy sequence data
* validating player card selection
* invoking `RoundResolver`
* applying round results to battle-related runtime state
* advancing battle-level flow
* producing `BattleOutcome`

It is not responsible for:

* run-level progression
* enemy switching
* reward entry decisions
* gameplay rule calculation already handled by `RoundResolver`

---

### RunService

`RunService` orchestrates the full run.

It is responsible for:

* starting a new run
* preparing the current enemy
* starting the next battle when allowed
* accepting and interpreting battle outcomes
* deciding whether to enter rewards, next battle, next enemy, victory, or defeat
* maintaining run-level flow consistency

It is not responsible for:

* per-round gameplay rule calculation
* battle-internal round progression details
* reward option generation internals

---

### RewardService

`RewardService` handles rewards.

It is responsible for:

* generating reward offers
* validating legal reward candidates
* deduplicating reward options by canonical resulting deck state
* applying selected reward options to the player deck

It is not responsible for:

* deciding when rewards happen
* deciding post-reward run progression
* battle flow

---

## Dependency Direction

The intended dependency direction is:

```
Presentation
    -> Application
        -> Domain
```

### Allowed Dependency Pattern

* Presentation may depend on Application and Domain-facing view data.
* Application may depend on Domain.
* Domain should avoid depending on Presentation.
* Domain should avoid unnecessary dependency on Application.
* Presentation should not directly implement core game rules.

---

## State and Service Principles

### State Objects

State objects hold mutable runtime data.

Examples:

* `RunState`
* `BattleState`
* `CardInstance`

### Services

Services apply rules or orchestrate state transitions.

Examples:

* `RunService`
* `BattleService`
* `RewardService`

### Important Principle

Services should be long-lived and mostly stateless.
They should operate on explicit state objects rather than storing hidden game state internally.

---

## Persistent vs Temporary Runtime Data

The architecture separates persistent runtime data from temporary round data.

### Persistent Runtime Data

Examples:

* player HP
* enemy HP
* player deck
* permanent power growth
* battle-used cards
* active lane positions during a battle

### Temporary Round Data

Examples:

* current round power values
* current round damage totals
* board-derived deltas
* slot-by-slot combat output for the current round

### Design Rule

Temporary round computation should live in round-local objects or board-projected state, not in persistent card definitions.

---

## Debug-First Design

The architecture intentionally supports strong debug visibility.

This is why the design includes:

* `RoundResult`
* `SlotCombatResult`
* `PhaseSnapshot`
* round and battle logs

The purpose is to make rule validation easier before polished presentation exists.

Debug visibility is considered a core architectural requirement for this demo phase.

---

## Reward Design Implications for Architecture

Reward generation is not treated as a simple random picker.

The architecture must support:

* a fixed four-option reward offer
* fallback from missing upgrades to extra replace options
* canonical deduplication by resulting deck state
* configuration-driven replacement generation
* current demo support for fixed replacement base power

This makes reward generation a real domain/application concern rather than a UI-only feature.

---

## Testing Strategy Implications

The architecture is designed to support lightweight but high-value tests.

### Main testing focus

* domain rules
* round resolution behavior
* reward legality
* reward deduplication
* battle/run flow smoke validation

### Main testing style

* Edit Mode tests first
* minimal Play Mode smoke tests later
* manual validation through a debug scene

The architecture intentionally favors explicit state and explicit results to make this testing style practical.

---

## Implementation Phases

The recommended implementation order is:

1. Domain enums and state objects
2. `RoundResult`, `SlotCombatResult`, `PhaseSnapshot`
3. `RoundResolver`
4. reward legality and deduplication helpers
5. Edit Mode tests for domain logic
6. `BattleService`
7. `RunService`
8. minimal debug UI
9. smoke tests and iteration

This order reduces early instability and helps surface rule bugs sooner.

---

## What This Architecture Explicitly Avoids

The current architecture intentionally avoids:

* complex event buses
* deep inheritance hierarchies for traits
* highly generic effect systems
* premature data-driven scripting systems
* heavy runtime reflection-based architectures
* UI-driven gameplay logic
* service-owned hidden game state

These may be revisited later if the game grows, but they are not appropriate for the current demo phase.

---

## Current Architectural Boundaries

The following boundaries are considered locked unless explicitly redesigned:

* `RoundResolver` resolves one round but does not control run progression.
* `BattleService` controls one battle but does not decide run-level progression.
* `RunService` controls run progression but does not compute round rules.
* `RewardService` generates and applies rewards but does not decide when rewards occur.
* Presentation displays and forwards input but does not implement gameplay logic.

---

## Future Extension Points

The current architecture leaves room for future changes such as:

* richer enemy behavior
* more reward profiles
* more trait types
* final runtime UI
* save/load support
* content expansion
* improved balancing tools

These future extensions should build on the current layering unless a redesign is explicitly documented.

---

## Summary

This architecture is centered on:

* explicit runtime state
* clear service boundaries
* deterministic rule resolution
* debug visibility
* small, testable components

It is designed to let the team implement the demo quickly without collapsing rule logic, flow orchestration, and UI into one layer.