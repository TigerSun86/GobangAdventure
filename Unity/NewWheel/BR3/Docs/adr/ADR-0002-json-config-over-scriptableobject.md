# ADR-0002: JSON Config Over ScriptableObject for Primary Authored Content

## Status

Accepted

---

## Context

The project needs an authored content and configuration system for:

* player starting state
* enemy authored content
* reward replacement generation rules
* trait numerical tuning values

The current demo has these constraints and goals:

* domain and runtime logic should remain as Unity-agnostic as practical
* authored content should be easy to read in Git diffs and PR review
* authored content should be easy to inspect and edit in VS Code
* Codex-based implementation should work well with repository-native text files
* the first implementation should stay lightweight
* the project should avoid overengineering the content pipeline too early

A decision is needed on whether the primary authored content format should be:

* ScriptableObject assets
  or
* JSON-based text configuration

---

## Decision

The project will use JSON as the primary authored content format for the current demo.

The authored config pipeline will be:

1. JSON files as authored content
2. pure C# config objects deserialized from JSON
3. a thin Unity-facing config loader
4. explicit conversion from config objects into runtime state

ScriptableObject will not be used as the primary authored content source in the current demo phase.

---

## Detailed Decision

## Primary authored format

Use JSON text files.

## Config object model

Use pure C# config objects such as:

* `GameConfig`
* `PlayerStartConfig`
* `EnemyConfig`
* `RewardGenerationConfig`
* `TraitTuning`
* `CardSpec`

## Unity integration boundary

Use a thin Unity-specific adapter such as:

* `TextAsset` input
* `GameConfigLoader`

## Runtime construction boundary

Use explicit conversion through:

* `RuntimeStateFactory`

---

## Why JSON Was Chosen

## Reason 1: Better text review workflow

The project uses Git and code review as a major development and validation workflow.

JSON is easier to:

* read in diffs
* review in pull requests
* inspect quickly in VS Code
* compare across revisions

This is especially useful in the current phase where design and implementation are evolving quickly.

---

## Reason 2: Better alignment with Codex workflow

The project explicitly plans to use Codex in VS Code for implementation.

A text-native authored content format is better aligned with this workflow because:

* Codex can read and modify JSON naturally
* JSON can be included in repository-based implementation tasks
* no Unity Editor asset authoring is required for core content iteration
* no heavy reliance on Unity-specific content authoring workflows is required early

---

## Reason 3: Cleaner separation from Unity-specific asset types

The project wants domain logic and runtime state to remain as Unity-agnostic as practical.

Using JSON as the primary authored format helps preserve a clean separation:

* authored config is plain text
* config objects are pure C#
* runtime state is pure C#
* Unity only appears at the loading edge

This supports the layered architecture established in ADR-0001.

---

## Reason 4: Lower early workflow friction

The current demo phase values:

* fast iteration
* minimal content pipeline complexity
* fewer editor-only steps
* lower setup overhead

A JSON-first pipeline keeps the first implementation simpler than a more Unity-asset-centric configuration workflow.

---

## Why ScriptableObject Was Not Chosen as Primary Authored Content

This decision does not claim that ScriptableObject is bad in general.

It only means that ScriptableObject is not the best primary authored content format for this specific project at this specific stage.

### Concerns with a ScriptableObject-first approach in the current demo phase

* authored content becomes more tied to Unity-specific asset workflows
* repository review is less convenient than plain JSON review
* Codex-centered editing is less natural
* it is easier to slide toward Unity asset structure dictating domain structure
* the project does not yet need the stronger Unity editor authoring advantages badly enough to justify the tradeoff

---

## Why This Is Not a Rejection of ScriptableObject Forever

The current decision is scoped to the current demo phase.

ScriptableObject may still become useful later for:

* editor tooling
* richer designer-facing content workflows
* larger content libraries
* content assets with stronger Unity-specific references
* more mature authoring pipelines

The decision is:

* not now as the primary authored source
* maybe later if project needs change

---

## Serialization Decision

The current implementation should use Unity `JsonUtility` first.

### Why

The current config model is intentionally designed to fit within a simple structured JSON shape:

* one object root
* serializable classes
* lists
* enums
* simple scalar fields

### Why not use a third-party serializer immediately

The project does not currently require:

* dictionaries
* polymorphic config graphs
* complex nested container structures
* advanced custom serialization behavior

So introducing a third-party serializer now would add unnecessary weight.

### Future-proofing rule

JSON deserialization should remain isolated behind the loader so a future switch to another serializer is possible if the config system grows beyond current needs.

---

## What This Decision Implies for Config Design

Because JSON is the primary authored format, config schemas should remain:

* explicit
* structured
* simple
* text-friendly
* serializer-friendly

This means the current config system should avoid:

* dictionary-heavy schemas
* polymorphic authored graphs
* deep inheritance in config data
* hidden authored content embedded inside services

This is one reason the current trait design only moves numerical tuning into config rather than implementing a full trait-definition config system.

---

## Relationship to Runtime State

This ADR also reinforces a second important rule:

Authored config is not runtime state.

Examples:

* `PlayerStartConfig.playerMaxHp` is authored config
* `RunState.PlayerHp` and `RunState.PlayerMaxHp` are runtime state

Examples:

* `EnemyConfig.maxHp` is authored config
* `EnemyProgressState.CurrentHp` and `EnemyProgressState.MaxHp` are runtime state

Runtime systems should not repeatedly consult authored config for active gameplay state.
Authored config should be converted into runtime state at initialization boundaries.

---

## Alternatives Considered

## Alternative 1: ScriptableObject as primary authored content

Rejected for the current demo phase.

### Pros

* good Unity Inspector workflow
* familiar Unity authoring style
* easy future editor tooling integration

### Cons

* more Unity-centric workflow
* less convenient text review
* less natural Codex editing workflow
* more manual asset-oriented authoring overhead early
* greater risk of config shape being dictated by Unity asset structure rather than domain needs

---

## Alternative 2: Hardcoded content in C# only

Rejected.

### Pros

* simplest possible startup path
* no serialization system required

### Cons

* poor balancing workflow
* code recompilation required for content changes
* content harder to inspect as data
* less appropriate for a data-driven iteration loop

---

## Alternative 3: Full third-party JSON pipeline immediately

Rejected for now.

### Pros

* stronger serialization feature set
* more future flexibility

### Cons

* more dependency weight immediately
* not needed for the current config structure
* higher early complexity for limited practical benefit

---

## Consequences

## Positive consequences

* text-readable authored content
* better Git review ergonomics
* better Codex workflow ergonomics
* cleaner separation between Unity and domain/runtime logic
* simpler current implementation pipeline
* easier balancing iteration for selected numerical values

## Negative consequences

* less direct inspector-based authoring
* custom loader is required
* config validation must be done explicitly
* future move to stronger Unity editor workflows may require some additional adaptation
* future config complexity might eventually require serializer replacement

These tradeoffs are acceptable for the current demo phase.

---

## Follow-Up Design Rules

This ADR implies the following implementation rules:

1. JSON files are the source of authored gameplay config
2. config objects remain pure C# objects
3. a loader handles deserialization and validation
4. a factory handles conversion into runtime state
5. runtime systems do not treat config as live gameplay state
6. trait behavior remains code-driven; only selected numerical tuning values move into config
7. Unity-specific integration stays at the edge of the loading pipeline

---

## Revisit Conditions

This decision should be revisited if one or more of the following becomes true:

* the content graph becomes significantly more complex
* designers need a stronger Unity-native authoring workflow
* config begins to require dictionaries, polymorphism, or richer serialization features
* the project develops a large reusable authored content library
* editor tooling becomes a higher priority than repository-native text editing

Until then, JSON remains the accepted primary authored content format.

---

## Summary

For the current demo, primary authored content will use:

* JSON files
* pure C# config objects
* a thin Unity loader
* explicit runtime-state construction

This decision supports:

* repository-friendly workflows
* Codex-friendly workflows
* low early complexity
* clean separation between authored content and runtime state

If the project grows, the content pipeline may be revisited later, but JSON is the accepted choice for now.