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
* `testing-strategy.md`
* `debug-ui-plan.md`

---

## Design Goals

The current architecture is optimized for:

1. gameplay rule correctness
2. clean separation of responsibilities
3. authoritative and internally consistent runtime state
4. deterministic outcome classification
5. fast iteration during early development
6. debug visibility
7. easy verification through lightweight tests
8. future expansion without heavy refactoring

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
* raw round consequence calculation
* explicit battle-completion reason representation
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
* `BattleCompletionReason`

### Domain Principles

* Domain logic should be as Unity-agnostic as practical.
* State should live in state objects, not in services.
* Core rule logic should be explicit and testable.
* Temporary round computation should not pollute persistent runtime state.
* Authored config types are not the same thing as runtime state types.
* Raw rule consequences and authoritative applied values must remain distinguishable.
* Gameplay outcome must be represented explicitly rather than inferred independently by multiple layers.

---

## Layer 2: Application

### Responsibilities

The Application layer contains:

* run-level flow orchestration
* battle-level flow orchestration
* reward flow orchestration
* authoritative HP application
* battle-completion classification
* completed-battle handoff
* reward eligibility and terminal run progression
* command handling
* state transitions
* coordination between domain objects and presentation
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
* Authoritative HP mutation must occur in one application-layer owner.
* A fixed battle outcome must not be recalculated during presentation.
* Run-level reward and terminal progression must consume the official battle outcome rather than reconstructing it from HP.

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
* Presentation may retain finalized result objects for inspection after authoritative gameplay state has advanced.
* Presentation-only retention is observational and must not become a second gameplay-state source.
* Presentation must not classify player death, enemy defeat, simultaneous-zero priority, reward eligibility, victory, or defeat.
* Continue is a presentation gate, not an outcome-decision command.

---

## Authored Config vs Runtime State

The project separates authored config from runtime state.

### Authored config

Authored config represents content known before runtime.

Examples:

* player max HP
* player starting deck specs
* enemy max HP
* each enemy's battle limit
* enemy fixed decks
* reward offer structure rules
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

## Battle Completion State

A completed battle outcome may be fixed before the active battle is handed back to `RunService`.

The relevant battle-scoped state is:

```
BattleState
├── RoundResults
├── BattleFlowStage
└── PendingBattleOutcome : BattleOutcome?
```

### PendingBattleOutcome

`PendingBattleOutcome` is the authoritative completed-battle result while:

* the final round result is being presented
* the battle has moved to `BattleComplete`
* run-level acceptance has not yet consumed the active battle

The run remains in:

* `RunFlowStage.InBattle`

until `RunService.AcceptCompletedBattle(...)` accepts the completed battle.

### Important rules

* the pending outcome is fixed from final authoritative HP
* Presentation may inspect it but must not modify or recreate it
* `FinishRoundPresentation(...)` does not clear it
* run-level acceptance clears the active battle after capturing the authoritative outcome
* no second mutable outcome source should coexist with it

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

* executing the complete fixed seven-phase pipeline
* updating round-local battle values
* resolving slot combat
* calculating raw merged damage
* calculating raw post-resolve healing
* applying persistent non-HP post-resolve effects such as `Growth`
* creating the non-finalized `RoundResult`
* generating logs and per-phase snapshots

It is not responsible for:

* authoritatively mutating player or enemy HP
* applying max-HP clamp
* establishing final authoritative HP
* classifying player death or enemy defeat
* classifying battle completion
* battle flow progression
* run flow progression
* reward generation
* UI interaction

### Important rule

All seven phases complete even when projected or intermediate HP reaches zero or below.

Player death, enemy defeat, and battle completion are classified after battle-layer HP application.

They do not form an eighth `RoundPhase`.

---

### BattleService

`BattleService` orchestrates one battle.

It is responsible for:

* starting a battle
* preparing enemy sequence data
* validating player card selection
* invoking `RoundResolver`
* applying raw merged damage to authoritative HP
* recording HP after merged damage
* applying raw post-resolve healing
* clamping healing to max HP
* recording actual healing applied
* establishing final authoritative HP
* finalizing the authoritative HP fields in `RoundResult`
* adding the finalized result to battle history
* classifying one official `BattleCompletionReason`
* creating and storing `BattleState.PendingBattleOutcome`
* advancing battle-level flow
* exposing the completed outcome after round-result presentation

It is not responsible for:

* deciding run-level victory or defeat
* selecting the next enemy
* deciding reward eligibility
* generating reward offers
* recalculating raw rules already handled by `RoundResolver`

### Completion priority

After final HP is established, classification is:

```
if final player HP <= 0
    PlayerDefeated
else if final enemy HP <= 0
    EnemyDefeated
else if the final round completed
    AllRoundsCompleted
else
    continue the battle
```

Simultaneous zero is officially:

* `BattleCompletionReason.PlayerDefeated`

### Presentation gate

`SubmitPlayerCard(...)` fixes the outcome before entering `PresentingRoundResult`.

`FinishRoundPresentation(...)` only advances the presentation gate and exposes the already-fixed outcome.

---

### RunService

`RunService` orchestrates the full run.

It is responsible for:

* starting a new run
* determining when the next battle may begin
* accepting the authoritative completed battle outcome
* incrementing completed-battle progression exactly once
* clearing completed active-battle state
* interpreting official battle-completion reason
* deciding reward eligibility
* deciding next battle or next enemy progression
* deciding victory or defeat
* maintaining run-level flow consistency

It is not responsible for:

* per-round gameplay rule calculation
* authoritative round HP application
* battle-internal round transitions
* reward option generation internals
* reconstructing official battle outcome from displayed or raw HP

### Official branch order

Run-level interpretation must use this order:

1. `BattleCompletionReason.PlayerDefeated`
2. `BattleCompletionReason.EnemyDefeated`
3. `BattleCompletionReason.AllRoundsCompleted` with the battle limit exhausted
4. `BattleCompletionReason.AllRoundsCompleted` with more battles remaining

### Player-death rule

When the official reason is `PlayerDefeated`:

* the run enters `Defeat`
* the active battle is cleared
* no reward offer is generated
* remaining enemy rewards are not settled
* reward progress does not increase
* no next enemy or `Victory` branch is entered

This branch also handles simultaneous zero.

### Important note

`RunService.CreateNewRun(...)` remains the run-level entry point for starting a run, but it delegates initial runtime graph construction to `RuntimeStateFactory`.

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
* deciding whether a completed battle is reward-eligible
* handling player-death logic
* interpreting simultaneous zero
* deciding battle-limit defeat
* deciding post-reward run progression
* battle flow

### Reward-call boundary

`RunService` controls whether `RewardService` is called.

For:

* player death
* simultaneous zero resolved as player death
* battle-limit exhaustion
* terminal victory or defeat

reward generation is prevented before entering `RewardService`.

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

## HP Application and Outcome Ownership

The architecture separates four consecutive responsibilities.

### 1. Rule resolution

Owned by:

* `RoundResolver`

Produces:

* raw merged damage
* raw healing
* slot results
* logs
* snapshots
* persistent non-HP effects

### 2. Authoritative application

Owned by:

* `BattleService`

Performs:

```
HP before
→ merged damage
→ HP after merged damage
→ raw healing
→ max-HP clamp
→ actual healing
→ final HP
```

### 3. Battle-outcome classification

Owned by:

* `BattleService`

Produces one official:

* `BattleCompletionReason`

and stores one authoritative:

* `PendingBattleOutcome`

### 4. Run-level interpretation

Owned by:

* `RunService`

Decides:

* reward flow
* next battle
* next enemy
* victory
* defeat

### Architectural invariant

No two layers may independently calculate competing final HP or official battle outcome.

The intended chain is:

```
RoundResolver computes consequences
→ BattleService applies consequences once
→ BattleService establishes final HP once
→ BattleService classifies outcome once
→ Presentation displays the fixed result
→ RunService interprets the fixed result once
```

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

### Outcome timing across flow stages

A completed battle outcome may already exist while:

* `RunFlowStage == InBattle`
* `BattleFlowStage == PresentingRoundResult`

This is intentional.

During this state:

* final HP is already authoritative
* the official completion reason is already fixed
* Presentation may display the result
* Continue cannot change the result

After presentation finishes:

* `BattleFlowStage` becomes `BattleComplete`
* the same pending outcome remains authoritative
* `RunService` accepts and interprets it

### Terminal stages

After entering:

* `Victory`
* `Defeat`

the run has:

* no active battle
* no pending reward offer
* no legal in-run gameplay action

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

### RoundResult lifecycle

`RoundResult` has two lifecycle points:

1. resolver-created, containing raw rule consequences
2. battle-finalized, containing authoritative HP application values

Only the battle-finalized result may be treated as the complete authoritative round result.

### BattleOutcome identity

`BattleOutcome` records:

* battle index
* rounds played
* one `BattleCompletionReason`
* final player HP
* final enemy HP

The completion reason is the official result.

Final HP values provide supporting authoritative data but do not create a second independently interpreted outcome.

### Command failure distinction

`BattleCommandResult` and `RunCommandResult` describe command execution.

Player defeat is a successful gameplay outcome, not a command failure.

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

The debug architecture must also support:

* HP before merged damage
* HP after merged damage
* raw healing
* actual healing after clamp
* final HP
* fixed battle-completion reason
* simultaneous-zero explanation
* retained visibility of the latest fatal round

### Presentation retention

`DebugUiState` may retain references to:

* the latest finalized `RoundResult`
* the latest completed `BattleOutcome`

This allows the debug inspector to remain populated after `RunService` clears `RunState.ActiveBattle`.

These references are presentation-only observation state.

They must not be mutated or treated as authoritative gameplay state.

---

## Reward Design Implications for Architecture

Reward generation is not treated as a simple random picker.

The architecture must support:

* exactly one `Skip` in every reward offer
* config-driven total offer size
* config-driven upgrade target
* replace count derived from remaining non-skip capacity after upgrade selection
* canonical deduplication by resulting deck state
* configuration-driven replacement generation
* current demo support for an allowed replacement base-power set, which may still contain only one value in the default baseline

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

The architecture is designed to support lightweight but high-value tests at the layer that owns each responsibility.

### Resolver tests

Verify:

* complete seven-phase execution
* raw damage and healing consequences
* Post Resolve after projected lethal damage
* no authoritative HP mutation
* no survival classification

### BattleService tests

Verify:

* authoritative HP application
* HP after merged damage
* healing clamp and actual healing
* final HP
* temporary-zero recovery
* final HP exactly zero
* player-death and enemy-defeat classification
* simultaneous-zero priority
* pending-outcome lifecycle
* presentation-gate stability

### RunService tests

Verify:

* authoritative completed-battle acceptance
* progression counters change exactly once
* player-death priority
* reward short circuit
* simultaneous-zero defeat
* enemy-defeat progression
* battle-limit defeat
* terminal-state cleanup

### Presentation and smoke validation

Verify:

* finalized result formatting
* terminal status text
* button gating
* retained fatal-round visibility
* controller handoff wiring

### Main testing style

* Edit Mode tests first
* minimal Play Mode smoke tests later
* manual validation through the debug scene

The architecture favors explicit state, explicit results, and narrow ownership so each layer can be tested without duplicating another layer's rule logic.

---

## Implementation Phases

The recommended implementation and change order is:

1. config objects and loading boundary
2. domain enums and state objects
3. result objects and explicit outcome representation
4. `RoundResolver` raw consequence behavior
5. reward legality and deduplication helpers
6. Edit Mode tests for domain logic
7. `BattleService` authoritative application and outcome classification
8. `RunService` outcome interpretation and reward eligibility
9. debug-oriented presentation and retained result inspection
10. smoke tests and manual iteration

For cross-layer rule changes, update the canonical engineering documents before implementation tasks are generated.

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

* `RoundResolver` completes all seven rule phases and produces raw consequences, but does not authoritatively mutate HP or classify survival.
* `BattleService` applies round consequences once, establishes final HP once, and classifies one official battle outcome.
* `BattleState.PendingBattleOutcome` remains authoritative through round-result presentation and battle-complete handoff.
* `FinishRoundPresentation(...)` does not calculate or alter gameplay outcome.
* `RunService` interprets the official battle outcome and owns reward eligibility, next-battle, next-enemy, victory, and defeat progression.
* `RunService` interprets `PlayerDefeated` before enemy-defeat or reward progression.
* `RewardService` generates and applies reward content but does not decide whether rewards occur.
* `GameConfigLoader` loads and validates authored config but does not create gameplay flow.
* `RuntimeStateFactory` constructs runtime objects but does not orchestrate runtime flow.
* Presentation displays authoritative state, retains finalized results for inspection, and forwards commands, but does not implement gameplay outcome rules.
* Player death, enemy defeat, and battle completion do not introduce an eighth `RoundPhase`.
* No new config field or flow stage is required for the player-death rule.

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
* explicit authoritative runtime state
* clear Domain, Application, and Presentation boundaries
* complete deterministic seven-phase rule resolution
* single-owner authoritative HP application
* one fixed battle-completion reason
* player-death-first run interpretation
* reward eligibility controlled by `RunService`
* retained debug visibility through terminal transitions
* small, testable components

The core execution chain is:

```
RoundResolver computes raw consequences
→ BattleService applies and finalizes the round
→ BattleService fixes the battle outcome
→ Presentation displays the fixed result
→ RunService interprets run progression
```

This structure allows the project to add player-death failure behavior without moving gameplay logic into Presentation, duplicating HP calculation, adding an eighth round phase, or coupling reward generation to death handling.