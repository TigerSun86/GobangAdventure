# Config and Content

## Purpose

This document defines how authored game content and configuration are represented, loaded, validated, and converted into runtime state for the Unity 6.4 2D turn-based card roguelike demo.

It explains:

* why the project uses JSON-based authored config
* which config objects exist
* which data belongs in config and which belongs in runtime state
* which quantities are derived from config collections rather than stored as separate source-of-truth fields
* how config is loaded
* how config is validated
* how config is converted into runtime state
* how trait tuning values are represented
* how this design supports the current demo while leaving room for future expansion

This document focuses on authored content and configuration structure. Runtime state behavior is described in:

* `domain-model.md`
* `round-resolution.md`
* `run-battle-reward-flow.md`
* `reward-generation.md`

---

## Design Goals

The current config and content system is designed to satisfy these goals:

1. keep domain and runtime logic as Unity-agnostic as practical
2. keep authored content readable in source control
3. keep configuration easy to inspect and edit in VS Code
4. avoid unnecessary editor-only workflows in the current demo phase
5. support data-driven balancing where it helps
6. keep the first implementation simple
7. separate locked gameplay invariants from default content scale
8. leave a clear path for future expansion

---

## High-Level Design

The project uses a JSON-first authored content pipeline.

The configuration system is split into three layers:

### 1. Authored Config

Human-authored JSON files stored in the project.

### 2. Config Objects

Pure C# objects deserialized from JSON.

### 3. Runtime State

Pure C# runtime objects used by gameplay systems during the run.

This separation is intentional.

---

## Why JSON Is Used in the Current Demo

The current demo uses JSON as the main authored config format instead of ScriptableObject-based primary authored content.

This choice supports the current goals:

* clearer Git diffs
* easier editing in VS Code
* better alignment with Codex-based implementation workflow
* less dependence on Unity-specific asset authoring during early development

A separate ADR records the full reasoning for this decision.

---

## Core Modeling Rule: Locked Invariants vs Default Content Scale

The current gameplay design distinguishes between:

1. **locked gameplay invariants**
2. **default content scale and default configuration values**

### Locked gameplay invariants include

* battle still has exactly 3 rounds
* the board still has 3 slots per side
* the base card types are still rock, scissors, and paper
* the accepted round pipeline is still fixed
* movement is still processed from right to left
* reward selection is still one-step
* `Skip` is still always present in a reward offer
* card trait legality rules are still enforced

### Default content scale includes

* default enemy count
* default per-enemy battle limit
* default starting deck size
* default enemy fixed deck size
* default reward offer size
* default upgrade target limit
* default replacement trait count

These defaults are important for the v1 baseline, but they are not the same thing as schema-level hard constraints.

This document reflects that distinction.

---

## What Configuration Should Contain

Configuration should contain information that is known before runtime and should be easy to author and review.

Examples:

* player max HP
* the starting player deck
* enemy max HP
* each enemy's battle limit
* each enemy's fixed deck
* reward generation rules
* numerical trait tuning values

Configuration should not contain:

* current run HP
* current battle board state
* current reward offer
* current enemy progression
* current card instance identity
* current round-local combat values

Those belong to runtime state, not configuration.

---

## Main Config Objects

The current config model uses these main objects:

* `GameConfig`
* `PlayerStartConfig`
* `EnemyConfig`
* `RewardGenerationConfig`
* `TraitTuning`
* `CardSpec`

These are all pure C# config objects intended to be serialized from JSON.

---

## Default Baseline for v1 Content

The current v1 demo uses the following **default baseline values**:

* default enemy count: 3
* default enemy battle limit: 3
* default starting deck size: 6
* default enemy fixed deck size: 6
* default reward offer total options: 4
* default reward upgrade target: 2
* default replacement trait count: 2

These defaults are useful for baseline content authoring and test expectations, but they should not be treated as universal hard-coded system constants.

Where practical, engineering documents and runtime systems should read actual values from config or from config-derived runtime state rather than silently assuming these defaults.

---

## GameConfig

### Purpose

`GameConfig` is the top-level root object of the authored config.

It groups all major configuration areas needed to start and run the demo.

### Recommended Fields

* `PlayerStartConfig playerStart`
* `List<EnemyConfig> enemies`
* `RewardGenerationConfig rewardGeneration`
* `TraitTuning traitTuning`

### Notes

* `GameConfig` is the root object deserialized from the main JSON file.
* It should remain a simple aggregator of configuration sections.
* It is not runtime state.
* enemy count should be derived from `enemies.Count`, not stored as a second explicit field.

---

## PlayerStartConfig

### Purpose

`PlayerStartConfig` defines the player's starting conditions at the beginning of a run.

### Recommended Fields

* `int playerMaxHp`
* `List<CardSpec> startingDeck`

### Rules

* `playerMaxHp` is the player's maximum HP
* the player's starting current HP is initialized from `playerMaxHp`
* `startingDeck` defines the actual starting deck contents
* starting deck size is derived from `startingDeck.Count`

### Current Baseline

* the current default starting deck size is 6
* the current battle structure still requires at least 3 cards to support one three-round battle without reusing a card

### Important Naming Note

The current design uses `playerMaxHp`, not `playerStartHp`.

This is intentional:

* the config value represents a max HP rule value
* runtime current HP is initialized from that max HP
* healing must not exceed max HP

---

## EnemyConfig

### Purpose

`EnemyConfig` defines one enemy's authored content.

### Recommended Fields

* `string enemyId`
* `string displayName`
* `int maxHp`
* `int battleLimit`
* `List<CardSpec> fixedDeck`

### Rules

* `maxHp` defines the enemy's maximum HP for the current enemy stage
* `battleLimit` defines how many battles the player may take against this enemy before defeat
* `fixedDeck` defines the enemy's authored fixed card pool
* enemy fixed deck size is derived from `fixedDeck.Count`

### Current Baseline

* the current default enemy battle limit is 3
* the current default enemy fixed deck size is 6
* because battle still uses 3 rounds, the fixed deck must still be large enough to produce a full 3-card enemy sequence for each battle

### Notes

* enemy cards are authored explicitly in config for the current demo
* the current demo does not use a procedural enemy deck generation system
* reward total for an enemy should default to that enemy's configured `battleLimit`
* do not introduce a second reward-count authored field for the current demo

---

## RewardGenerationConfig

### Purpose

`RewardGenerationConfig` defines how reward offers are structured and how replacement card generation works.

It controls:

* the legal generation space for replacement cards
* the default total size of one reward offer
* the target upper bound for upgrade options inside one offer

### Recommended Fields

* `int rewardOfferTotalOptions`
* `int upgradeTarget`
* `List<RpsType> allowedReplacementRpsTypes`
* `List<int> allowedReplacementBasePowers`
* `List<TraitType> allowedReplacementTraits`
* `int replacementTraitCount`

### Rules

* `rewardOfferTotalOptions` is the total number of options shown in one reward offer
* `Skip` is always exactly 1 option and is not configured separately
* `upgradeTarget` is the configured upper bound for how many upgrade options the generator tries to include before using replace options to fill the remaining non-skip slots
* replace count is not configured as an independent source of truth
* actual replace count is derived at runtime from:

`rewardOfferTotalOptions - 1 - actualUpgradeCount`

* `replacementTraitCount` defines how many traits a generated replacement card spec must contain

### Current Baseline

* default reward offer total options: 4
* default upgrade target: 2
* default replacement trait count: 2

### Current Allowed Test Range

For the current test-oriented phase:

* `replacementTraitCount` is not treated as permanently fixed to 2
* the currently allowed test range is 0 to 3 inclusive

### Why This Object Exists

Reward offer structure and replacement generation both belong to authored content rules.

The current design wants:

* a fixed `Skip` rule
* a configurable default offer size
* a configurable default upgrade target
* replacement generation from legal authored rules rather than from a fully hand-authored replacement catalog

This object defines those authored rules.

---

## TraitTuning

### Purpose

`TraitTuning` stores numerical tuning values for traits whose behavior already exists in code.

This is intentionally not a full trait-definition system.

### Recommended Fields

* `int empowerBonus`
* `int adjacentAidBonus`
* `int suppressPenalty`
* `int regrowHeal`
* `int growthBonus`

### What Is Intentionally Not Here

`TraitTuning` does not define:

* trait phase ownership
* trait behavior type
* targeting rules
* general effect scripting
* lifesteal behavior schema

Those remain hard-coded in gameplay logic for the current demo.

### Why This Design Is Used

The project wants the convenience of config-based numerical tuning without introducing a full data-driven trait behavior system too early.

This keeps the current demo simple while still making balancing easier.

---

## CardSpec

### Purpose

`CardSpec` describes the static specification of one card.

It is the shared config representation used by:

* player starting deck entries
* enemy fixed deck entries
* generated replacement card specs

### Recommended Fields

* `RpsType rpsType`
* `int basePower`
* `List<TraitType> traits`

### Why CardSpec Exists

`CardSpec` is used instead of a heavier `CardDefinition` concept.

This keeps card authorship simple and supports all current card sources with one format:

* authored player cards
* authored enemy cards
* generated replacement cards

### Why CardSpec Does Not Include Runtime Data

`CardSpec` should not contain:

* instance identity
* current HP
* permanent growth accumulated during a run
* board position
* battle usage state

Those belong to runtime objects.

### Display Name Note

The current design does not require a stored display name in `CardSpec`.

Readable card text should be generated by presentation or debug formatting code from:

* RPS type
* base power
* trait list

This avoids redundant authored text.

---

## Derived Quantity Rules

The current config model deliberately derives several quantities from primary authored collections or fields rather than duplicating them in separate source-of-truth values.

### Enemy count

Enemy count is derived from:

* `GameConfig.enemies.Count`

Do not add a second top-level enemy-count field for the current demo.

### Starting deck size

Starting deck size is derived from:

* `PlayerStartConfig.startingDeck.Count`

Do not add a separate `playerCardSize` field for the current demo.

### Enemy fixed deck size

Enemy fixed deck size is derived from:

* `EnemyConfig.fixedDeck.Count`

Do not add a second explicit fixed-deck-size field for the current demo.

### Reward total per enemy

Reward total per enemy defaults to:

* that enemy's configured `battleLimit`

Do not add a second independent authored reward-count field for the current demo.

### Replace count inside an offer

Replace count is derived from:

* `rewardOfferTotalOptions`
* the fixed single `Skip`
* the actual number of upgrade options selected

Do not store replace count as a separate authored source-of-truth field for the current demo.

---

## Config Authoring Model

The current design assumes:

* one main JSON file is enough for the first implementation
* future splitting into multiple JSON files is allowed if needed later

A likely early setup is:

* one main `game_config.json`

This keeps the first implementation simple.

---

## JSON Structure Expectations

The JSON should deserialize into a single root `GameConfig` object.

The config structure should remain:

* explicit
* object-based
* simple
* compatible with Unity `JsonUtility`

This means the current config design should avoid:

* dictionary-heavy schemas
* polymorphic config graphs
* deep inheritance in config data
* hidden authored content embedded inside services

---

## Loader Boundary

The config loading boundary should stay thin.

### Recommended Unity-facing entry

A Unity-facing object such as a bootstrap MonoBehaviour or debug controller may provide authored JSON text through:

* `TextAsset`
* direct file text in future tooling
* another thin file-loading edge if later needed

### Recommended loader object

Use:

* `GameConfigLoader`

### Loader responsibilities

`GameConfigLoader` should:

* receive JSON text
* deserialize it into `GameConfig`
* validate the config
* return the validated config object
* report failure cleanly if validation fails

### Loader should not

* own runtime state
* construct gameplay state directly
* mutate gameplay state
* contain gameplay rules beyond lightweight config validation

---

## Validation Rules

Config validation should be lightweight but explicit.

The goal is to catch content mistakes early while keeping the config schema easy to evolve for testing.

### GameConfig validation

* `playerStart` must exist
* `enemies` must exist
* `enemies.Count >= 1`
* `rewardGeneration` must exist
* `traitTuning` must exist

### PlayerStartConfig validation

* `playerMaxHp > 0`
* `startingDeck` must exist
* `startingDeck.Count >= 3`

### EnemyConfig validation

* `enemyId` must not be empty
* `maxHp > 0`
* `battleLimit >= 1`
* `fixedDeck` must exist
* `fixedDeck.Count >= 3`

### RewardGenerationConfig validation

* `rewardOfferTotalOptions >= 2`
* `upgradeTarget >= 0`
* `upgradeTarget <= rewardOfferTotalOptions - 1`
* `allowedReplacementRpsTypes` must exist and contain at least one value
* `allowedReplacementBasePowers` must exist and contain at least one value
* `allowedReplacementTraits` must exist and contain at least one value
* `replacementTraitCount >= 0`
* `replacementTraitCount <= 3`

### TraitTuning validation

* all numerical fields must be non-negative

### CardSpec validation

* `basePower >= 0`
* `traits` must exist
* traits must not contain duplicates
* traits must not contain both `ShiftLeft` and `ShiftRight`
* authored card trait count must not exceed 3

### Validation Notes

These are schema and legality checks, not balance checks.

They intentionally distinguish:

* hard minimum validity requirements
  from
* default content baselines such as 3 enemies or 6-card starting decks

---

## Runtime Construction Boundary

The current design intentionally separates:

* config loading
  from
* runtime construction

### Recommended constructor object

Use:

* `RuntimeStateFactory`

### RuntimeStateFactory responsibilities

It should convert validated config objects into runtime state objects.

Examples:

* `GameConfig` -> `RunState`
* `EnemyConfig` -> `EnemyProgressState`
* `CardSpec` -> `CardInstance`

### It should not

* decide run progression
* decide battle progression
* decide reward timing
* contain UI logic

Those belong elsewhere.

---

## Conversion Rules

The following conversion rules are part of the current design.

### CardSpec to CardInstance

When converting `CardSpec` into `CardInstance`:

* copy `rpsType`
* copy `basePower`
* copy traits
* assign a new runtime `InstanceId`
* set `PermanentPowerBonus = 0`

### EnemyConfig to EnemyProgressState

When converting `EnemyConfig` into `EnemyProgressState`:

* copy or reference the static enemy config
* set `CurrentHp = maxHp`
* set `MaxHp = maxHp`
* set `BattlesPlayed = 0`
* set `RewardsClaimed = 0`

### GameConfig to RunState

When creating a new run:

* set `PlayerHp = playerStart.playerMaxHp`
* set `PlayerMaxHp = playerStart.playerMaxHp`
* create player deck instances from `startingDeck`
* set `CurrentEnemyIndex = 0`
* create the first `EnemyProgressState` from the first enemy config
* set `ActiveBattle = null`
* set `PendingRewardOffer = null`
* set run flow to the stage that means the first battle can begin

### Important conversion note

Runtime construction should not separately author a reward-total field for the current enemy if that total can be derived from the enemy config's `battleLimit`.

The current design prefers one clear source of truth.

---

## Max HP Design Rule

The current design explicitly separates:

* max HP from authored config
* current HP in runtime state

### Player

`PlayerStartConfig.playerMaxHp` is copied into:

* `RunState.PlayerHp`
* `RunState.PlayerMaxHp`

### Enemy

`EnemyConfig.maxHp` is copied into:

* `EnemyProgressState.CurrentHp`
* `EnemyProgressState.MaxHp`

### Important Rule

Runtime systems should not repeatedly consult config to know current max HP.

Max HP should already exist in runtime state.

This keeps runtime state self-contained and easier to reason about.

---

## Healing Clamp Rule

The current design requires:

* healing cannot raise current HP above max HP

This applies to:

* player HP
* enemy HP

### Responsibility Boundary

* `RoundResolver` calculates raw healing totals
* `BattleService` applies healing to runtime HP and clamps to max HP

This keeps round calculation and runtime application responsibilities separate.

---

## Why Traits Are Not Fully Config-Driven

The current design intentionally avoids a full trait-definition config system.

The project does not currently want:

* generic trait scripting
* trait polymorphism in config
* behavior schemas for each trait
* config-driven phase binding
* a full effect interpreter

Instead, the design is:

* trait behavior stays in gameplay code
* small numerical values move into config

This keeps the current demo simpler and reduces overengineering.

---

## Why ScriptableObject Is Not the Main Authored Source

The current design intentionally avoids using ScriptableObject as the primary authored content format for this demo.

The chosen approach favors:

* text-readable source control diffs
* editor-independent authored content review
* easier VS Code and Codex workflows
* reduced Unity coupling in the config pipeline

A dedicated ADR records the full reasoning.

---

## What This Config System Intentionally Avoids

The current design intentionally avoids:

* per-card ScriptableObject assets
* a large authored card catalog
* a trait-definition asset system
* configuration dictionaries
* polymorphic JSON content graphs
* config-owned runtime state
* loader-owned gameplay logic
* duplicated source-of-truth fields for quantities that can be derived directly from primary authored collections or fields

These are not needed for the current demo phase.

---

## Extension Points

The current design leaves room for future expansion such as:

* splitting config into multiple JSON files
* replacing `JsonUtility` with another serializer
* supporting multiple replacement base powers
* supporting richer reward generation profiles
* supporting different default offer sizes or upgrade targets
* adding more enemy content
* adding more tuning values
* introducing stronger editor tooling later if needed

These future changes should preserve the current separation:

* authored config
* config objects
* runtime state

---

## Summary

The current config and content design is:

* JSON-first
* pure C# config objects
* Unity only at the loading edge
* explicit conversion into runtime state
* simple enough for the current demo
* extensible enough for future growth
* explicit about the difference between locked gameplay invariants and configurable default content scale

The key objects are:

* `GameConfig`
* `PlayerStartConfig`
* `EnemyConfig`
* `RewardGenerationConfig`
* `TraitTuning`
* `CardSpec`
* `GameConfigLoader`
* `RuntimeStateFactory`

This structure supports a data-driven demo without introducing unnecessary complexity too early.
