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

This document is intended to support:

* fast iteration
* early bug discovery
* clear Codex task scoping
* confidence in core gameplay correctness

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

### `RuntimeStateFactoryTests.cs`

Should test:

* `GameConfig` creates a valid `RunState`
* `CardSpec` creates a valid `CardInstance`
* `EnemyConfig` creates a valid `EnemyProgressState`
* player max HP is copied into runtime state
* enemy max HP is copied into runtime state

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

* `Regrow`
* `Lifesteal`
* `Growth`
* post-resolve only affects newly played cards
* `Growth` updates persistent source card state

### `RoundResolverSnapshotTests.cs`

Should test:

* snapshot count
* snapshot phase order
* presence of snapshots for all phases

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
* replacement trait count rule
* use of authored replacement config
* filtering of meaningless replacements

### `RewardOfferCompositionTests.cs`

Should test:

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

Should test:

* battle startup
* valid player card submission
* invalid player card rejection
* round result application
* healing clamp to max HP
* flow transition to next round or battle complete

### `RunServiceTests.cs`

Should test:

* run creation
* legal and illegal battle start conditions
* battle outcome interpretation
* reward flow entry
* reward choice progression
* next enemy progression
* victory
* defeat

---

## PlayMode / Smoke

### `BootstrapSmokeTests.cs`

Should test:

* Unity-side bootstrap can load config
* a run can be created successfully

### `DebugFlowSmokeTests.cs`

Should test:

* a battle can start
* one round can be played
* result presentation can complete
* flow can continue

These Play Mode tests may be implemented after the first Edit Mode-heavy batches are stable.

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

### Should not do

* battle-level rule execution
* reward legality logic

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

This batch may include only light sanity tests if needed.
Its main purpose is to enable later resolver, reward, and flow tests.

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

### Required helpers

* `TestCardFactory`
* `TestBattleFactory`
* `TestConfigFactory`

---

## Batch 5: BattleService

### Production scope

* `StartBattle(...)`
* `SubmitPlayerCard(...)`
* `FinishRoundPresentation(...)`
* `BattleCommandResult`

### Required tests

* `BattleServiceTests.cs`

### Required helpers

* `TestBattleFactory`
* `TestRunFactory`
* `DeterministicRandom`

---

## Batch 6: RunService

### Production scope

* `CreateNewRun(...)`
* `CanStartNextBattle(...)`
* `StartNextBattle(...)`
* `AcceptCompletedBattle(...)`
* `ChooseReward(...)`
* `RunCommandResult`

### Required tests

* `RunServiceTests.cs`

### Required helpers

* `TestRunFactory`
* `TestConfigFactory`
* optional small fake outcome builders when useful

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

Tests should avoid overdependence on:

* private implementation details
* exact helper call counts
* large brittle string-log equality unless clearly necessary

The goal is to keep tests resilient while still meaningful.

---

## Manual Validation Workflow

Automated tests do not replace manual validation.

The debug scene should be used for:

* visually inspecting flow progression
* checking log readability
* checking snapshot usefulness
* checking reward readability
* spotting friction in interaction and debugging workflows

The intended validation loop is:

1. run relevant Edit Mode tests
2. run minimal Play Mode smoke tests when appropriate
3. manually inspect the debug scene when behavior clarity matters

---

## Definition of Success for the Current Testing Strategy

The testing strategy is successful if:

* core rule regressions are caught quickly
* reward duplication or legality bugs are caught quickly
* config mistakes fail early
* battle and run flow bugs are visible early
* Codex tasks can be scoped together with meaningful validation
* the project avoids large up-front testing overhead

This is the intended balance for the current demo phase.

---

## Summary

The current testing strategy is:

* Edit Mode tests as the primary automated layer
* Play Mode smoke tests as a minimal runtime integration layer
* manual debug-scene validation as a required inspection layer

The first-wave emphasis is on:

* config loading and runtime construction
* reward canonicalization and generation
* round resolution
* battle and run flow correctness

The strategy is intentionally lightweight, explicit, and aligned with incremental Codex-assisted implementation.