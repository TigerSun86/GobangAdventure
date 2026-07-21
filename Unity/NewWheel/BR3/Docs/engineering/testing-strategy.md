# Testing Strategy

## Purpose

This document defines the testing strategy for the Unity 6.4 2D turn-based card roguelike demo.

It explains:

* the overall testing philosophy
* how Edit Mode tests, Play Mode tests, and manual validation are divided
* which modules should be tested first
* how test code should be organized
* which test helpers should exist in the first implementation phase
* how tests should be paired with Codex implementation batches
* how the debug scene fits into the validation workflow

This document is intended to support:

* fast iteration
* early bug discovery
* clear Codex task scoping
* confidence in core gameplay correctness

The detailed design of the debug scene and debug-oriented presentation is defined in `debug-ui-plan.md`.

---

## Testing Goals

The current testing strategy is designed to satisfy these goals:

1. catch rule calculation bugs early
2. catch flow progression bugs early
3. keep feedback fast during implementation
4. avoid overengineering the test system
5. prioritize high-value tests over broad but shallow coverage
6. support Codex-assisted implementation with small verifiable tasks
7. preserve a manual debug path for behavior inspection
8. protect gameplay-outcome priority, terminal-state cleanup, and reward short-circuit behavior from regression

The current goal is not exhaustive automated coverage.

---

## Core Strategy Summary

The project uses a three-part testing strategy:

### 1. Edit Mode tests

These are the primary automated tests.

They should cover:

* config loading and validation
* runtime construction
* domain rules
* reward generation and deduplication
* battle and run application flow logic where practical
* authoritative HP application and final-HP classification
* player-death and simultaneous-zero outcome priority
* completed-battle handoff and reward eligibility
* terminal-state cleanup and action gating

### 2. Play Mode smoke tests

These are minimal runtime-level checks.

They should cover:

* basic config bootstrap
* minimal scene-level flow
* basic integration between systems in Unity runtime

### 3. Manual debug validation

A debug scene and debug-oriented UI should remain part of the validation workflow.

This is important for:

* inspecting logs
* inspecting snapshots
* verifying flow feel
* understanding system behavior that is correct logically but unclear visually

---

## Why Edit Mode Tests Are the Main Priority

Most of the highest-value bugs in the current demo are expected to occur in:

* config conversion
* runtime state construction
* round resolution
* reward legality
* reward deduplication
* battle and run orchestration logic

These areas do not require a polished Unity scene to validate.

That makes Edit Mode tests the best main investment in the current phase.

---

## Why Play Mode Tests Are Kept Small

The project does not currently need a large runtime integration test matrix.

Heavy Play Mode coverage would increase maintenance cost too early.

For the current phase, Play Mode tests should only confirm that:

* the Unity-side bootstrap works
* the basic runtime loop can execute
* the minimal debug flow is operational

Anything beyond this should be added only if a strong need appears.

---

## Why Manual Validation Still Matters

Some problems are not primarily about logic correctness.

Examples:

* unclear logs
* hard-to-read snapshots
* bad debug panel usability
* confusing battle presentation timing

These problems are better discovered through manual debug usage than through automated tests.

For this reason, manual debug validation remains a required part of the workflow.

---

## Relationship to the Debug Scene

The debug scene is a manual validation workbench.

It is not:

* a replacement for Edit Mode tests
* a replacement for domain-level rule verification
* a production-ready final UI

It is:

* the primary manual inspection surface
* the main place to validate debug readability
* the main place to validate presentation sequencing
* the basis for later minimal Play Mode smoke tests

The detailed scene layout, controller responsibilities, prefab guidance, and refresh model are defined in `debug-ui-plan.md`.

---

## Test Layers

## Layer 1: Edit Mode Tests

Edit Mode tests are the primary automated layer.

They should be used for:

### Config and construction

* `GameConfigLoader`
* config validation
* `RuntimeStateFactory`

### Domain logic

* `RoundResolver`
* reward canonicalization
* reward legality
* reward generation
* reward application

### Application flow logic

* `BattleService`
* `RunService`

Where possible, these tests should remain:

* deterministic
* explicit
* easy to debug
* independent from scene setup

---

## Layer 2: Play Mode Smoke Tests

Play Mode tests should exist, but remain minimal.

They should focus on:

* config bootstrap inside Unity runtime
* basic debug scene flow
* confirming that key scene wiring does not break

These tests should not become the main validation mechanism for gameplay rules.

---

## Layer 3: Manual Debug Validation

Manual validation should use a debug scene or equivalent debug runtime entry.

The debug scene should support:

* loading config
* starting a run
* starting a battle
* selecting a card
* stepping through battle flow
* viewing round logs
* viewing phase snapshots
* viewing reward offers
* continuing flow after battle and reward resolution
* inspecting HP before damage, HP after merged damage, raw healing, actual healing, and final HP
* validating temporary-zero recovery
* validating player-death presentation
* validating simultaneous-zero presentation
* validating terminal button state after run defeat

The debug scene is part of the validation strategy, not just a convenience tool.

---

## Test Organization

The recommended test directory structure is:

```
Assets/
  Tests/
    EditMode/
      Config/
      Domain/
      Application/
      TestHelpers/
    PlayMode/
      Smoke/
```

This structure keeps test code aligned with production code layers while staying simple enough for the current demo.

---

## Recommended First Test Files

The current recommended first-wave test files are:

## EditMode / Config

### `GameConfigLoaderTests.cs`

Should test:

* valid config loads successfully
* invalid config fails validation
* required sections must exist
* invalid authored card specs are rejected
* `enemies.Count >= 1`
* `startingDeck.Count >= 3`
* `fixedDeck.Count >= 3`
* `battleLimit >= 1`
* `rewardOfferTotalOptions >= 2`
* `upgradeTarget >= 0`
* `upgradeTarget <= rewardOfferTotalOptions - 1`
* `replacementTraitCount` is accepted only within the current allowed range `0..3`

### `RuntimeStateFactoryTests.cs`

Should test:

* `GameConfig` creates a valid `RunState`
* `CardSpec` creates a valid `CardInstance`
* `EnemyConfig` creates a valid `EnemyProgressState`
* player max HP is copied into runtime state
* enemy max HP is copied into runtime state
* starting deck size is derived from authored `startingDeck`
* enemy fixed deck size is derived from authored `fixedDeck`
* enemy battle limit remains available through `EnemyProgressState.Config.battleLimit`
* reward total is not introduced as a separate authored runtime source of truth when it can be derived from enemy config

---

## EditMode / Domain

### `RoundResolverCoreTests.cs`

Should test:

* RPS outcome rules
* tie deals no damage
* minimum damage rule

### `RoundResolverFixedSelfTests.cs`

Should test:

* fixed self recalculation
* empower bonus application
* current power does not incorrectly persist between rounds

### `RoundResolverMovementTests.cs`

Should test:

* `ShiftLeft`
* `ShiftRight`
* right-to-left processing order
* movement persistence across rounds

### `RoundResolverBoardDerivedTests.cs`

Should test:

* `AdjacentAid`
* `Suppress`
* derived effects after movement
* delta-based application behavior

### `RoundResolverPostResolveTests.cs`

Should test:

* `Regrow` produces the expected raw healing
* `Lifesteal` produces raw healing equal to the newly played card's attributed damage
* `Growth` updates persistent source-card state
* post-resolve effects only affect cards newly played this round
* Post Resolve executes when projected player HP after merged damage would be zero or below
* Post Resolve executes when projected enemy HP after merged damage would be zero or below
* `Regrow` raw healing is still produced after projected lethal damage
* `Lifesteal` raw healing is still produced after projected lethal damage
* `Growth` still executes during a round that will later result in player defeat
* `RoundResolver` does not classify player death
* `RoundResolver` does not classify enemy defeat
* `RoundResolver` does not authoritatively mutate player HP
* `RoundResolver` does not authoritatively mutate enemy HP
* `RoundResolver` does not perform max-HP clamp
* `RoundResolver` does not independently establish final authoritative HP

### Important boundary

These tests verify rule consequences and complete execution of Phase 7.

They do not verify:

* actual healing after clamp
* final HP
* battle-completion reason
* run defeat

Those belong to `BattleServiceTests` and `RunServiceTests`.

### `RoundResolverSnapshotTests.cs`

Should test:

* snapshot count
* exact snapshot phase order
* presence of snapshots for all seven phases
* all seven snapshots are produced when projected HP after merged damage is zero or below
* the `PostResolve` snapshot is still produced after projected lethal damage
* no additional eighth `RoundPhase` is introduced for survival or battle-completion classification

---

## EditMode / Domain / Reward

### `RewardCanonicalSignatureTests.cs`

Should test:

* trait-order-insensitive equivalence
* deck-order-insensitive equivalence
* instance-identity-insensitive equivalence

### `RewardUpgradeGenerationTests.cs`

Should test:

* upgrade legality
* duplicate trait rejection
* fourth trait rejection
* left/right shift conflict rejection
* distinct upgrade generation

### `RewardReplaceGenerationTests.cs`

Should test:

* replacement spec legality
* use of authored replacement config
* filtering of meaningless replacements
* `replacementTraitCount = 0`
* `replacementTraitCount = 1`
* `replacementTraitCount = 2` default-baseline behavior
* `replacementTraitCount = 3`
* illegal replacement trait counts outside the current allowed range are rejected
* duplicate trait rejection still applies under all valid replacement trait-count settings
* left/right shift conflict rejection still applies under all valid replacement trait-count settings

### `RewardOfferCompositionTests.cs`

Should test two layers.

#### Layer 1: generalized invariant tests

* exactly one `Skip`
* total option count equals configured `rewardOfferTotalOptions`
* selected upgrade count never exceeds configured `upgradeTarget`
* replace count fills the remaining non-skip slots
* non-skip options are pairwise distinct by canonical resulting deck state

#### Layer 2: default-baseline example tests

When:

* `rewardOfferTotalOptions = 4`
* `upgradeTarget = 2`
* `replacementTraitCount = 2`

the system should still produce the familiar default-baseline structures:

* `2 Upgrade + 1 Replace + 1 Skip`
* `1 Upgrade + 2 Replace + 1 Skip`
* `0 Upgrade + 3 Replace + 1 Skip`

### `RewardDedupTests.cs`

Should test:

* duplicate upgrade result collapse
* duplicate replace result collapse
* canonical resulting deck comparison

### `RewardApplicationTests.cs`

Should test:

* upgrade modifies existing card in place
* replace creates a new card instance
* replace does not inherit old permanent growth
* skip leaves the deck unchanged

---

## EditMode / Application

### `BattleServiceTests.cs`

`BattleServiceTests` should be divided conceptually into five groups.

#### Group 1: battle startup and command validation

Should test:

* battle startup
* `PendingBattleOutcome` starts as `null`
* valid player card submission
* invalid player card rejection
* used player card rejection
* invalid battle flow stage rejection
* failed submission does not mutate HP
* failed submission does not mark a card as used
* failed submission does not advance round or battle flow

#### Group 2: authoritative HP application

Should test:

* `PlayerHpBefore` and `EnemyHpBefore` match authoritative runtime state
* merged damage is applied exactly once
* `PlayerHpAfterMergedDamage` is recorded correctly
* `EnemyHpAfterMergedDamage` is recorded correctly
* raw player healing is preserved in `HealToPlayer`
* raw enemy healing is preserved in `HealToEnemy`
* `PlayerHealingApplied` records healing after clamp
* `EnemyHealingApplied` records healing after clamp
* healing cannot exceed max HP
* final `PlayerHpAfter` matches `RunState.PlayerHp`
* final `EnemyHpAfter` matches `EnemyProgressState.CurrentHp`
* the finalized `RoundResult` is added to battle history only after authoritative HP fields are populated

#### Group 3: temporary zero and recovery

Should test:

* player receives lethal merged damage with no healing and ends at zero or below
* player reaches zero or below after merged damage but survives after `Regrow`
* player reaches zero or below after merged damage but survives after `Lifesteal`
* healing produces a final player HP of exactly zero
* healing clamp produces the correct final HP and actual-healing value
* temporary zero followed by final HP above zero does not create a player-defeat outcome
* final HP exactly zero does create a player-defeat outcome

Representative assertions should include:

```
PlayerHpBefore
PlayerHpAfterMergedDamage
HealToPlayer
PlayerHealingApplied
PlayerHpAfter
```

#### Group 4: battle-completion classification

Should test:

* player death completes the battle
* player death may complete the battle in round 1
* player death may complete the battle in round 2
* player death in round 3 produces `BattleCompletionReason.PlayerDefeated`
* player alive and enemy final HP zero or below produces `BattleCompletionReason.EnemyDefeated`
* both sides alive after round 3 produces `BattleCompletionReason.AllRoundsCompleted`
* both sides alive before round 3 produces no pending outcome
* simultaneous zero produces only `BattleCompletionReason.PlayerDefeated`
* simultaneous zero preserves both final HP values in the outcome
* `EnemyDefeated` is never the official reason when player final HP is zero or below
* one immutable pending outcome is created when the battle completes
* the pending outcome records the actual rounds played
* the pending outcome records final authoritative HP

#### Group 5: presentation-gate behavior

Should test:

* `SubmitPlayerCard(...)` fixes `PendingBattleOutcome` before result presentation begins
* after a fatal submission, `BattleFlowStage == PresentingRoundResult`
* after a fatal submission, `IsBattleComplete == false`
* `FinishRoundPresentation(...)` returns the already-stored outcome
* `FinishRoundPresentation(...)` moves the battle to `BattleComplete`
* `FinishRoundPresentation(...)` does not clear `PendingBattleOutcome`
* `FinishRoundPresentation(...)` does not recreate or reclassify the outcome
* `FinishRoundPresentation(...)` does not read final HP to choose a different result
* when no pending outcome exists, `FinishRoundPresentation(...)` advances to the next round
* when no pending outcome exists, the round index increments exactly once
* repeated or invalid presentation-finish commands do not advance battle state twice

### Important test boundary

`BattleServiceTests` verify:

* authoritative HP mutation
* clamp
* final HP
* official battle-completion reason
* pending-outcome lifecycle

They do not decide:

* reward eligibility
* next enemy
* victory
* run defeat

Those belong to `RunServiceTests`.

### `RunServiceTests.cs`

`RunServiceTests` should cover both existing run progression and the two official run-defeat sources.

#### Group 1: existing run and battle entry behavior

Should test:

* run creation
* legal battle start conditions
* illegal battle start conditions
* a battle cannot start in `Victory`
* a battle cannot start in `Defeat`
* a battle cannot start while a reward is pending
* a battle cannot start while another active battle exists

#### Group 2: completed-battle acceptance validation

Should test:

* acceptance requires `RunFlowStage.InBattle`
* acceptance requires an active battle
* acceptance requires `BattleFlowStage.BattleComplete`
* acceptance requires a non-null authoritative pending outcome
* if `AcceptCompletedBattle(...)` accepts a supplied `BattleOutcome`, it must match the authoritative pending outcome
* if the implementation reads the outcome directly from the active battle, no second conflicting outcome source may exist
* failed acceptance does not increment `BattlesPlayed`
* failed acceptance does not clear the active battle
* one completed battle increments `BattlesPlayed` exactly once
* repeated acceptance cannot increment `BattlesPlayed` twice

#### Group 3: player-death run flow

Should test:

* `PlayerDefeated` enters `RunFlowStage.Defeat`
* player death clears `ActiveBattle`
* player death leaves `PendingRewardOffer == null`
* player death does not increment `RewardsClaimed`
* player death does not generate a reward offer
* player death does not settle remaining enemy rewards
* player death does not move to the next enemy
* player death does not enter `Victory`
* player death prevents starting another battle
* player death remains terminal after later invalid commands
* a fatal completed battle still increments `BattlesPlayed` exactly once

A small spy or fake around reward generation may be used to verify that reward generation was not requested.

#### Group 4: simultaneous zero

Should test:

* simultaneous zero is accepted as `BattleCompletionReason.PlayerDefeated`
* simultaneous zero enters `Defeat`
* simultaneous zero clears `ActiveBattle`
* simultaneous zero does not enter reward flow
* simultaneous zero does not increment `RewardsClaimed`
* simultaneous zero does not move to the next enemy
* simultaneous zero does not enter final-enemy `Victory`
* enemy final HP zero or below does not override the official player-defeat reason

#### Group 5: enemy defeated while player survives

Should test:

* `EnemyDefeated` is interpreted only when player final HP is above zero
* non-final enemy defeat enters the normal reward-settlement flow
* early enemy defeat settles remaining reward opportunities
* final enemy defeat ignores unresolved reward opportunities
* final enemy defeat enters `Victory`
* final enemy defeat leaves no active battle
* final enemy defeat leaves no pending reward offer

#### Group 6: battle-limit exhaustion

Should test:

* `AllRoundsCompleted` with `BattlesPlayed < battleLimit` enters reward flow
* after the eligible reward is resolved, the run becomes `ReadyForNextBattle`
* `AllRoundsCompleted` with `BattlesPlayed >= battleLimit` enters `Defeat`
* battle-limit exhaustion leaves `ActiveBattle == null`
* battle-limit exhaustion leaves `PendingRewardOffer == null`
* battle-limit exhaustion does not generate a reward for the exhausting battle
* battle-limit exhaustion does not increment `RewardsClaimed`
* battle-limit exhaustion remains functional independently of player-death rules

#### Group 7: reward progress invariants

Should test:

* `RewardsClaimed` changes only when a pending reward choice is resolved
* generating a reward offer does not itself increment `RewardsClaimed`
* choosing `Skip` increments `RewardsClaimed`
* player death never creates an immediately due reward
* battle-limit exhaustion never creates an immediately due reward
* an active battle and pending reward never coexist

---

## Player-Death Regression Matrix

The following matrix defines the minimum ownership of player-death regression scenarios.

| Scenario                              |       Resolver tests | BattleService tests |       RunService tests | Formatter / Presentation tests | Manual debug validation |
| ------------------------------------- | -------------------: | ------------------: | ---------------------: | -----------------------------: | ----------------------: |
| lethal damage with no healing         | raw consequence only |            required |               required |                       optional |                required |
| temporary zero recovered by Regrow    |             required |            required |           not required |                    recommended |                required |
| temporary zero recovered by Lifesteal |             required |            required |           not required |                    recommended |                required |
| healing leaves final HP exactly zero  |     raw healing only |            required |               required |                       optional |             recommended |
| healing clamp                         |     raw healing only |            required |           not required |                    recommended |             recommended |
| Post Resolve not skipped              |             required |            indirect |           not required |                   not required |             recommended |
| player death in round 1 or 2          |         not required |            required |               required |                    recommended |                required |
| simultaneous zero                     |         not required |            required |               required |                       required |                required |
| enemy defeated while player survives  |         not required |            required |               required |                       optional |             recommended |
| both survive round 3                  |         not required |            required |               required |                       optional |             recommended |
| no reward after player death          |         not required |        not required |               required |                       required |                required |
| terminal action gating                |         not required |        not required |  service gate required |                       required |                required |
| fatal result remains inspectable      |         not required |     result required | state cleanup required |                       required |                required |

### Matrix interpretation

The same full scenario does not need to be reconstructed at every layer.

Each test layer should verify only the responsibility it owns.

For example:

* resolver tests verify that raw Regrow healing still exists
* battle tests verify that the healing changes final HP
* run tests verify that final player death enters `Defeat`
* presentation tests verify that the result is displayed correctly

---

## PlayMode / Smoke

### `BootstrapSmokeTests.cs`

Should test:

* Unity-side bootstrap can load config
* a run can be created successfully

No player-death-specific bootstrap test is required.

### `BattleFlowSmokeTests.cs`

Should test:

* a battle can start
* one round can be played
* the finalized round result becomes visible
* Continue completes the presentation gate
* normal nonfatal flow can continue
* one fatal round can still be presented before run-level defeat is accepted
* after Continue on the fatal result, the run displays `Defeat`
* no gameplay action remains available after defeat
* the fatal round result remains inspectable after the active battle is cleared

### Scope rule

The fatal Play Mode smoke path validates:

* Unity wiring
* controller command chaining
* refresh behavior
* button gating
* retained result visibility

It must not replace the detailed Edit Mode matrix for:

* Regrow recovery
* Lifesteal recovery
* exact HP values
* simultaneous-zero priority
* reward counter invariants

---

## Minimal First-Wave Test Set

If implementation capacity needs to stay especially lean, the minimum high-value first-wave test set should be:

* `GameConfigLoaderTests.cs`
* `RuntimeStateFactoryTests.cs`
* `RewardCanonicalSignatureTests.cs`
* `RewardOfferCompositionTests.cs`
* `RewardDedupTests.cs`
* `RoundResolverCoreTests.cs`
* `RoundResolverMovementTests.cs`
* `RoundResolverPostResolveTests.cs`

This reduced set still covers the highest-risk logic.

For the current config-driven design direction, `RewardOfferCompositionTests.cs` should cover both:

* generalized reward-offer invariants
* default-baseline examples

This keeps the first-wave test set aligned with both the generalized reward structure and the current v1 default content baseline.

---

## Required Regression Set for Player-Death Implementation

When the player-death rule is implemented, the minimum required regression set is:

* `RoundResolverPostResolveTests.cs`
* `RoundResolverSnapshotTests.cs`
* `BattleServiceTests.cs`
* `RunServiceTests.cs`
* pure formatter tests where formatters exist
* manual debug validation

At minimum, this set must cover:

* temporary-zero Post Resolve execution
* temporary-zero recovery
* final HP exactly zero
* player death in an early round
* simultaneous zero
* fixed pending outcome before presentation
* player-death reward short circuit
* terminal run cleanup
* existing battle-limit defeat

This rule-change regression set is required even if the older minimal first-wave list is kept lean for historical implementation sequencing.

---

## Test Helper Strategy

The project should use lightweight test helpers, not a heavy custom test framework.

The first implementation phase should provide only the helpers that meaningfully reduce repeated setup code.

---

## Recommended First Test Helpers

## `TestConfigFactory`

### Purpose

Quickly build valid or intentionally invalid config objects for tests.

### Should support

* minimal valid `GameConfig`
* custom `TraitTuning`
* custom `EnemyConfig`
* custom `CardSpec`
* invalid config variants when needed

### Should not do

* gameplay orchestration
* loader behavior
* runtime mutation logic

---

## `TestCardFactory`

### Purpose

Quickly build `CardSpec` and `CardInstance` objects.

### Should support

* creating rock, scissors, or paper cards
* adding traits
* setting base power
* setting permanent power bonus when needed

### Should not do

* full run setup
* full battle setup
* reward generation

---

## `TestBattleFactory`

### Purpose

Quickly build battle-related object graphs.

### Should support

* empty battle creation
* lane setup
* open slot setup
* board card placement
* round index and flow stage setup
* custom player and enemy HP context
* custom round index
* custom battle flow stage
* custom pending battle outcome
* battle state already in `PresentingRoundResult`
* battle state already in `BattleComplete`
* finalized round-result history when needed

### Should not do

* run-level orchestration
* reward generation

---

## `TestRunFactory`

### Purpose

Quickly build valid runtime run state for application-level tests.

### Should support

* minimal valid `RunState`
* custom HP and max HP
* current enemy state setup
* flow stage setup
* custom `BattlesPlayed`
* custom `RewardsClaimed`
* custom enemy `battleLimit`
* active battle attachment
* pending reward attachment
* `Victory` and `Defeat` terminal states
* player and enemy HP combinations for simultaneous-zero scenarios

### Should not do

* battle-level rule execution
* reward legality logic

---

## `TestBattleOutcomeFactory`

### Purpose

Build valid immutable `BattleOutcome` instances for run-level tests without requiring full round resolution.

### Should support

* `PlayerDefeated`
* `EnemyDefeated`
* `AllRoundsCompleted`
* custom battle index
* custom rounds played
* custom final player HP
* custom final enemy HP
* simultaneous-zero final HP with official `PlayerDefeated` reason

### Important rule

The helper should produce only valid outcome combinations.

It must not make invalid states convenient, such as:

```
CompletionReason = EnemyDefeated
PlayerHpAfterBattle <= 0
```

A large outcome-builder framework is not required.

---

## `DeterministicRandom`

### Purpose

Provide predictable random behavior for tests.

### Why it is needed

The current project includes randomness in:

* enemy battle sequence preparation
* reward candidate selection

Tests should not rely on uncontrolled randomness.

### Required first-wave capability

It is sufficient if it supports:

* fixed seed behavior
  or
* fixed ordered outputs

A heavy random abstraction is not required yet.

---

## `DomainAssert`

### Purpose

Provide a very small number of high-value assertion helpers.

### Recommended first-wave scope

* compare trait sets ignoring order
* compare canonical deck equivalence
* assert flow stage
* assert HP values

### Important rule

Do not build a large assertion library.
Only add helpers that clearly improve readability.

---

## Helper Priority

The recommended helper implementation priority is:

### Highest priority

* `TestConfigFactory`
* `TestCardFactory`

### Medium priority

* `TestBattleFactory`

### Lower priority

* `TestRunFactory`
* `TestBattleOutcomeFactory`
* `DeterministicRandom`

### Optional and very light

* `DomainAssert`

This order reflects the fact that the earliest high-value tests mainly target config, reward logic, and round resolution.

---

## Production Code Requirements for Testability

The testing strategy assumes the production code follows these rules:

### 1. Explicit dependencies

Dependencies such as random behavior should be injected or otherwise made replaceable in tests.

### 2. Thin Unity loading edge

Config loading should remain outside the core gameplay logic.

### 3. Pure or mostly pure rule evaluation where possible

`RoundResolver` should remain highly testable.

### 4. Explicit result objects

`RoundResult`, `BattleOutcome`, `BattleCommandResult`, and `RunCommandResult` should remain explicit rather than hidden behind side-effect-only APIs.

These rules are important for both maintainability and Codex task quality.

### 5. Stable outcome observation

Tests must be able to inspect `BattleState.PendingBattleOutcome` before presentation completion.

### 6. Reward-call observation

Run-level tests must be able to verify that reward generation is not requested for:

* player death
* simultaneous zero
* battle-limit exhaustion

Use the smallest practical seam, such as:

* an existing reward-service abstraction
* a lightweight fake
* a lightweight spy

Do not introduce a general mocking framework solely for this purpose.

### 7. Finalized result observation

Tests must be able to inspect:

* HP before
* HP after merged damage
* raw healing
* actual healing applied
* final HP

without parsing Presentation strings.

### 8. Presentation remains replaceable

Pure formatting and view-data construction should remain testable without loading a Unity scene where practical.

---

## What Not to Build Yet

The project should not build the following too early:

* a large custom testing framework
* UI automation suites
* deep golden-file snapshot systems
* a large scenario DSL
* large combinatorial parameterization systems
* heavy fake service graphs
* broad Play Mode integration suites

These would add cost without matching the current demo phase priorities.

---

## How Tests Should Pair with Codex Implementation Batches

Each major Codex implementation batch should include its most relevant Edit Mode tests.

This is a required principle of the current workflow.

---

## Batch 1: Config and Runtime Construction

### Production scope

* `GameConfig`
* `PlayerStartConfig`
* `EnemyConfig`
* `RewardGenerationConfig`
* `TraitTuning`
* `CardSpec`
* `GameConfigLoader`
* `RuntimeStateFactory`

### Required tests

* `GameConfigLoaderTests.cs`
* `RuntimeStateFactoryTests.cs`

Validation coverage for this batch should include config-driven quantity and bound rules such as:

* enemy count lower bound
* starting deck size lower bound
* enemy fixed deck size lower bound
* per-enemy battle limit lower bound
* reward offer total option lower bound
* upgrade target upper-bound relationship to total offer size
* replacement trait count allowed range

### Required helpers

* `TestConfigFactory`
* `TestCardFactory`

---

## Batch 2: Domain Core State Objects

### Production scope

* `RunState`
* `EnemyProgressState`
* `BattleState`
* `LaneState`
* `BoardSlotState`
* `CardInstance`
* `BoardCard`
* `RewardOffer`
* `RewardOption`
* payload objects
* `RoundResult`
* `SlotCombatResult`
* `PhaseSnapshot`
* `BattleOutcome`

### Required tests

This batch may include light sanity tests for:

* `BattleCompletionReason`
* valid `BattleOutcome` construction
* derived `PlayerDefeated` and `EnemyDefeated` properties
* simultaneous-zero representation
* nullable `BattleState.PendingBattleOutcome`

The main behavioral coverage still belongs to later resolver, battle-service, and run-service batches.

### Required helpers

* `TestCardFactory`
* `TestBattleFactory`

---

## Batch 3: Reward Canonicalization, Generation, and Application

### Production scope

* canonical card signature
* canonical deck signature
* reward legality checks
* reward generation
* reward application

### Required tests

* `RewardCanonicalSignatureTests.cs`
* `RewardUpgradeGenerationTests.cs`
* `RewardReplaceGenerationTests.cs`
* `RewardOfferCompositionTests.cs`
* `RewardDedupTests.cs`
* `RewardApplicationTests.cs`

Reward-generation tests in this batch should cover both:

* generalized config-driven offer structure and replacement-generation behavior
* the current v1 default baseline behavior

### Required helpers

* `TestConfigFactory`
* `TestCardFactory`
* `DeterministicRandom`
* optional small `DomainAssert`

---

## Batch 4: RoundResolver

### Production scope

* `RoundResolver`
* phase order
* logs
* snapshots

### Required tests

* `RoundResolverCoreTests.cs`
* `RoundResolverFixedSelfTests.cs`
* `RoundResolverMovementTests.cs`
* `RoundResolverBoardDerivedTests.cs`
* `RoundResolverPostResolveTests.cs`
* `RoundResolverSnapshotTests.cs`

`RoundResolverPostResolveTests.cs` must explicitly cover:

* projected lethal damage does not stop Phase 7
* Regrow still produces raw healing
* Lifesteal still produces raw healing
* Growth still executes
* authoritative HP is not mutated
* survival is not classified

`RoundResolverSnapshotTests.cs` must explicitly cover:

* all seven snapshots in projected-lethal scenarios
* no eighth survival-classification phase

### Required helpers

* `TestCardFactory`
* `TestBattleFactory`
* `TestConfigFactory`

---

## Batch 5: BattleService

### Production scope

* `StartBattle(...)`
* `SubmitPlayerCard(...)`
* authoritative HP application
* `RoundResult` finalization
* battle-completion classification
* `PendingBattleOutcome`
* `FinishRoundPresentation(...)`
* `BattleCommandResult`
* `BattleCompletionReason`
* `BattleOutcome`

### Required tests

* `BattleServiceTests.cs`

Required coverage includes:

* damage, healing, and clamp order
* HP after merged damage
* actual healing applied
* final HP
* temporary-zero recovery
* final HP exactly zero
* player death in rounds 1 and 2
* enemy defeat while player survives
* simultaneous-zero priority
* all-round completion
* pending-outcome creation before presentation
* stable outcome through presentation
* nonfatal next-round progression
* duplicate presentation-finish protection

### Required helpers

* `TestBattleFactory`
* `TestRunFactory`
* `TestCardFactory`
* the smallest practical deterministic resolver setup
* `DeterministicRandom` where battle startup requires it

### Important rule

This batch must not defer gameplay-outcome classification to Presentation or controller code.

---

## Batch 6: RunService

### Production scope

* `CreateNewRun(...)`
* `CanStartNextBattle(...)`
* `StartNextBattle(...)`
* `AcceptCompletedBattle(...)`
* player-death defeat flow
* battle-limit defeat flow
* reward eligibility
* `ChooseReward(...)`
* `RunCommandResult`

### Required tests

* `RunServiceTests.cs`

Run-flow tests in this batch must explicitly cover:

* acceptance of the authoritative pending outcome
* `BattlesPlayed` increments exactly once
* player death enters `Defeat`
* player death clears active battle
* player death creates no reward
* player death does not increase reward progress
* simultaneous zero does not enter reward or victory flow
* enemy defeat while player survives retains normal reward progression
* configurable per-enemy `battleLimit`
* battle-limit exhaustion defeat
* no reward for the exhausting battle
* early reward settlement on enemy defeat
* final-enemy remainder-ignore behavior
* terminal action gating after `Defeat`

### Required helpers

* `TestRunFactory`
* `TestConfigFactory`
* `TestBattleOutcomeFactory`
* a small reward-service spy or equivalent observation seam

### Important rule

The official branch order must be tested as:

1. `PlayerDefeated`
2. `EnemyDefeated`
3. `AllRoundsCompleted` with battle limit exhausted
4. `AllRoundsCompleted` with more battles remaining

---

## Batch 7: Debug-Oriented Presentation

### Production scope

* debug scene skeleton
* `DebugSceneController`
* panel view components
* view data objects
* formatter helpers
* command-driven full refresh
* runtime-generated repeated UI entries
* button state rules
* status message line
* `TextAsset` loading entry wiring
* manual-validation-ready debug scene behavior

### Required tests and verification

* manual validation against the scenarios defined in `debug-ui-plan.md`

* optional light Edit Mode tests for pure formatter or view-data builders

* formatter coverage for:

  * HP before
  * merged damage
  * HP after merged damage
  * raw healing
  * actual healing
  * final HP
  * player-death status
  * simultaneous-zero status

* button-state verification for:

  * `PresentingRoundResult`
  * `BattleComplete`
  * `Defeat`

* verification that the last fatal round remains visible after active-battle cleanup

* no heavy Play Mode coverage is required for fine-grained rule behavior

### Required helpers and dependencies

* fixed scene skeleton authored in Unity
* prefabs or templates for repeated entries
* previously implemented services and runtime state objects
* stable scene references for controller wiring

### Important rule

Presentation tests verify display and action gating.

They must not recreate player-death, simultaneous-zero, reward-eligibility, victory, or defeat rules inside formatter or controller code.

This batch implements debug-oriented presentation, not production UI polish.

---

## Batch 8: Minimal Play Mode Smoke Tests and Iteration

### Production scope

* `BootstrapSmokeTests.cs`
* `BattleFlowSmokeTests.cs`
* optional later `RewardFlowSmokeTests.cs`
* small debug UI fixes driven by smoke results and manual validation

### Required tests

* minimal Play Mode smoke coverage for:

  * config bootstrap
  * new run creation
  * battle start
  * card selection
  * round result visibility
  * continue flow
  * optional reward path later
  * one fatal-round presentation path
  * Continue handoff from the fatal result
  * final `Defeat` stage display
  * no enabled gameplay action after defeat
  * retained visibility of the last round result

### Required helpers and dependencies

* stable button references
* stable key text references
* stable panel visibility rules
* a working debug scene from Batch 7

### Important rule

This batch validates runtime wiring only.
Gameplay rule correctness remains primarily the responsibility of Edit Mode tests.

---

## Assertion Style Guidance

Tests should prefer asserting gameplay-relevant facts and explicit flow outcomes.

Good assertions include:

* final HP values
* max HP clamp behavior
* slot winner
* resulting trait sets
* canonical deck equivalence
* current flow stage
* reward offer composition
* HP after merged damage
* raw healing
* actual healing applied
* `BattleCompletionReason`
* pending battle outcome presence
* pending outcome stability through presentation
* `ActiveBattle == null` after run-level acceptance
* `PendingRewardOffer == null` after player death
* unchanged `RewardsClaimed` after player death
* terminal flow stage
* reward generation not requested for ineligible outcomes

Tests should avoid overdependence on:

* private implementation details
* exact helper call counts
* large brittle string-log equality unless clearly necessary

The goal is to keep tests resilient while still meaningful.

Tests may assert that a collaborator was not called when the absence of that call is itself a gameplay invariant, such as proving that no reward offer is generated after player death.

Avoid asserting unrelated internal call order when state and outcome assertions are sufficient.

---

## Manual Validation Workflow

Automated tests do not replace manual validation.

The debug scene should be used for:

* visually inspecting normal flow progression
* checking log readability
* checking snapshot usefulness
* checking reward readability
* inspecting HP before damage, HP after merged damage, healing, and final HP
* checking temporary-zero recovery
* checking player-death presentation
* checking simultaneous-zero presentation
* checking terminal button state
* checking that the last fatal result remains inspectable

### Required player-death scenarios

#### Scenario 1: lethal damage without recovery

Verify:

* all seven phases were resolved
* final player HP is zero or below
* the round result is presented
* Continue remains the only relevant battle action during presentation
* after Continue, run stage becomes `Defeat`
* no reward appears
* no next battle action is available

#### Scenario 2: temporary zero recovered by Regrow

Verify:

* HP after merged damage is zero or below
* raw Regrow healing is shown
* actual healing is shown
* final HP is above zero
* the player does not enter `Defeat`
* battle flow continues according to round and enemy state

#### Scenario 3: temporary zero recovered by Lifesteal

Verify:

* the new card dealt attributable damage
* HP after merged damage is zero or below
* Lifesteal healing is shown
* final HP is above zero
* the player survives

#### Scenario 4: final HP exactly zero

Verify:

* final HP displays exactly zero
* the official result is player defeat
* no reward appears

#### Scenario 5: simultaneous zero

Verify:

* both final HP values are zero or below
* the status clearly identifies player defeat
* the UI does not describe the enemy as officially defeated
* no reward appears
* final-enemy simultaneous zero does not show `Victory`

#### Scenario 6: battle-limit exhaustion

Verify:

* the existing non-death defeat path still works
* the exhausting battle does not generate a reward
* the status distinguishes battle-limit defeat from player-death defeat

### Intended validation loop

1. run relevant resolver Edit Mode tests
2. run relevant BattleService Edit Mode tests
3. run relevant RunService Edit Mode tests
4. open the debug scene
5. inspect the scenarios above
6. run the minimal fatal Play Mode smoke path after scene behavior is stable
7. fix presentation or wiring issues without moving gameplay rules into the UI layer

---

## Definition of Success for the Current Testing Strategy

The testing strategy is successful if:

* core rule regressions are caught quickly
* reward duplication or legality bugs are caught quickly
* config mistakes fail early
* battle and run flow bugs are visible early
* Codex tasks can be scoped together with meaningful validation
* the project avoids large up-front testing overhead
* the debug scene becomes a useful manual validation workbench
* minimal Play Mode smoke tests can validate runtime wiring without taking over rule verification
* temporary HP at zero or below cannot silently skip Post Resolve
* final HP and battle outcome cannot diverge
* simultaneous zero cannot produce enemy-defeat or victory progression
* player death cannot generate or advance rewards
* `FinishRoundPresentation(...)` cannot alter a fixed gameplay outcome
* both official run-defeat sources remain covered
* fatal round results remain inspectable in the debug workbench

This is the intended balance for the current demo phase.

---

## Summary

The current testing strategy is:

* Edit Mode tests as the primary automated layer
* Play Mode smoke tests as a minimal runtime-integration layer
* manual debug-scene validation as a required inspection layer

The main automated responsibility split is:

* `RoundResolverPostResolveTests` verify complete seven-phase rule execution and raw post-resolve consequences
* `BattleServiceTests` verify authoritative HP application, clamp, final HP, and one official battle-completion reason
* `RunServiceTests` verify run progression, player-death priority, reward short circuit, and terminal cleanup
* formatter and Presentation tests verify display and action gating without implementing gameplay rules
* Play Mode smoke tests verify controller and scene wiring only

The player-death regression strategy specifically protects:

* temporary-zero recovery
* final HP exactly zero
* early battle completion from player death
* simultaneous-zero priority
* fixed pending outcome before presentation completion
* no reward after player death
* no reward after battle-limit exhaustion
* terminal `Defeat` behavior
* retained visibility of the fatal round result

The strategy remains intentionally lightweight, explicit, and aligned with incremental Codex-assisted implementation.