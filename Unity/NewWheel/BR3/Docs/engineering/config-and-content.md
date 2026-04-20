# Config and Content

## Purpose

This document defines how authored game content and configuration are represented, loaded, validated, and converted into runtime state for the Unity 6.4 2D turn-based card roguelike demo.

It explains:

* why the project uses JSON-based authored config
* which config objects exist
* which data belongs in config and which belongs in runtime state
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
7. leave a clear path for future expansion

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

## What Configuration Should Contain

Configuration should contain information that is known before runtime and should be easy to author and review.

Examples:

* player max HP
* the starting player deck
* enemy max HP
* each enemy's fixed deck
* reward replacement generation rules
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

---

## PlayerStartConfig

### Purpose

`PlayerStartConfig` defines the player's starting conditions at the beginning of a run.

### Recommended Fields

* `int playerMaxHp`
* `List<CardSpec> startingDeck`

### Current Rules

* `playerMaxHp` is the player's maximum HP
* the player's starting current HP is initialized from `playerMaxHp`
* `startingDeck` must contain exactly 6 cards

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
* `List<CardSpec> fixedDeck`

### Current Rules

* `fixedDeck` must contain exactly 6 cards
* `maxHp` must be positive
* enemy cards are authored explicitly in config for the current demo
* the current demo does not use a procedural enemy deck generation system

### Notes

Enemy authored content remains simple for the current demo.
Each enemy directly defines its fixed six-card deck.

---

## RewardGenerationConfig

### Purpose

`RewardGenerationConfig` defines how replacement card generation works for reward offers.

It controls the legal generation space for replacement cards.

### Recommended Fields

* `List<RpsType> allowedReplacementRpsTypes`
* `List<int> allowedReplacementBasePowers`
* `List<TraitType> allowedReplacementTraits`
* `int replacementTraitCount`

### Current Demo Rules

* `replacementTraitCount` is fixed to 2
* the current demo uses exactly one allowed replacement base power value
* the architecture still supports multiple allowed base powers in the future

### Why This Object Exists

Replacement cards are not chosen from a fully hand-authored card catalog.
Instead, they are generated from legal authored rules.
This object defines those legal authored rules.

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
* nested container structures beyond what is necessary
* complex generic serialization requirements

---

## Serialization Choice

The current implementation should use Unity `JsonUtility`.

### Why This Is Acceptable

For the current demo, the config structure is intentionally kept compatible with `JsonUtility`:

* object root
* serializable classes
* lists
* enums
* simple scalar fields

### Why Third-Party JSON Is Deferred

A third-party serializer such as Newtonsoft may be useful later if the config system grows significantly.

However, the current design intentionally avoids the kinds of structures that would force that dependency immediately.

### Future-Proofing Rule

JSON loading should be isolated behind a thin loader so the serializer can be replaced later if needed without redesigning the whole system.

---

## Config Loading Pipeline

The intended loading pipeline is:

1. Unity provides the JSON file as a `TextAsset`
2. a thin Unity-facing loader reads the text
3. the JSON text is deserialized into `GameConfig`
4. config validation runs
5. a runtime factory converts config objects into runtime state

This pipeline keeps Unity-specific concerns at the edge.

---

## GameConfigLoader

### Purpose

`GameConfigLoader` is the thin adapter that loads authored config into pure C# config objects.

### Responsibilities

It should:

* read JSON text from a Unity-facing source such as `TextAsset`
* deserialize JSON into `GameConfig`
* validate the config
* return a valid config object

### It Should Not

It should not:

* create runtime battle state
* create run progression state directly
* contain gameplay logic
* contain reward logic
* contain round resolution logic

### Design Rule

The loader only converts text into validated config objects.

---

## RuntimeStateFactory

### Purpose

`RuntimeStateFactory` converts authored config objects into runtime state objects.

### Responsibilities

It should:

* create an initial `RunState` from `GameConfig`
* create `CardInstance` from `CardSpec`
* create `EnemyProgressState` from `EnemyConfig`

### It Should Not

It should not:

* read raw JSON
* read Unity assets directly
* handle battle progression
* handle run progression
* perform gameplay rule logic

### Design Rule

The factory only converts static config into initial runtime objects.

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

## Validation Rules

Config validation should be lightweight but explicit.

The goal is to catch content mistakes early.

### GameConfig validation

* `playerStart` must exist
* `enemies` must exist
* `enemies.Count` must be 3 for the current demo
* `rewardGeneration` must exist
* `traitTuning` must exist

### PlayerStartConfig validation

* `playerMaxHp > 0`
* `startingDeck` must exist
* `startingDeck.Count == 6`

### EnemyConfig validation

* `enemyId` must not be empty
* `maxHp > 0`
* `fixedDeck` must exist
* `fixedDeck.Count == 6`

### RewardGenerationConfig validation

* `allowedReplacementRpsTypes` must exist and contain at least one value
* `allowedReplacementBasePowers` must exist and contain at least one value
* `allowedReplacementTraits` must exist and contain at least one value
* `replacementTraitCount == 2` for the current demo

### TraitTuning validation

* all numerical fields must be non-negative

### CardSpec validation

* `basePower >= 0`
* `traits` must exist
* traits must not contain duplicates
* traits must not contain both `ShiftLeft` and `ShiftRight`
* authored card trait count must not exceed 3

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

These are not needed for the current demo phase.

---

## Extension Points

The current design leaves room for future expansion such as:

* splitting config into multiple JSON files
* replacing `JsonUtility` with another serializer
* supporting multiple replacement base powers
* supporting richer reward generation profiles
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