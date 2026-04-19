# Docs README

## Purpose

This directory contains the canonical design and implementation documents for the Unity 6.4 2D turn-based card roguelike demo.

These documents are the primary source of truth for:
- locked gameplay rules
- software architecture
- domain model design
- battle and run flow
- reward generation rules
- debug UI planning
- testing strategy

Use these documents to keep ChatGPT discussions, Codex implementation, and repository code aligned.

---

## Document Categories

### Game Design
Documents in `Docs/game-design/` describe gameplay-facing rules and content intent.

Recommended files:
- `game-rules-locked.md`
- `trait-list.md`
- `enemy-and-reward-rules.md`

These documents define what the game should do.

---

### Engineering
Documents in `Docs/engineering/` describe the software architecture and implementation design.

Recommended files:
- `architecture-overview.md`
- `domain-model.md`
- `round-resolution.md`
- `run-battle-reward-flow.md`
- `reward-generation.md`
- `debug-ui-plan.md`
- `testing-strategy.md`
- `config-and-content.md`

These documents define how the game should be implemented.

---

### ADR
Documents in `Docs/adr/` record important architectural decisions.

Recommended files:
- `ADR-0001-layering-and-service-boundaries.md`
- `ADR-0002-reward-dedup-by-canonical-deck.md`

Use ADRs to preserve design intent behind non-obvious decisions.

---

### Tasks
Documents in `Docs/tasks/` are temporary planning documents for current implementation work.

Examples:
- `current-implementation-plan.md`
- `domain-v1-task-list.md`

These documents are not long-term source-of-truth documents unless explicitly promoted.

---

## Reading Order

### For a new engineer or coding agent
Read in this order:

1. `../AGENTS.md`
2. `Docs/game-design/game-rules-locked.md`
3. `Docs/engineering/architecture-overview.md`
4. `Docs/engineering/domain-model.md`
5. `Docs/engineering/round-resolution.md`
6. `Docs/engineering/run-battle-reward-flow.md`
7. `Docs/engineering/reward-generation.md`
8. `Docs/engineering/testing-strategy.md`

### For gameplay clarification
Read:
1. `Docs/game-design/game-rules-locked.md`
2. `Docs/game-design/trait-list.md`
3. `Docs/game-design/enemy-and-reward-rules.md`

### For implementation work on domain logic
Read:
1. `Docs/engineering/domain-model.md`
2. `Docs/engineering/round-resolution.md`
3. `Docs/engineering/reward-generation.md`
4. `Docs/engineering/testing-strategy.md`

### For implementation work on application flow
Read:
1. `Docs/engineering/architecture-overview.md`
2. `Docs/engineering/run-battle-reward-flow.md`
3. `Docs/engineering/domain-model.md`

### For implementation work on debug UI
Read:
1. `Docs/engineering/debug-ui-plan.md`
2. `Docs/engineering/run-battle-reward-flow.md`
3. `Docs/engineering/domain-model.md`

---

## Canonical Document Rules

The following documents should be treated as canonical sources:

### Gameplay
- `Docs/game-design/game-rules-locked.md`
- `Docs/game-design/trait-list.md`
- `Docs/game-design/enemy-and-reward-rules.md`

### Engineering
- `Docs/engineering/architecture-overview.md`
- `Docs/engineering/domain-model.md`
- `Docs/engineering/round-resolution.md`
- `Docs/engineering/run-battle-reward-flow.md`
- `Docs/engineering/reward-generation.md`

If a temporary task document conflicts with a canonical document, the canonical document wins.

If an engineering document appears to conflict with a locked gameplay rule, do not resolve the conflict silently. Report it and update the documents explicitly.

---

## Scope of Current Phase

The current development phase prioritizes:

1. Domain correctness
2. Application flow clarity
3. Debug-oriented presentation
4. Lightweight but high-value tests

The current goal is not final production polish.

---

## Current Architecture Summary

The system is divided into three layers:

### Domain
Contains:
- core state objects
- value objects
- result objects
- rule logic
- canonical signature logic
- reward legality and deduplication

### Application
Contains:
- run orchestration
- battle orchestration
- reward flow orchestration
- command handling
- state transitions

### Presentation
Contains:
- debug UI
- state display
- input forwarding
- view models
- presentation-only formatting

---

## Main Architectural Components

### Domain-focused
- `RoundResolver`
- runtime state objects such as `RunState`, `BattleState`, `CardInstance`, `BoardCard`
- result objects such as `RoundResult`, `SlotCombatResult`, `PhaseSnapshot`

### Application-focused
- `RunService`
- `BattleService`
- `RewardService`

### Presentation-focused
- debug scene
- debug UI controllers
- view models
- display formatting helpers

---

## Documentation Maintenance Rules

- Keep design documents in English.
- Update canonical documents when design decisions change.
- Do not leave important rules only in chat history.
- Prefer updating an existing canonical document instead of creating many overlapping documents.
- Use ADRs for important design decisions that need long-term traceability.
- Keep task documents small and disposable.

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

1. domain enums and state objects
2. round result objects
3. round resolution skeleton
4. reward legality and deduplication helpers
5. Edit Mode tests for domain rules

Application flow and debug UI should follow after the domain core is stable enough.

---

## Notes on Temporary vs Stable Design

Stable documents:
- locked gameplay rules
- architecture overview
- domain model
- round resolution
- run / battle / reward flow
- reward generation

More changeable documents:
- task plans
- implementation sequence notes
- temporary migration notes

Keep stable documents authoritative and avoid polluting them with short-lived implementation details.

---

## Future Expansion

The current docs are written for a demo-sized implementation.

Later phases may add:
- more enemies
- more reward profiles
- richer debug tooling
- final runtime UI
- additional tests
- save/load support

Future work should extend the existing architecture unless a deliberate redesign is approved and documented.