# AGENTS.md

## Project Purpose

This repository contains a Unity 6.4 2D turn-based card roguelike demo.

The current implementation priority is:

1. Domain core
2. Application flow
3. Debug-oriented presentation
4. Polish and refactoring

The goal of the current phase is correctness, clarity, and fast iteration, not final production polish.

---

## Hard Constraints

- Use English only in code, comments, identifiers, logs, strings, and documentation written for implementation.
- Do not change locked gameplay rules unless the user explicitly asks for a design change.
- Prefer small, reviewable changes.
- Keep behavior deterministic where possible.
- Do not introduce unnecessary abstractions.
- Do not add speculative systems that are not required by the current design.

---

## Authoritative Documents

Always read these files before making changes in related areas:

- `Docs/README.md`
- `Docs/game-design/game-rules-locked.md`
- `Docs/engineering/architecture-overview.md`
- `Docs/engineering/domain-model.md`
- `Docs/engineering/round-resolution.md`
- `Docs/engineering/run-battle-reward-flow.md`
- `Docs/engineering/reward-generation.md`
- `Docs/engineering/testing-strategy.md`

If two documents appear to conflict, prefer the more specific engineering document for implementation details, but never override locked gameplay rules silently. Report the conflict clearly.

---

## Layering Rules

### Domain
Domain code should contain:
- core state objects
- value objects
- enums
- pure or mostly pure rules logic
- reward legality and deduplication logic
- canonical signature logic
- battle and round result objects

Domain code should avoid unnecessary Unity-specific dependencies whenever practical.

### Application
Application code should contain:
- run-level orchestration
- battle-level orchestration
- reward flow orchestration
- command handling
- state transitions
- coordination between domain logic and UI

Application code should not reimplement domain rules.

### Presentation
Presentation code should contain:
- debug UI
- state display
- input forwarding
- view models
- presentation-only formatting

Presentation code must not contain gameplay rule logic.

---

## Core Architectural Intent

The project is intentionally split into the following major responsibilities:

- `RoundResolver`: resolves one round of gameplay rules
- `BattleService`: orchestrates one battle
- `RunService`: orchestrates the full run
- `RewardService`: generates and applies reward options

Keep these boundaries clean.

### Important boundary rules
- `RoundResolver` resolves rules but does not advance run-level flow.
- `BattleService` may apply battle results to battle-related runtime state, but does not decide run-level progression.
- `RunService` decides run progression, enemy progression, reward entry, next battle, next enemy, victory, and defeat.
- `RewardService` generates and applies rewards, but does not decide when rewards should happen.

---

## Implementation Priorities

Implement in this order unless explicitly instructed otherwise:

1. Domain models
2. Round resolution core
3. Reward generation and deduplication
4. Edit Mode tests for core rules
5. Battle application flow
6. Run application flow
7. Debug UI
8. Additional smoke tests

Do not start with final UI polish.

---

## Domain Modeling Rules

- State must live in state objects, not in services.
- Services should be long-lived and mostly stateless.
- Use runtime state objects for mutable game state.
- Separate persistent card state from board-projected round state.
- Do not mix run-level state, battle-level state, and round-temporary data.

Examples:
- Persistent card data belongs in `CardInstance`.
- On-board temporary combat values belong in `BoardCard`.
- Current enemy progression belongs in `EnemyProgressState`.
- Active battle progression belongs in `BattleState`.

---

## Reward Rules

Reward generation must follow the locked design:

- Each reward offer has exactly 4 options.
- Exactly 1 option is `Skip`.
- The other 3 options are non-skip options.
- Use up to 2 different legal `Upgrade` options.
- If fewer than 2 distinct legal `Upgrade` options exist, fill remaining non-skip slots with `Replace` options.
- Reward options must be deduplicated by resulting canonical deck state, not by raw action parameters.

Canonical reward deduplication must ignore:
- card instance identity
- deck order
- trait order

Do not simplify this rule.

---

## Round Resolution Rules

The round pipeline is fixed and must not be reordered:

1. Enter
2. Fixed Self
3. Movement
4. Board Derived
5. Resolve Open Slots
6. Apply Merged Damage
7. Post Resolve

Additional important rules:
- Movement is processed from right to left.
- Board-derived effects are calculated after movement.
- Damage is accumulated first, then applied simultaneously.
- Post-resolve effects only apply to cards newly played this round.
- Round snapshots should be recorded per phase for debugging and future presentation.

---

## Testing Expectations

Prefer lightweight, high-value tests.

### Must prioritize
- Edit Mode tests for domain logic
- Round resolution tests
- Reward legality tests
- Reward deduplication tests

### Keep minimal at first
- Play Mode smoke tests
- UI tests

Do not create a large testing framework before the core logic is stable.

When implementing a rule-heavy module, add targeted tests in the same task whenever practical.

---

## Code Quality Guidelines

- Keep classes focused.
- Keep methods readable.
- Prefer explicitness over cleverness.
- Avoid hidden coupling.
- Avoid duplicated rule logic across layers.
- Avoid premature generalization.
- Use stable naming aligned with the design documents.
- Keep logs and debug text concise and useful.

---

## Unity-Specific Guidance

- This project targets Unity 6.4.
- Keep core domain code as Unity-agnostic as practical.
- Use Unity-specific code mainly in presentation, scene wiring, and configuration assets.
- Prefer Edit Mode tests for pure logic validation.
- Use Play Mode only where runtime scene behavior matters.

---

## When a Task Is Ambiguous

If implementation details are unclear:

1. Follow the authoritative docs.
2. Preserve current architectural boundaries.
3. Choose the simpler solution.
4. Leave a short note describing the ambiguity.
5. Do not silently change locked rules.

---

## Expected Deliverables Per Task

Unless instructed otherwise, each implementation task should aim to provide:

- focused code changes
- no unrelated refactors
- tests for high-risk logic when appropriate
- a short summary of what was implemented
- a short note on any remaining ambiguity or risk

---

## Definition of Done for Early Iteration Tasks

A task is considered done when:

- it follows the locked docs
- it respects layer boundaries
- it compiles
- high-risk logic has at least minimal validation
- no unrelated systems were introduced
- the change is small enough to review comfortably