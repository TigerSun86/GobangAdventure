# Architecture Overview

## Purpose

This document describes the high-level software architecture for the Unity 6.4 2D turn-based card roguelike demo.

It explains:

* the main architectural layers
* the core modules and their responsibilities
* the main runtime state objects
* the direction of dependencies
* the role of authored config
* the implementation priorities for the current demo phase

This document is a high-level engineering reference. More detailed behavior is specified in:

* `domain-model.md`
* `round-resolution.md`
* `run-battle-reward-flow.md`
* `reward-generation.md`
* `config-and-content.md`

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

The project also separates:

* authored config
* runtime state
* runtime orchestration

This separation is part of the architecture, not an implementation accident.

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
* `BattleOutcome`
* `RoundResolver`

### Domain Principles

* Domain logic should be as Unity-agnostic as practical.
* State should live in state objects, not in services.
* Core rule logic should be explicit and testable.
* Temporary round computation should not pollute persistent runtime state.
* Authored config types are not the same thing as runtime state types.

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
* runtime construction delegation through helpers such as factories

### Examples

Typical Application objects include:

* `RunService`
* `BattleService`
* `RewardService`
* `RuntimeStateFactory`
* controller-style entry points that translate UI input into application commands
* application-level result objects such as `RunCommandResult` and `BattleCommandResult`

### Application Principles

* Application code should orchestrate, not reimplement domain rules.
* Application flow should be explicit and state-driven.
* The layer should coordinate existing state objects rather than hide them behind unnecessary abstractions.
* Construction helpers and flow orchestrators are distinct concepts.

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
* thin Unity-facing config loading entry points

### Examples

Typical Presentation objects include:

* debug scene components
* debug UI controllers
* view model builders
* UI formatting helpers
* a bootstrap MonoBehaviour that receives a `TextAsset` and asks the config loader to deserialize it

### Presentation Principles

* Presentation must not contain gameplay rule logic.
* UI should read state and send commands.
* The first implementation target is a debug-oriented text-heavy UI, not a polished final UI.
* Unity-specific concerns should stay near the edge of the system.

---

## Authored Config vs Runtime State

The project separates authored config from runtime state.

### Authored config

Authored config represents content known before runtime.

Examples:

* player max HP
* player starting deck specs
* enemy max HP
* enemy fixed decks
* replacement generation rules
* trait tuning values

### Runtime state

Runtime state represents what is currently true during a run.

Examples:

* current player HP
* current player max HP
* current enemy current HP
* current enemy max HP
* active battle state
* pending reward offer
* current battle board positions
* permanent power growth accumulated during the run

### Design Rule

Runtime systems should not repeatedly consult authored config to know live gameplay state such as max HP.
Authored config should be converted into runtime state at initialization boundaries.

---

## Core Runtime State Tree

The runtime state is organized around a single run-level root.

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

This state tree is the authoritative source of the current run.

### Important Rule

Core runtime state must not be fragmented across unrelated MonoBehaviours or hidden inside services.

---

## Authored Config Model

The authored config model is JSON-first.

The main config objects are:

* `GameConfig`
* `PlayerStartConfig`
* `EnemyConfig`
* `RewardGenerationConfig`
* `TraitTuning`
* `CardSpec`

The config pipeline is:

1. authored JSON
2. pure C# config objects
3. validation through a thin loader
4. explicit conversion into runtime state

More detail is defined in `config-and-content.md`.

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
* clamping healing to max HP when applying round results
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
* determining when the next battle may begin
* accepting and interpreting battle outcomes
* deciding whether to enter rewards, next battle, next enemy, victory, or defeat
* maintaining run-level flow consistency

It is not responsible for:

* per-round gameplay rule calculation
* battle-internal round progression details
* reward option generation internals

### Important note

`RunService.CreateNewRun(...)` is the run-level entry point for starting a run, but it should delegate initial runtime graph construction to `RuntimeStateFactory`.

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

### GameConfigLoader

`GameConfigLoader` is the thin config loading adapter.

It is responsible for:

* receiving JSON text from a Unity-facing source such as `TextAsset`
* deserializing into `GameConfig`
* validating the authored config

It is not responsible for:

* gameplay logic
* run progression
* battle progression
* reward generation
* runtime orchestration

---

### RuntimeStateFactory

`RuntimeStateFactory` is a construction helper.

It is responsible for:

* creating `RunState` from `GameConfig`
* creating `EnemyProgressState` from `EnemyConfig`
* creating `CardInstance` from `CardSpec`

It is not responsible for:

* reading JSON
* owning run progression
* owning battle progression
* owning reward progression

### Important distinction

The factory constructs runtime objects.
Services orchestrate runtime flow.

---

## Flow Stages

The architecture uses explicit flow stages at run level and battle level.

### RunFlowStage

The accepted run flow stages are:

* `ReadyForNextBattle`
* `InBattle`
* `ChoosingReward`
* `Victory`
* `Defeat`

### BattleFlowStage

The accepted battle flow stages are:

* `WaitingForPlayerCard`
* `ResolvingRound`
* `PresentingRoundResult`
* `BattleComplete`

### Why this matters

Run flow and battle flow operate at different scopes and should not be collapsed into a single state machine.

---

## Application Result Objects

The architecture distinguishes between:

* gameplay result objects
* application command result objects

### Gameplay result objects

These describe gameplay facts:

* `RoundResult`
* `BattleOutcome`

### Application command result objects

These describe service command execution:

* `BattleCommandResult`
* `RunCommandResult`

### Why this distinction exists

A gameplay result is not the same thing as a service command result.
Keeping both concepts explicit improves readability and keeps orchestration easier to inspect.

---

## Dependency Direction

The intended dependency direction is:

```
Presentation
    -> Application
        -> Domain
```

Authored config is loaded at the Unity-facing edge and then converted into pure config objects and runtime state.

### Allowed Dependency Pattern

* Presentation may depend on Application and Domain-facing view data.
* Presentation may host thin Unity-specific adapters such as JSON `TextAsset` bootstrap wiring.
* Application may depend on Domain.
* Application may depend on authored config objects and construction helpers where needed.
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
* `EnemyProgressState`

### Services

Services apply rules or orchestrate state transitions.

Examples:

* `RunService`
* `BattleService`
* `RewardService`

### Construction Helpers

Construction helpers build object graphs from config.

Examples:

* `RuntimeStateFactory`

### Important Principle

Services should be long-lived and mostly stateless.
They should operate on explicit state objects rather than storing hidden game state internally.

---

## Persistent vs Temporary Runtime Data

The architecture separates persistent runtime data from temporary round data.

### Persistent Runtime Data

Examples:

* player current HP
* player max HP
* enemy current HP
* enemy max HP
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
* explicit flow stages
* explicit application command results

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

## Config Design Implications for Architecture

The authored config system is intentionally lightweight.

The architecture must support:

* JSON-authored config
* pure C# config objects
* simple serializer-friendly config schemas
* trait tuning values in config
* trait behavior remaining code-driven
* loader isolation so serializer replacement remains possible later

This avoids overengineering the authored content system too early.

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

1. config objects and loading boundary
2. domain enums and state objects
3. `RoundResult`, `SlotCombatResult`, `PhaseSnapshot`
4. `RoundResolver`
5. reward legality and deduplication helpers
6. Edit Mode tests for domain logic
7. `BattleService`
8. `RunService`
9. minimal debug UI
10. smoke tests and iteration

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
* ScriptableObject as the primary authored gameplay content source for the current demo
* config-driven trait behavior systems in the first implementation phase

These may be revisited later if the game grows, but they are not appropriate for the current demo phase.

---

## Current Architectural Boundaries

The following boundaries are considered locked unless explicitly redesigned:

* `RoundResolver` resolves one round but does not control run progression.
* `BattleService` controls one battle but does not decide run-level progression.
* `RunService` controls run progression but does not compute round rules.
* `RewardService` generates and applies rewards but does not decide when rewards occur.
* `GameConfigLoader` loads and validates authored config but does not create gameplay flow.
* `RuntimeStateFactory` constructs runtime objects but does not orchestrate runtime flow.
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
* a future serializer upgrade if config complexity grows
* future Unity-native editor tooling if the project later needs it

These future extensions should build on the current layering unless a redesign is explicitly documented.

---

## Summary

This architecture is centered on:

* explicit authored config
* explicit runtime state
* clear service boundaries
* deterministic rule resolution
* debug visibility
* small, testable components

It is designed to let the team implement the demo quickly without collapsing config, rule logic, flow orchestration, and UI into one layer.