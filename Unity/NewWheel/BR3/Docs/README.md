# Docs README

## Purpose

This directory contains the canonical design and implementation documents for the Unity 6.4 2D turn-based card roguelike demo.

These documents are the primary source of truth for:

* locked gameplay rules
* software architecture
* domain model design
* battle and run flow
* reward generation rules
* config and content structure
* debug UI planning
* testing strategy

Use these documents to keep design discussion, Codex implementation, and repository code aligned.

---

## Document Categories

### Game Design

Documents in `Docs/game-design/` describe gameplay-facing rules and content intent.

Recommended files:

* `game-rules-locked.md`
* `trait-list.md`
* `enemy-and-reward-rules.md`

These documents define what the game should do.

---

### Game Design Tasks

Documents in `Docs/game-design/tasks/` store deferred gameplay ideas, design incubator notes, and temporary gameplay proposals that are not part of the current canonical rules.

Recommended files:

* `deferred-gameplay-ideas.md`

These documents are not current source-of-truth gameplay rules unless explicitly promoted.

---

### GDR

Documents in `Docs/game-design/gdr/` record important gameplay design decisions and their rationale.

Examples:

* `GDR-0001-movement-processed-right-to-left.md`
* `GDR-0002-start-with-six-card-deck.md`
* `GDR-0003-enemy-rewards-settle-early-on-defeat.md`

Use GDRs to preserve the reasoning behind non-obvious gameplay rule decisions.

### Engineering

Documents in `Docs/engineering/` describe the software architecture and implementation design.

Current and planned files:

* `architecture-overview.md`
* `domain-model.md`
* `round-resolution.md`
* `run-battle-reward-flow.md`
* `reward-generation.md`
* `config-and-content.md`
* `debug-ui-plan.md`
* `testing-strategy.md`

These documents define how the game should be implemented.

---

### ADR

Documents in `Docs/adr/` record important architectural decisions.

Current files:

* `ADR-0001-layering-and-service-boundaries.md`
* `ADR-0002-json-config-over-scriptableobject.md`
* `ADR-0003-reward-dedup-by-canonical-deck.md`

Use ADRs to preserve the reasoning behind non-obvious design decisions.

---

### Tasks

Documents in `Docs/tasks/` are temporary planning documents for current implementation work.

Examples:

* `current-implementation-plan.md`
* `domain-v1-task-list.md`

These documents are not long-term source-of-truth documents unless explicitly promoted.

---

## Reading Order

### For a new engineer or coding agent

Read in this order:

1. `../AGENTS.md`
2. `Docs/game-design/game-rules-locked.md`
3. `Docs/engineering/architecture-overview.md`
4. `Docs/engineering/config-and-content.md`
5. `Docs/engineering/domain-model.md`
6. `Docs/engineering/round-resolution.md`
7. `Docs/engineering/run-battle-reward-flow.md`
8. `Docs/engineering/reward-generation.md`
9. `Docs/engineering/testing-strategy.md`

---

### For gameplay clarification

Read:

1. `Docs/game-design/game-rules-locked.md`
2. `Docs/game-design/trait-list.md`
3. `Docs/game-design/enemy-and-reward-rules.md`

---

### For gameplay decision history

Read:

1. `Docs/game-design/game-rules-locked.md`
2. `Docs/game-design/trait-list.md`
3. `Docs/game-design/enemy-and-reward-rules.md`
4. relevant files in `Docs/game-design/gdr/`

---

### For implementation work on config and runtime construction

Read:

1. `Docs/engineering/config-and-content.md`
2. `Docs/engineering/domain-model.md`
3. `Docs/adr/ADR-0002-json-config-over-scriptableobject.md`

---

### For implementation work on domain logic

Read:

1. `Docs/engineering/domain-model.md`
2. `Docs/engineering/round-resolution.md`
3. `Docs/engineering/reward-generation.md`
4. `Docs/adr/ADR-0003-reward-dedup-by-canonical-deck.md`
5. `Docs/engineering/testing-strategy.md`

---

### For implementation work on application flow

Read:

1. `Docs/engineering/architecture-overview.md`
2. `Docs/engineering/run-battle-reward-flow.md`
3. `Docs/engineering/domain-model.md`
4. `Docs/adr/ADR-0001-layering-and-service-boundaries.md`

---

### For implementation work on debug UI

Read:

1. `Docs/engineering/debug-ui-plan.md`
2. `Docs/engineering/run-battle-reward-flow.md`
3. `Docs/engineering/domain-model.md`

---

## Canonical Document Rules

The following documents should be treated as canonical sources.

### Gameplay

* `Docs/game-design/game-rules-locked.md`
* `Docs/game-design/trait-list.md`
* `Docs/game-design/enemy-and-reward-rules.md`

### Engineering

* `Docs/engineering/architecture-overview.md`
* `Docs/engineering/config-and-content.md`
* `Docs/engineering/domain-model.md`
* `Docs/engineering/round-resolution.md`
* `Docs/engineering/run-battle-reward-flow.md`
* `Docs/engineering/reward-generation.md`

If a temporary task document conflicts with a canonical document, the canonical document wins.

If an engineering document appears to conflict with a locked gameplay rule, do not resolve the conflict silently. Report it and update the documents explicitly.

---

## Scope of Current Phase

The current development phase prioritizes:

1. config and runtime construction clarity
2. domain correctness
3. application flow clarity
4. debug-oriented presentation
5. lightweight but high-value tests

The current goal is not final production polish.

---

## Current Architecture Summary

The system is divided into three layers:

### Domain

Contains:

* core runtime state objects
* value objects
* result objects
* rule logic
* canonical signature logic
* reward legality and deduplication

### Application

Contains:

* run orchestration
* battle orchestration
* reward flow orchestration
* command handling
* state transitions
* runtime construction delegation

### Presentation

Contains:

* debug UI
* state display
* input forwarding
* view models
* presentation-only formatting
* thin Unity-facing bootstrapping and config loading entry points

---

## Main Architectural Components

### Config and construction

* `GameConfigLoader`
* `RuntimeStateFactory`

### Domain-focused

* `RoundResolver`
* runtime state objects such as `RunState`, `BattleState`, `CardInstance`, `BoardCard`
* config-side objects such as `GameConfig`, `EnemyConfig`, `CardSpec`
* result objects such as `RoundResult`, `SlotCombatResult`, `PhaseSnapshot`, `BattleOutcome`

### Application-focused

* `RunService`
* `BattleService`
* `RewardService`
* application command result objects such as `RunCommandResult` and `BattleCommandResult`

### Presentation-focused

* debug scene
* debug UI controllers
* view models
* display formatting helpers

---

## Documentation Maintenance Rules

* Keep implementation-facing documents in English.
* Gameplay-facing documents under `Docs/game-design/` and `Docs/game-design/tasks/` may be written in Chinese.
* Keep file names and implementation-facing terminology stable in English where useful.
* Update canonical documents when design decisions change.
* Do not leave important rules only in chat history.
* Prefer updating an existing canonical document instead of creating many overlapping documents.
* Use ADRs for engineering decisions that need long-term traceability.
* Use GDRs for gameplay decisions that need long-term traceability.
* Keep task documents small and disposable.
* Keep gameplay rules, gameplay decision records, engineering structure, and architectural rationale in separate documents when possible.

---

## Recommended Implementation Workflow

1. Discuss and lock design decisions.
2. Update canonical documents.
3. Let Codex implement a small scoped task.
4. Validate with tests and debug tooling.
5. Review the result against the docs.
6. Update docs again if a real design change is approved.

Do not let implementation drift away from the documents.

---

## Recommended First Implementation Targets

The safest first implementation targets are:

1. config objects and config loading boundary
2. domain enums and state objects
3. round result objects
4. round resolution skeleton
5. reward legality and deduplication helpers
6. Edit Mode tests for domain rules

Application flow and debug UI should follow after the domain core is stable enough.

---

## Stable vs Changeable Documents

### Stable documents

These should change only when the accepted design changes:

* locked gameplay rules
* gameplay trait list
* enemy and reward rules
* gameplay decision records (GDRs)
* architecture overview
* config and content
* domain model
* round resolution
* run / battle / reward flow
* reward generation
* ADRs

### More changeable documents

These may evolve more frequently:

* gameplay task notes
* gameplay incubator notes
* implementation task plans
* implementation sequence notes
* migration notes
* temporary rollout notes

Keep stable documents authoritative and avoid polluting them with short-lived implementation details.

---

## Current Important ADRs

### ADR-0001

Defines the layering model and service boundaries:

* Domain
* Application
* Presentation
* `RoundResolver`
* `BattleService`
* `RunService`
* `RewardService`

### ADR-0002

Explains why the current demo uses JSON as the primary authored config format instead of ScriptableObject.

### ADR-0003

Explains why reward options are deduplicated by canonical resulting deck state rather than raw action parameters.

These ADRs are part of the current architecture and should be read when the relevant area is being implemented or changed.

---

## Future Expansion

The current docs are written for a demo-sized implementation.

Later phases may add:

* more enemies
* more reward profiles
* richer debug tooling
* final runtime UI
* additional tests
* save/load support
* stronger editor tooling if needed

Future work should extend the existing architecture unless a deliberate redesign is approved and documented.