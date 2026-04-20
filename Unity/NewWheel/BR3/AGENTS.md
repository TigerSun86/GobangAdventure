# AGENTS.md

## Project Purpose

This repository contains a Unity 6.4 2D turn-based card roguelike demo.

The current implementation priority is:

1. Config and runtime construction
2. Domain core
3. Application flow
4. Debug-oriented presentation
5. Polish and refactoring

The goal of the current phase is correctness, clarity, and fast iteration, not final production polish.

---

## Hard Constraints

- Use English only in code, comments, identifiers, logs, strings, and implementation-facing documentation.
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
- `Docs/engineering/config-and-content.md`
- `Docs/engineering/domain-model.md`
- `Docs/engineering/round-resolution.md`
- `Docs/engineering/run-battle-reward-flow.md`
- `Docs/engineering/reward-generation.md`
- `Docs/engineering/testing-strategy.md`
- `Docs/adr/ADR-0001-layering-and-service-boundaries.md`
- `Docs/adr/ADR-0002-json-config-over-scriptableobject.md`
- `Docs/adr/ADR-0003-reward-dedup-by-canonical-deck.md`

If two documents appear to conflict, prefer the more specific engineering document for implementation details, but never override locked gameplay rules silently. Report the conflict clearly.

---

## Layering Rules

### Domain
Domain code should contain:
- core runtime state objects
- value objects
- enums
- result objects
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
- construction delegation through helpers such as factories

Application code should not reimplement domain rules.

### Presentation
Presentation code should contain:
- debug UI
- state display
- input forwarding
- view models
- presentation-only formatting
- thin Unity-facing bootstrapping and config loading entry points

Presentation code must not contain gameplay rule logic.

---

## Core Architectural Intent

The project is intentionally split into the following major responsibilities:

- `RoundResolver`: resolves one round of gameplay rules
- `BattleService`: orchestrates one battle
- `RunService`: orchestrates the full run
- `RewardService`: generates and applies rewards
- `GameConfigLoader`: loads and validates authored config
- `RuntimeStateFactory`: constructs runtime state from authored config

Keep these boundaries clean.

### Important boundary rules
- `RoundResolver` resolves rules but does not advance run-level flow.
- `BattleService` may apply battle results to battle-related runtime state, but does not decide run-level progression.
- `RunService` decides run progression, enemy progression, reward entry, next battle, next enemy, victory, and defeat.
- `RewardService` generates and applies rewards, but does not decide when rewards should happen.
- `GameConfigLoader` loads config but does not orchestrate gameplay.
- `RuntimeStateFactory` constructs runtime object graphs but does not orchestrate gameplay flow.

---

## Authored Config Rules

The current demo uses:
- JSON as the primary authored config format
- pure C# config objects
- a thin Unity-facing loading boundary

Do not introduce ScriptableObject as the primary gameplay content source for the current demo unless explicitly asked.

The main config objects are:
- `GameConfig`
- `PlayerStartConfig`
- `EnemyConfig`
- `RewardGenerationConfig`
- `TraitTuning`
- `CardSpec`

Important rules:
- authored config is not runtime state
- runtime state should not repeatedly query authored config for live gameplay values such as max HP
- max HP should be copied into runtime state during initialization
- JSON loading should remain isolated behind the loader boundary

---

## Runtime State Rules

State must live in state objects, not in services.

Use runtime state objects for mutable game state.

Separate:
- authored card spec
- runtime card instance
- on-board projected card state

Examples:
- static authored card data belongs in `CardSpec`
- persistent runtime deck card data belongs in `CardInstance`
- on-board temporary combat values belong in `BoardCard`
- current enemy progression belongs in `EnemyProgressState`
- active battle progression belongs in `BattleState`

Do not collapse these concepts together.

---

## Flow Stage Rules

Use the accepted flow stage model.

### RunFlowStage
- `ReadyForNextBattle`
- `InBattle`
- `ChoosingReward`
- `Victory`
- `Defeat`

### BattleFlowStage
- `WaitingForPlayerCard`
- `ResolvingRound`
- `PresentingRoundResult`
- `BattleComplete`

Do not invent extra flow stages unless there is a clear design reason and the docs are updated.

---

## Result Object Rules

The architecture distinguishes between gameplay result objects and application command result objects.

### Gameplay result objects
- `RoundResult`
- `BattleOutcome`

### Application command result objects
- `BattleCommandResult`
- `RunCommandResult`

Do not merge these concepts casually.

---

## Implementation Priorities

Implement in this order unless explicitly instructed otherwise:

1. Config objects and loading boundary
2. Runtime construction helpers
3. Domain models
4. Round resolution core
5. Reward generation and deduplication
6. Edit Mode tests for core rules
7. Battle application flow
8. Run application flow
9. Debug UI
10. Additional smoke tests

Do not start with final UI polish.

---

## Config and Construction Rules

- `GameConfigLoader` should deserialize and validate config.
- `RuntimeStateFactory` should create runtime state from config.
- `RunService.CreateNewRun(...)` should delegate initial runtime construction to `RuntimeStateFactory`.
- Do not manually spread runtime construction logic across unrelated services or MonoBehaviours.
- Keep config schemas simple and compatible with the current serializer assumptions.

Do not introduce:
- config dictionaries
- polymorphic trait config systems
- heavy serializer-dependent config graphs
unless explicitly required.

---

## Reward Rules

Reward generation must follow the locked design:

- Each reward offer has exactly 4 options.
- Exactly 1 option is `Skip`.
- Exactly 3 options are non-skip.
- Use up to 2 different legal `Upgrade` options.
- If fewer than 2 distinct legal `Upgrade` options exist, fill remaining non-skip slots with `Replace` options.
- Reward options must be deduplicated by resulting canonical deck state, not by raw action parameters.

Canonical reward deduplication must ignore:
- card instance identity
- deck order
- trait order

Do not simplify this rule.

Replacement generation must use authored reward-generation config rather than hardcoded values.

---

## HP and Healing Rules

Runtime state must carry max HP explicitly.

- `RunState` should contain `PlayerHp` and `PlayerMaxHp`
- `EnemyProgressState` should contain `CurrentHp` and `MaxHp`

Healing rules:
- healing cannot raise current HP above max HP
- `RoundResolver` computes raw healing totals
- `BattleService` applies healing and clamps to max HP

Do not repeatedly query authored config during live runtime flow to determine max HP.

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
- round resolution tests
- reward legality tests
- reward deduplication tests

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
- Keep core domain and runtime logic as Unity-agnostic as practical.
- Use Unity-specific code mainly in presentation, bootstrapping, and config loading edges.
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