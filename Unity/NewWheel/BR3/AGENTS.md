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

- Use English only in code, comments, identifiers, logs, strings, Codex prompts, and implementation-facing documentation.
- Gameplay-facing documents under `Docs/game-design/`, `Docs/game-design/tasks/`, and `Docs/game-design/gdr/` may be written in Chinese.
- Do not change locked gameplay rules unless the user explicitly asks for a design change.
- Prefer small, reviewable changes.
- Keep behavior deterministic where possible.
- Do not introduce unnecessary abstractions.
- Do not add speculative systems that are not required by the current design.
- Once a valid round begins resolution, all seven accepted round phases must complete.
- Do not perform player-death, enemy-defeat, or battle-completion classification before authoritative final HP has been established.
- Player final HP of `0` or below causes run defeat.
- When both final HP values are `0` or below, player defeat has priority.
- Player defeat must not generate the fatal battle reward, settle remaining enemy rewards, advance to another enemy, or enter `Victory`.
- Do not add an eighth `RoundPhase`, a new flow stage, or a new config field for player-death handling.
- Do not let Presentation reconstruct gameplay outcome from displayed HP.

---

## Authoritative Documents

Always read these files before making changes in related areas:

- `Docs/README.md`
- `Docs/game-design/game-rules-locked.md`
- `Docs/game-design/trait-list.md`
- `Docs/game-design/enemy-and-reward-rules.md`
- relevant files under `Docs/game-design/gdr/`
- `Docs/game-design/gdr/GDR-0004-player-death-checked-after-full-round-resolution.md`
- `Docs/engineering/architecture-overview.md`
- `Docs/engineering/config-and-content.md`
- `Docs/engineering/domain-model.md`
- `Docs/engineering/round-resolution.md`
- `Docs/engineering/run-battle-reward-flow.md`
- `Docs/engineering/reward-generation.md`
- `Docs/engineering/testing-strategy.md`
- `Docs/engineering/debug-ui-plan.md`
- `Docs/adr/ADR-0001-layering-and-service-boundaries.md`
- `Docs/adr/ADR-0002-json-config-over-scriptableobject.md`
- `Docs/adr/ADR-0003-reward-dedup-by-canonical-deck.md`

If two documents appear to conflict, prefer the more specific engineering document for implementation details, but never override locked gameplay rules silently. Report the conflict clearly.

For player-death, temporary-zero, simultaneous-zero, reward short-circuit, or terminal-flow work, the minimum required references are:

- `Docs/game-design/gdr/GDR-0004-player-death-checked-after-full-round-resolution.md`
- `Docs/engineering/domain-model.md`
- `Docs/engineering/round-resolution.md`
- `Docs/engineering/run-battle-reward-flow.md`
- `Docs/engineering/testing-strategy.md`
- `Docs/engineering/debug-ui-plan.md`

---

## Layering Rules

### Domain
Domain code should contain:
- core runtime state objects
- value objects
- enums
- result objects
- pure or mostly pure rules logic
- raw round consequence representation
- explicit `BattleCompletionReason`
- immutable `BattleOutcome`
- nullable battle-scoped `PendingBattleOutcome`
- reward legality and deduplication logic
- canonical signature logic
- battle and round result objects

Domain result objects may contain both:

- raw rule consequences
- authoritative application values populated later by the Application layer

The ownership of those values must remain explicit.

Domain code should avoid unnecessary Unity-specific dependencies whenever practical.

### Application
Application code should contain:
- run-level orchestration
- battle-level orchestration
- reward flow orchestration
- authoritative HP application
- max-HP clamp
- final-HP establishment
- battle-completion classification
- completed-battle handoff
- reward eligibility
- terminal run progression
- command handling
- state transitions
- coordination between domain logic and UI
- construction delegation through helpers such as factories

Application code should not reimplement domain rules.
Application code must not independently recalculate raw trait consequences already resolved by `RoundResolver`.

### Presentation
Presentation code should contain:
- debug UI
- state display
- input forwarding
- view models
- presentation-only formatting
- presentation-only retention of finalized results
- terminal-state display and action gating
- thin Unity-facing bootstrapping and config loading entry points

Presentation code must not contain gameplay rule logic.

It may retain finalized result objects for inspection, but those retained references are observational only and must never become a second authoritative gameplay-state source.

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

- `RoundResolver` executes the complete seven-phase rule pipeline.
- `RoundResolver` produces raw merged damage, raw healing, logs, snapshots, and persistent non-HP post-resolve effects.
- `RoundResolver` does not authoritatively mutate HP, apply max-HP clamp, establish final HP, or classify survival.
- `BattleService` applies round consequences exactly once.
- `BattleService` establishes authoritative intermediate and final HP exactly once.
- `BattleService` finalizes `RoundResult`.
- `BattleService` classifies exactly one official `BattleCompletionReason`.
- `BattleService` stores a completed result in `BattleState.PendingBattleOutcome`.
- `FinishRoundPresentation(...)` advances only the presentation gate and must not recalculate or clear the authoritative pending outcome.
- `RunService` accepts and interprets the fixed battle outcome.
- `RunService` owns reward eligibility, next battle, next enemy, victory, and defeat.
- `RunService` interprets `PlayerDefeated` before enemy-defeat or reward progression.
- `RewardService` generates and applies reward content but does not decide whether a completed battle is reward-eligible.
- `GameConfigLoader` loads config but does not orchestrate gameplay.
- `RuntimeStateFactory` constructs runtime object graphs but does not orchestrate gameplay flow.
- Presentation displays and forwards authoritative data but does not classify gameplay outcome.

The intended chain is:

```text
RoundResolver computes raw consequences
→ BattleService applies consequences
→ BattleService establishes final HP
→ BattleService fixes the battle outcome
→ Presentation displays the fixed result
→ RunService interprets run progression
```

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

### Completed battle state

`BattleState` should contain:

- `PendingBattleOutcome : BattleOutcome?`

Rules:

- it is `null` while the battle can continue
- it becomes non-null when a round completes the battle
- it is fixed before entering result presentation
- it remains stable during `PresentingRoundResult`
- it remains available in `BattleComplete`
- `FinishRoundPresentation(...)` must not clear it
- it is consumed only when `RunService.AcceptCompletedBattle(...)` accepts the completed battle and clears `RunState.ActiveBattle`

Do not store the same authoritative outcome in two independently mutable gameplay locations.

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

### Outcome timing

A completed battle outcome may already exist while:

```text
RunFlowStage == InBattle
BattleFlowStage == PresentingRoundResult
PendingBattleOutcome != null
```

This is valid and intentional.

At that point:

- all seven phases have completed
- HP application has completed
- final HP is authoritative
- the completion reason is fixed
- Continue cannot change the result

After `FinishRoundPresentation(...)`:

- the battle moves to `BattleComplete`
- the same pending outcome remains authoritative
- the run remains `InBattle` until completed-battle acceptance

### Terminal-state invariants

For both `Victory` and `Defeat`:

- `RunState.ActiveBattle == null`
- `RunState.PendingRewardOffer == null`
- no in-run gameplay action is legal

Do not introduce an additional run or battle stage for player death.

---

## Result Object Rules

The architecture distinguishes between gameplay result objects and application command result objects.

### Gameplay result objects

- `RoundResult`
- `BattleOutcome`

### Gameplay outcome enum

- `BattleCompletionReason`

Accepted reasons:

- `PlayerDefeated`
- `EnemyDefeated`
- `AllRoundsCompleted`

### Application command result objects

- `BattleCommandResult`
- `RunCommandResult`

Do not merge these concepts casually.

### RoundResult lifecycle

A `RoundResult` has two lifecycle points:

1. resolver-created
2. battle-finalized

The resolver-created result contains raw consequences such as:

- merged damage
- raw healing
- slot results
- logs
- snapshots

`BattleService` finalizes fields such as:

- HP before
- HP after merged damage
- actual healing applied
- final HP

A resolver-created result must not be treated as the complete authoritative result until application fields are finalized.

### BattleOutcome rule

`BattleOutcome` contains:

- battle index
- actual rounds played
- one official `BattleCompletionReason`
- final player HP
- final enemy HP

The completion reason is authoritative.

Do not reconstruct a different official result from the HP fields.

### Command-result distinction

Player defeat is a successful gameplay outcome.

It is not a failed command and must not be represented through `FailureReason`.

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
9. Debug-oriented presentation
10. Minimal Play Mode smoke tests
11. Additional smoke validation and iteration

Do not start with final UI polish.

For a cross-layer gameplay-rule change:

1. update canonical gameplay rules and GDR where required
2. update canonical engineering documents
3. update `AGENTS.md`
4. only then create implementation tasks or Codex prompts

Do not begin implementation while the canonical engineering documents still disagree.

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

Reward generation must follow the locked gameplay design and current engineering documents.

### Locked invariants

- Each reward offer contains exactly 1 `Skip`.
- `Skip` is always a real reward option and consumes that reward opportunity when chosen.
- Non-skip options within the same offer must be deduplicated by resulting canonical deck state, not by raw action parameters.
- Reward options must remain one-step actions.
- Replacement generation must use authored reward-generation config rather than hardcoded values.

### Config-driven offer structure

The current demo uses config-driven default reward structure values.

Use:

- `RewardGenerationConfig.rewardOfferTotalOptions`
- `RewardGenerationConfig.upgradeTarget`
- `RewardGenerationConfig.replacementTraitCount`

Important rules:

- total offer size is configurable
- upgrade target is configurable
- replace count is not an independent source of truth
- actual replace count is derived from:

`rewardOfferTotalOptions - 1 - actualUpgradeCount`

where the `1` is the always-present `Skip`

### Default baseline note

The current default baseline is still:

- 4 total options
- upgrade target 2
- replacement trait count 2

Under that default baseline, the familiar structures are still:

- `2 Upgrade + 1 Replace + 1 Skip`
- `1 Upgrade + 2 Replace + 1 Skip`
- `0 Upgrade + 3 Replace + 1 Skip`

But these are default-baseline examples, not permanent hard-coded reward invariants.

### Canonical reward deduplication

Canonical reward deduplication must ignore:

- card instance identity
- deck order
- trait order

Do not simplify this rule.

### Replacement legality

Replacement generation must follow the authored config and current legality rules.

Important rules:

- replacement cards must have exactly the configured `replacementTraitCount`
- the current allowed test range for `replacementTraitCount` is `0..3`
- replacement trait sets must not contain duplicates
- replacement trait sets must not contain both `ShiftLeft` and `ShiftRight`

### Reward eligibility boundary

`RewardService` does not decide whether a completed battle earns a reward.

`RunService` must prevent reward generation for:

- `BattleCompletionReason.PlayerDefeated`
- simultaneous zero resolved as `PlayerDefeated`
- `BattleCompletionReason.AllRoundsCompleted` when the configured battle limit is exhausted
- terminal `Victory`
- terminal `Defeat`

### Player-death reward rule

After player defeat:

- do not generate the fatal battle reward
- do not settle remaining rewards for the current enemy
- do not increment `RewardsClaimed`
- do not enter `ChoosingReward`
- do not advance to another enemy
- do not enter `Victory`

A reward-service call spy or equivalent test seam may be used to prove that reward generation was not requested.

---

## HP and Healing Rules

Runtime state must carry max HP explicitly.

- `RunState` should contain `PlayerHp` and `PlayerMaxHp`
- `EnemyProgressState` should contain `CurrentHp` and `MaxHp`

Do not repeatedly query authored config during live runtime flow to determine max HP.

### Ownership

`RoundResolver` computes:

- `DamageToPlayer`
- `DamageToEnemy`
- `HealToPlayer`
- `HealToEnemy`

These are raw consequences.

`BattleService` owns authoritative application.

### Required application order

```text
read HP before application
→ apply merged damage
→ record HP after merged damage
→ apply raw healing
→ clamp to max HP
→ record actual healing applied
→ establish final HP
→ finalize RoundResult
→ classify battle completion
```

### Required RoundResult application fields

- `PlayerHpBefore`
- `PlayerHpAfterMergedDamage`
- `PlayerHealingApplied`
- `PlayerHpAfter`
- `EnemyHpBefore`
- `EnemyHpAfterMergedDamage`
- `EnemyHealingApplied`
- `EnemyHpAfter`

### Temporary zero

HP after merged damage may be `0` or negative.

That intermediate value does not determine survival.

Post Resolve must still complete, and healing may restore final HP above zero.

### Final-zero rule

Final player HP of exactly `0` is player defeat.

### No duplicate HP calculation

- `RoundResolver` must not independently calculate authoritative final HP.
- Presentation must not independently calculate authoritative final HP.
- `BattleService` must not independently recalculate raw trait damage or healing rules already resolved by `RoundResolver`.

---

## Round Resolution Rules

The round pipeline is fixed and must not be reordered or terminated early:

1. Enter
2. Fixed Self
3. Movement
4. Board Derived
5. Resolve Open Slots
6. Apply Merged Damage
7. Post Resolve

Additional rules:

- Movement is processed from right to left.
- Board-derived effects are calculated after movement.
- Damage is accumulated before application.
- Player-side and enemy-side merged damage are simultaneous consequences.
- Post-resolve effects apply only to cards newly played this round.
- `Regrow`, `Lifesteal`, and `Growth` still execute after projected lethal damage.
- All seven phase snapshots must still be produced in projected-lethal scenarios.
- `RoundResolver` may read HP as round context but must not mutate authoritative HP.
- Player death, enemy defeat, battle completion, and continuation are classified after battle-layer application.
- Survival classification is not an eighth `RoundPhase`.

Do not insert an early-return death check between `ApplyMergedDamage` and `PostResolve`.

---

## Battle Outcome Rules

`BattleService` classifies the resolved round only after final authoritative HP has been established.

Required priority:

```text
if final player HP <= 0
    BattleCompletionReason.PlayerDefeated
else if final enemy HP <= 0
    BattleCompletionReason.EnemyDefeated
else if the final round completed
    BattleCompletionReason.AllRoundsCompleted
else
    no completed battle outcome
```

### Simultaneous zero

When both final HP values are `0` or below:

- the official reason is `BattleCompletionReason.PlayerDefeated`
- do not also create an `EnemyDefeated` outcome
- preserve both final HP values for debug and presentation
- do not enter enemy-defeat, reward, next-enemy, or victory flow

### Presentation gate

`SubmitPlayerCard(...)` must:

- complete rule resolution
- complete authoritative HP application
- finalize `RoundResult`
- classify and store any completed outcome
- then enter `PresentingRoundResult`

`FinishRoundPresentation(...)` must:

- return or expose the already-fixed outcome
- move to `BattleComplete` when appropriate
- never reclassify from current HP
- never clear `PendingBattleOutcome`

---

## Run Defeat and Completed-Battle Handoff Rules

`RunService.AcceptCompletedBattle(...)` must validate:

- run stage is `InBattle`
- an active battle exists
- battle stage is `BattleComplete`
- an authoritative pending outcome exists

When a supplied `BattleOutcome` parameter is used, it must match the pending authoritative outcome.

When `RunService` reads the outcome directly from the active battle, do not introduce a second conflicting outcome source.

### Completed-battle accounting

A fatal completed battle still counts as a completed battle.

`BattlesPlayed` should increment exactly once before terminal branch interpretation.

Repeated acceptance must not increment it twice.

### Required branch order

1. `BattleCompletionReason.PlayerDefeated`
2. `BattleCompletionReason.EnemyDefeated`
3. `BattleCompletionReason.AllRoundsCompleted` with battle limit exhausted
4. `BattleCompletionReason.AllRoundsCompleted` with more battles remaining

### PlayerDefeated branch

Required final state:

- `FlowStage = Defeat`
- `ActiveBattle = null`
- `PendingRewardOffer = null`
- `RewardsClaimed` unchanged

Prohibited behavior:

- reward generation
- early remaining-reward settlement
- next battle
- next enemy
- victory

### Battle-limit defeat

The existing second defeat source remains valid:

- player final HP is above zero
- enemy final HP is above zero
- the final round completed
- the configured battle limit has been exhausted

This branch also creates no reward for the exhausting battle.

---

## Presentation and Debug Scene Rules

The current presentation target is a debug-oriented workbench, not a production UI.

Follow `Docs/engineering/debug-ui-plan.md` for:

- debug scene structure
- panel responsibilities
- controller responsibilities
- full-refresh model
- fixed skeleton versus runtime-generated repeated entries
- Unity UI component mapping
- button enable / disable rules
- manual validation scenarios

### Gameplay-boundary rules

- keep all gameplay classification out of UI code
- do not infer player defeat from displayed player HP
- do not infer enemy defeat from displayed enemy HP
- do not decide simultaneous-zero priority
- do not decide reward eligibility
- do not decide victory or defeat
- Continue is a presentation gate only

### Latest-result retention

Presentation may retain:

- the latest finalized `RoundResult`
- the latest completed `BattleOutcome`

Retain them before `RunService.AcceptCompletedBattle(...)` clears the active battle.

The retained data must remain inspectable after:

- reward transition
- `Victory`
- `Defeat`

Do not mutate retained result objects.

### Required HP display

The latest round should distinguish:

- HP before
- merged damage
- HP after merged damage
- raw healing
- actual healing applied
- final HP

Do not reduce this to a single before/after display.

### Simultaneous-zero display

Display:

- official player defeat
- both final HP values
- a clear player-death-priority explanation

Do not display official enemy defeat.

### Terminal action gating

After `Victory` or `Defeat`, disable:

- player deck actions
- Continue
- Start Battle
- reward actions

Debug utility actions such as New Run may remain available.

---

## Testing Expectations

Prefer lightweight, high-value tests owned by the layer responsible for the behavior.

### Resolver tests must prioritize

- complete seven-phase execution
- Post Resolve after projected lethal damage
- Regrow raw healing after projected lethal damage
- Lifesteal raw healing after projected lethal damage
- Growth after projected lethal damage
- seven snapshots in lethal-projection scenarios
- no authoritative HP mutation
- no survival classification

### BattleService tests must prioritize

- HP before application
- HP after merged damage
- raw versus actual healing
- max-HP clamp
- final HP
- temporary-zero recovery
- final HP exactly zero
- early-round player death
- enemy defeat while player survives
- simultaneous-zero priority
- one stable `PendingBattleOutcome`
- outcome fixed before presentation
- no outcome recomputation during Continue

### RunService tests must prioritize

- completed-battle acceptance validation
- `BattlesPlayed` increments exactly once
- player-death terminal flow
- simultaneous-zero terminal flow
- no reward after player death
- no reward after battle-limit exhaustion
- enemy-defeat reward progression
- final-enemy victory only while player survives
- terminal cleanup and action rejection

### Presentation validation must prioritize

- authoritative HP timeline formatting
- player-death status
- simultaneous-zero status
- terminal button gating
- retained fatal-round visibility
- controller handoff wiring

### Test-layer rules

- use Edit Mode tests for gameplay correctness
- keep Play Mode coverage to smoke-level integration
- use the debug scene for manual inspection
- do not move gameplay assertions into formatter or controller code
- do not create a large mocking framework solely for this change
- use a small fake or spy where proving that reward generation was not called is necessary

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
- Preserve one authoritative source for final HP.
- Preserve one authoritative source for completed battle outcome.
- Avoid early returns that skip accepted round phases.
- Avoid duplicated death, defeat, or reward-eligibility checks across layers.
- Prefer explicit enum-based outcome branching over inference from unrelated state.
- Keep terminal cleanup atomic and easy to inspect.
- Do not hide outcome classification inside UI formatting or MonoBehaviour callbacks.

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

For player-death-related ambiguity, do not guess by reading only current HP.

Trace the authoritative chain:

```text
finalized RoundResult
→ PendingBattleOutcome
→ BattleCompletionReason
→ RunService branch
→ RunFlowStage
```

Report any conflict instead of introducing an alternate classification path.

---

## Gameplay Document Scope Rules

- Treat `Docs/game-design/game-rules-locked.md`, `Docs/game-design/trait-list.md`, and `Docs/game-design/enemy-and-reward-rules.md` as the current accepted gameplay rules.
- Treat `Docs/game-design/gdr/` as gameplay decision rationale, not as a replacement for canonical rules.
- Treat `Docs/game-design/tasks/` as deferred ideas, proposals, or incubator notes.
- Do not implement ideas from `Docs/game-design/tasks/` unless they are explicitly promoted into canonical gameplay documents or the user explicitly asks for them.

---

## Expected Deliverables Per Task

Unless instructed otherwise, each implementation task should aim to provide:

- focused code changes
- no unrelated refactors
- tests for high-risk logic when appropriate
- a short summary of what was implemented
- a short note on any remaining ambiguity or risk
- explicit note on which layer owns each changed rule
- targeted regression tests for any changed outcome or reward branch
- confirmation that no new phase, flow stage, or config field was introduced unless explicitly approved
- manual Unity Editor wiring notes when Presentation references change

---

## Definition of Done for Early Iteration Tasks

A task is considered done when:

- it follows the locked docs
- it respects layer boundaries
- it compiles
- high-risk logic has at least minimal validation
- no unrelated systems were introduced
- the change is small enough to review comfortably
- all seven round phases still execute in projected-lethal scenarios
- authoritative HP application occurs exactly once
- one official battle-completion reason is produced
- simultaneous zero resolves as player defeat
- player death cannot enter reward or victory flow
- terminal state clears active battle and pending reward
- finalized fatal-round information remains inspectable
- existing battle-limit defeat remains functional
- tests respect the resolver / battle / run / presentation ownership boundaries