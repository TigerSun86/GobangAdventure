# Debug UI Plan

## Purpose

This document defines the plan for the first debug scene and debug-oriented UI for the Unity 6.4 2D turn-based card roguelike demo.

It explains:

* the role of the debug UI in the current project phase
* what the first debug scene must support
* how the scene should be constructed in Unity
* the planned screen layout
* the responsibilities of each major UI region
* the Presentation-layer object boundaries
* the controller and refresh model
* button visibility and interaction rules
* which parts are manually authored in the Editor
* which parts are runtime-generated
* the recommended Unity UI component mapping
* how the debug UI supports manual validation and later Play Mode smoke tests

This document is about debug-oriented presentation only.
It does not redefine gameplay rules, application flow rules, or domain model rules.

---

## Scope and Non-Goals

## In Scope

The first debug scene must support:

* loading authored config from the StreamingAssets-based game_config.json source-of-truth
* creating a new run
* displaying run summary state
* starting a battle
* selecting a player card when allowed
* displaying the current board state
* displaying the enemy sequence for the current battle
* displaying the latest round result
* displaying slot combat results
* displaying round logs
* displaying phase snapshots
* continuing after round-result presentation
* displaying reward offers
* choosing reward options
* displaying clear status / failure messages
* displaying the authoritative HP application timeline for the latest round
* displaying a fixed battle-completion reason when one exists
* displaying player-death and simultaneous-zero outcomes clearly
* retaining the latest fatal round result after the active battle is cleared
* disabling all in-run gameplay actions after `Victory` or `Defeat`

## Out of Scope

The first debug scene does not aim to provide:

* final player-facing production UI
* polished visuals
* card art
* animations
* drag-and-drop card play
* tooltip systems
* replay history browsers
* timeline scrubbing
* heavy MVVM or reactive binding frameworks
* gameplay rule logic inside UI components
* complex layout algorithms created in code

The current goal is a stable debug workbench, not a final game screen.

---

## Role of Debug UI in the Validation Workflow

The project uses three validation layers:

### 1. Edit Mode tests

These validate rule correctness and core logic.

### 2. Play Mode smoke tests

These validate minimal runtime wiring.

### 3. Manual debug validation

This validates:

* readability
* inspectability
* interaction clarity
* debugging usefulness

The debug scene exists primarily for the third layer.

It is not a replacement for Edit Mode tests.
It is also not intended to carry the full burden of runtime integration testing.

Instead, it provides a reliable manual entry point for inspecting:

* current run state
* current battle state
* current reward state
* latest round output
* logs and snapshots
* authoritative HP before and after merged damage
* raw healing and actual healing after clamp
* final HP
* fixed battle-completion reason
* player-death and simultaneous-zero status
* terminal action gating
* retained visibility of the latest fatal round

---

## Scene and UI Construction Approach

## Recommended Approach

The recommended first implementation uses a hybrid approach:

* manually author the main UI layout in the Unity Editor
* runtime-generate repeated child entries
* keep Unity-specific scene and UI setup at the presentation edge
* keep gameplay rules outside UI code

The first debug scene should use:

* one Unity scene
* one main `Canvas`
* standard Unity UI components
* TextMeshPro text elements where appropriate
* simple layout groups on container objects

## Why This Approach Is Chosen

This approach is preferred because it:

* is easy to inspect and adjust in the Editor
* avoids writing large scene-construction code
* keeps repeated content generation simple
* is suitable for debug-first iteration
* reduces layout fragility compared with hand-calculated runtime positioning

---

## High-Level Screen Layout

The debug scene should use a single-screen layout with the following main regions:

### Top

Run and battle summary bar

### Left column

Main interaction and battle area:

* enemy sequence display
* board panel
* player deck panel
* action bar

### Right column

Inspector panel

### Bottom full-width line

Status message line

This layout is chosen for clarity, not visual polish.

---

## ASCII Wireframe

The following wireframe is illustrative only.

It defines:

* relative layout intent
* panel relationships
* information grouping

It does not define:

* pixel-perfect sizing
* final art direction
* exact spacing or padding

```text
+--------------------------------------------------------------------------------------------------+
| Top Summary Bar                                                                                  |
| Player HP | Enemy HP | Enemy 1/N | Battles X/Limit | Rewards X/Limit | Run Stage | Battle Stage | Round |+------------------------------------------------------+-------------------------------------------+
| Left Main Column                                     | Right Inspector Panel                     |
|                                                      |                                           |
| Enemy Sequence (read-only)                           | Latest Round Result                       |
| R1: Rock (5)                                         | Slot Results                              |
| R2: Scissors (4)                                     | Logs                                      |
| R3: Paper (5)                                        | Snapshots                                 |
|                                                      | Reward Details                            |
| Enemy Board Row                                      |                                           |
| [Slot 1] [Slot 2] [Slot 3]                           |                                           |
|                                                      |                                           |
| Player Board Row                                     |                                           |
| [Slot 1] [Slot 2] [Slot 3]                           |                                           |
|                                                      |                                           |
| Player Deck Panel                                    |                                           |
| [Card 1] [Card 2] [Card 3]                           |                                           |
| [Card 4] [Card 5] [Card 6]                           |                                           |
|                                                      |                                           |
| Action Bar                                           |                                           |
| [Load Config] [New Run] [Start Battle] [Continue]    |                                           |
| Reward action area appears here when needed          |                                           |
+------------------------------------------------------+-------------------------------------------+
| Status Message                                                                                   |
| Config loaded successfully                                                                        |
+--------------------------------------------------------------------------------------------------+
```

### Example Deck Entry

```text
+-------------------------------+
| Rock                          |
| Traits: Empower, ShiftLeft    |
| Base 4 | Perm +1              |
| AVAILABLE                     |
+-------------------------------+
```

### Example Reward Option

```text
+--------------------------------------------------+
| Upgrade                                          |
| Target: Rock [Empower]                           |
| Add: Suppress                                    |
| Result: Rock [Empower, Suppress]                 |
+--------------------------------------------------+
```

### Example Board Slot

```text
+---------------------+
| Slot 2              |
| Paper               |
| Traits: AdjacentAid |
| Power: 6            |
| New this round      |
+---------------------+
```

---

## Layout Rationale

The layout should support the following reading flow:

1. see high-level run and battle state
2. inspect the current board
3. inspect the player's selectable cards
4. use the next action buttons
5. inspect detailed round output on the right
6. read command feedback at the bottom

The player deck panel should be below the board, not above it.
This better matches the common mental model of:

* enemy above
* board in the middle
* player and player controls below

The inspector should occupy a full right column, including the area beside the player deck and action bar.
This prevents wasted space and provides a stable home for detailed debug information.

The status message should be below the main content and action area, not between the deck and action bar.
This keeps interaction controls visually grouped together.

---

## Panel-by-Panel Design

## Top Summary Bar

The top summary bar should always show the most important current state.

### Recommended fields

* `Player HP: current / max`
* `Enemy: current index / total enemy count`
* `Enemy HP: current / max`
* `Battles Played: x / current enemy battle limit`
* `Rewards Claimed: x / current enemy reward total`
* `Run Stage`
* `Battle Stage`
* `Round`

### Recommended behavior

* if no active battle exists, battle stage may display `-`
* if no run exists, fields may display placeholders
* values should be concise and stable in position
* when the run is in `Defeat` or `Victory`, `Run Stage` must remain visible
* after the active battle is cleared, `Battle Stage` and `Round` may display `-`
* clearing the active battle must not clear the latest-round inspector
* final player and enemy HP remain visible through the run and enemy summary fields

### Important terminal-state rule

The top summary bar shows current authoritative run state.

It must not infer defeat from displayed HP.

The official terminal stage comes from:

* `RunState.FlowStage`

### Formatting responsibility

Display text should come from presentation formatting helpers, not from gameplay services.

---

## Board Panel

The board panel is the central visual area.

### Structure

It should show:

* enemy lane row on top
* player lane row below
* three slots per side

### Each slot should show

* slot index
* whether the slot is closed, open, or occupied
* occupant summary when occupied
* current power when occupied
* `New this round` marker when applicable

### Closed slot example

A closed slot must be clearly different from an empty open slot.

### Important Rule

The UI must preserve the distinction between:

* closed slot
* open but empty slot
* occupied slot

---

## Player Deck Panel

The player deck panel is the main card-selection area.

### Display rule

Each current player deck card should appear as one clickable entry.

### Recommended content per entry

* card type or short display name
* trait summary
* base power
* permanent bonus
* used / selectable state indicator

### Interaction rule

These entries are buttons because the player uses them to submit input.

### Layout recommendation

Use a `GridLayoutGroup` on the deck panel parent container.

Recommended initial setup:

* fixed column count: 3
* actual visible entry count is generated at runtime from the current deck size
* the familiar 2-row, 3-column layout is only the default-baseline visual example for a 6-card deck

This is preferred over a single row of six when debug card entries contain multiple lines of text.

### Runtime generation rule

The visible deck entries should be instantiated at runtime from one entry prefab or template.

The scene should not rely on six separately hand-authored card buttons.

---

## Enemy Sequence Display

The enemy sequence display is a read-only debug panel.

### Purpose

Show the current battle's prepared enemy sequence.

### Recommended content

For each round:

* round number
* enemy card type
* enemy card strength summary

### Interaction rule

Do not use buttons.
This is a read-only display.

### Layout recommendation

Use a simple `VerticalLayoutGroup`.

---

## Inspector Panel

The inspector panel is for detailed inspection, not primary interaction.

It should contain the following subsections.

### Latest Round Result

This section should display the latest finalized `RoundResult`.

It must remain available after the active battle has been cleared by run-level progression.

### Recommended general fields

* round index
* selected player card summary
* enemy card summary

### Recommended player HP timeline

* player HP before
* merged damage to player
* player HP after merged damage
* raw healing to player
* actual healing applied to player
* final player HP

### Recommended enemy HP timeline

* enemy HP before
* merged damage to enemy
* enemy HP after merged damage
* raw healing to enemy
* actual healing applied to enemy
* final enemy HP

### Example display

```
Player HP
Before: 2
Damage: 3
After Damage: -1
Raw Healing: 2
Healing Applied: 2
Final: 1

Enemy HP
Before: 4
Damage: 4
After Damage: 0
Raw Healing: 0
Healing Applied: 0
Final: 0
```

### Temporary-zero visibility

When:

```
HpAfterMergedDamage <= 0
FinalHp > 0
```

the formatter may display a presentation-only marker such as:

```
Recovered from temporary zero
```

This marker is derived from authoritative result fields.

It is not a gameplay-state field and must not be used to decide survival.

### Finalization rule

Only a finalized `RoundResult` returned through successful `BattleService` application may be displayed as the authoritative latest result.

The UI must not display a resolver-created non-finalized result as complete.
### Latest Battle Outcome

When the latest round completed a battle, the inspector should display:

* `BattleIndexForEnemy`
* `RoundsPlayed`
* `CompletionReason`
* `PlayerHpAfterBattle`
* `EnemyHpAfterBattle`

### Official outcome text

The displayed official result must come from:

* `BattleOutcome.CompletionReason`

Presentation must not independently determine the result from HP.

### Player defeated

For:

```
BattleCompletionReason.PlayerDefeated
```

display a clear result such as:

```
Official Result: Player Defeated
```

### Enemy defeated

For:

```
BattleCompletionReason.EnemyDefeated
```

display:

```
Official Result: Enemy Defeated
```

### All rounds completed

For:

```
BattleCompletionReason.AllRoundsCompleted
```

display:

```
Official Result: All Rounds Completed
```

This text describes the completed battle.

It does not by itself decide whether the full run later enters another battle or battle-limit defeat.

### Simultaneous zero

When:

```
CompletionReason ==
    BattleCompletionReason.PlayerDefeated
```

and:

```
PlayerHpAfterBattle <= 0
EnemyHpAfterBattle <= 0
```

the UI should state explicitly:

```
Official Result: Player Defeated
Both sides reached zero or below.
Player-death priority applies.
```

The enemy HP may still be displayed as zero or below.

The UI must not describe the enemy as officially defeated.

### Slot Results

This section should display slot-by-slot combat information from the latest round.

Recommended content per slot:

* slot index
* player card summary
* enemy card summary
* player power
* enemy power
* winner
* damage dealt

### Logs

This section should display round or battle logs as readable lines.

Use:

* one line per log entry
* stable ordering
* optional scroll support

### Snapshots

This section should allow inspection of per-phase snapshots.

Recommended first implementation:

* a phase selector dropdown
* one currently selected snapshot shown at a time

It is not necessary to display all phase snapshots at once.

### Reward Details

When a reward offer is active, this section should display readable reward option details.

---

## Reward Panel

The reward panel is visible only when a reward is pending.

### Display requirement

It must show exactly the current reward offer.

### Each reward option should show

For `Upgrade`:

* option type
* target card summary
* added trait
* resulting card summary when useful

For `Replace`:

* option type
* target card summary
* replacement card summary

For `Skip`:

* option type
* clear indication that no deck change occurs

### Interaction rule

Each reward option should be directly clickable as a one-step action.

### Layout recommendation

Use a `VerticalLayoutGroup` and stack options vertically.

This is recommended for readability because reward entries tend to contain more descriptive text than player deck entries.

### Visibility rule

When no reward is pending:

* hide the reward options area
  or
* show a small `No reward pending` placeholder

The preferred first implementation is to hide the option entries and optionally show a simple placeholder message.

---

## Action Bar

The action bar is the stable home for command buttons.

### Recommended buttons

* `Load Config`
* `New Run`
* `Start Battle`
* `Continue`

### Reward action area

When a reward is pending, reward actions may appear in the action area, in the reward panel, or in both a compact and detailed form, depending on implementation preference.

The key rule is:

* only show reward actions when a reward is pending
* keep their location stable enough for debugging use

### Optional debug controls

Optional later additions may include:

* show raw ids toggle
* clear logs
* expand/collapse inspector controls

These are not required for the first implementation.

### Stability rule

Action button positions should remain stable.
Do not let buttons move unpredictably between stages.

The action bar should remain close to the player deck panel.

---

## Status Message Line

The debug UI should include a fixed status message line.

### Purpose

Display:

* command success messages
* command failure messages
* `FailureReason`
* short next-step guidance
* completed battle handoff status
* run terminal status

### Command failure versus gameplay defeat

`FailureReason` represents an invalid or failed command.

Player defeat is a successful gameplay outcome and must not be presented as a command failure.

For example:

```
Incorrect:
Command failed: player died

Correct:
Run defeated: player final HP is zero or below.
```

### Recommended terminal status messages

#### Player death

```
Run defeated: player final HP is zero or below.
```

#### Simultaneous zero

```
Run defeated: both sides reached zero or below; player-death priority applies.
```

#### Battle-limit exhaustion

```
Run defeated: the enemy survived the configured battle limit.
```

#### Victory

```
Run victory: the final enemy was defeated while the player survived.
```

### Status source rule

The controller and formatter may combine already-authoritative facts from:

* `RunCommandResult`
* `RunState.FlowStage`
* cached `BattleOutcome`
* finalized `RoundResult`

They must not recalculate gameplay outcome or reward eligibility.

### Placement

The recommended placement remains:

* a full-width line below the main content and action area

Do not add a separate death dialog or death panel for the first implementation.

---

## Presentation Architecture and Object Boundaries

## Goals

The Presentation layer should:

* display state
* forward input
* format readable text
* keep interaction flow understandable

It should not:

* own gameplay rules
* reimplement domain calculations
* mutate runtime state directly outside service calls

---

## DebugSceneController

`DebugSceneController` should be the scene-level entry point.

### Responsibilities

* hold the current config reference
* hold the current run reference
* hold references to services and helpers
* respond to UI button clicks
* call service public methods
* receive command results
* update presentation-only local state
* trigger full refresh
* retain the latest finalized `RoundResult` for presentation
* retain the latest completed `BattleOutcome` for presentation
* preserve those references before run-level acceptance clears the active battle
* chain battle completion into run-level acceptance
* prevent duplicate Continue processing
* build status text from authoritative command and outcome data

### It should not

* calculate gameplay rules
* interpret trait effects
* compute reward legality
* directly mutate runtime state without going through the appropriate service or factory
* classify player death
* classify enemy defeat
* decide simultaneous-zero priority
* decide reward eligibility
* decide victory or defeat
* recreate a `BattleOutcome` from displayed HP
* clear the authoritative pending outcome inside `BattleState`

---

## Panel View Components

The scene should use small view components for each region.

Recommended examples:

* `RunSummaryPanelView`
* `BoardPanelView`
* `PlayerDeckPanelView`
* `InspectorPanelView`
* `RewardPanelView`
* `ActionBarView`

### Responsibilities

Each panel view should:

* receive view data
* update texts
* update button states
* show or hide subsections as instructed

Each panel view should not:

* call gameplay services directly
* own global UI flow
* implement gameplay rules

---

## View Data Objects

The first implementation should use lightweight view data objects.

Recommended examples:

* `RunSummaryViewData`
* `BoardViewData`
* `BoardSlotViewData`
* `DeckCardViewData`
* `RewardOptionViewData`
* `InspectorViewData`
* `ActionBarViewData`
* `RoundResultViewData`
* `BattleOutcomeViewData`
* `HpTimelineViewData`

### `HpTimelineViewData`

Recommended fields:

* `HpBefore`
* `Damage`
* `HpAfterMergedDamage`
* `RawHealing`
* `HealingApplied`
* `FinalHp`

### `BattleOutcomeViewData`

Recommended fields:

* battle index
* rounds played
* completion-reason text
* final player HP
* final enemy HP
* simultaneous-zero explanatory text when applicable

### Important rule

View data may derive display labels from authoritative fields.

It must not create or alter gameplay outcome.

### Purpose

These objects make UI rendering cleaner by separating:

* domain state
  from
* display-ready presentation data

### Important rule

This is a lightweight view-data layer, not a heavy MVVM framework.

---

## Formatter Helpers

Use lightweight formatter helpers to centralize display text generation.

Recommended examples:

* `CardTextFormatter`
* `RewardOptionTextFormatter`
* `RoundResultTextFormatter`
* `SnapshotTextFormatter`
* `FlowStageTextFormatter`

### Purpose

Prevent display text assembly from being scattered across many MonoBehaviours.

### Important rule

Formatters produce human-readable text only.
They must not contain gameplay rule logic.

---

## Presentation-Only Local State

The debug scene should keep a small UI-only local state object.

Recommended example:

* `DebugUiState`

### Suggested responsibilities

* currently selected inspector section
* selected snapshot phase
* whether raw ids are shown
* temporary status text
* local expansion/collapse flags
* latest finalized `RoundResult`
* latest completed `BattleOutcome`

### Latest-result retention

`DebugUiState.LatestRoundResult` should be updated after each successful `SubmitPlayerCard(...)`.

Only the finalized result returned after `BattleService` application may be stored.

`DebugUiState.LatestBattleOutcome` should be updated when a completed battle outcome is exposed through `FinishRoundPresentation(...)`.

These retained references are presentation-only observation state.

They are not authoritative gameplay state and must never be mutated by Presentation.

### Clear rules

Clear retained latest-result state when:

* a new run is successfully created
* a new battle is successfully started, if the intended inspector behavior is to begin that battle with a clean result area

Do not clear it merely because:

* `RunService.AcceptCompletedBattle(...)` clears `RunState.ActiveBattle`
* the run enters reward flow
* the run enters `Victory`
* the run enters `Defeat`

### Why retention is required

After `RunService.AcceptCompletedBattle(...)`:

* `RunState.ActiveBattle` becomes `null`

Without a presentation cache, the final round's:

* HP timeline
* slot results
* logs
* snapshots
* completion reason

would disappear immediately.

The debug workbench must retain that information for inspection.

---

## Refresh Model

## Chosen Strategy

Use a command-driven full-refresh model.

### Core sequence

1. user clicks a button
2. controller calls the relevant service or helper
3. service returns result and mutates runtime state as appropriate
4. controller updates `DebugUiState` if needed
5. controller calls `RefreshAll()`
6. all visible panels rebuild from current state

## Why This Strategy Is Chosen

It is:

* simple
* stable
* easy to reason about
* appropriate for a debug scene
* unlikely to miss refresh cases

The project scale is small enough that full refresh is acceptable.

## Explicitly Not Chosen

The first implementation should not use:

* event buses
* reactive subscriptions
* automatic UI diff patching
* partial smart refresh systems
* heavy data-binding frameworks

---

## RefreshAll Responsibilities

`RefreshAll()` should:

1. read current root state
2. read current presentation-only local state
3. build run-summary view data
4. build board and deck view data when an active battle exists
5. build latest-round view data from the retained finalized result
6. build latest-outcome view data from the retained completed outcome
7. render all panels
8. apply current button enable / disable rules
9. apply reward panel visibility rules
10. display current status message

### Data precedence

For latest-round display:

* use the latest finalized result from `DebugUiState`

The active battle may be used to update that cache, but the inspector must not require an active battle to remain visible.

### Terminal refresh

When the run enters `Victory` or `Defeat`, `RefreshAll()` must:

* display the terminal run stage
* disable in-run gameplay actions
* keep the latest round and outcome inspector visible
* keep status text visible

---

## Command Flow per User Action

## Load Config

### Input source

The debug scene should load authored config from the StreamingAssets game_config.json file, not from a runtime text input box.

### Flow

1. read the StreamingAssets game_config.json file
2. call GameConfigLoader
3. store the resulting config if successful
4. update status message
5. refresh UI

### Important note

Loading config does not automatically create a new run.

---

## New Run

### Flow

1. ensure config is available
2. call `RunService.CreateNewRun(...)`
3. allow runtime construction to be delegated to `RuntimeStateFactory`
4. store current run reference
5. clear run-dependent local UI state
6. refresh UI

---

## Start Battle

### Flow

1. call `RunService.StartNextBattle(...)`
2. capture `RunCommandResult`
3. update status message
4. clear any stale latest-result selection state if needed
5. refresh UI

---

## Select Player Card

### Flow

1. card button click sends the selected card id to the controller

2. controller temporarily prevents duplicate card submission

3. controller calls `BattleService.SubmitPlayerCard(...)`

4. capture `BattleCommandResult`

5. if successful:

   * store the finalized `RoundResult` in `DebugUiState.LatestRoundResult`
   * clear any stale `LatestBattleOutcome`
   * default inspector focus to the latest round result

6. update status message

7. refresh UI

### Important note

This resolves and applies the current round and enters result presentation.

Any completed battle outcome is already fixed in `BattleState.PendingBattleOutcome`, but the controller does not send it to `RunService` until Continue finishes the presentation gate.

---

## Continue

### Flow

1. verify that an active battle exists
2. verify that its flow stage is `PresentingRoundResult`
3. temporarily disable Continue or use a controller re-entry guard
4. call `BattleService.FinishRoundPresentation(...)`
5. capture `BattleCommandResult`

### If the battle continues

If:

* `IsBattleComplete == false`

then:

1. update status message
2. refresh UI
3. restore normal interaction according to the new battle stage

### If the battle completed

If:

* `IsBattleComplete == true`
* a `BattleOutcome` is returned

then:

1. retain the already-finalized latest `RoundResult`
2. store the returned authoritative `BattleOutcome` in `DebugUiState.LatestBattleOutcome`
3. call `RunService.AcceptCompletedBattle(...)`
4. capture `RunCommandResult`
5. update status message from the run command result and retained authoritative outcome
6. refresh UI

### Outcome-source rule

The controller must preserve the authoritative outcome exposed by battle flow.

If `AcceptCompletedBattle(...)` accepts a supplied `BattleOutcome`, the controller must pass that same authoritative outcome.

If `RunService` reads the outcome directly from the active battle, the controller must not create or supply a second conflicting outcome source.

The controller must never create a new outcome by checking displayed HP.

### Active-battle cleanup rule

`RunService.AcceptCompletedBattle(...)` may clear `RunState.ActiveBattle`.

The controller must retain the latest finalized result and completed outcome before that cleanup makes the battle state unavailable.

### Player-death flow

For a player-defeat outcome, the same Continue button is used:

```
PresentingRoundResult
→ Continue
→ BattleComplete
→ RunService.AcceptCompletedBattle(...)
→ RunFlowStage.Defeat
```

No separate death-confirmation button is required.

### Important note

Continue is a presentation gate.

It does not decide:

* player death
* enemy defeat
* simultaneous-zero priority
* reward eligibility
* victory
* defeat

Those outcomes have already been fixed or are decided by application services.

---

## Choose Reward

### Flow

1. controller temporarily disables reward buttons
2. controller calls `RunService.ChooseReward(...)`
3. capture `RunCommandResult`
4. update status message
5. refresh UI

### Important note

Reward selection is handled at run level, not battle level.

---

## Button Visibility and Enable/Disable Rules

### Global terminal rule

When:

* `RunState.FlowStage == Victory`
  or
* `RunState.FlowStage == Defeat`

disable all in-run gameplay actions:

* player deck buttons
* Continue
* Start Battle
* reward option buttons

Debug utility actions such as:

* Load Config
* New Run

may remain available.

---

## Player Deck Buttons

Enable only when:

* a run exists
* `RunState.FlowStage == InBattle`
* an active battle exists
* `BattleFlowStage == WaitingForPlayerCard`

Disable when:

* the battle is resolving
* the round result is being presented
* the battle is complete
* a reward is pending
* the run is in `Victory`
* the run is in `Defeat`

---

## Continue Button

Enable only when:

* `RunState.FlowStage == InBattle`
* an active battle exists
* `BattleFlowStage == PresentingRoundResult`
* the controller is not already processing Continue

Disable otherwise.

The button may remain visible for layout stability.

### Fatal-round behavior

Continue remains enabled while a fatal round result is being presented.

This allows the user to inspect the complete round before advancing to terminal run defeat.

After run-level acceptance enters `Defeat`:

* Continue becomes disabled
* the fatal result remains visible

---

## Start Battle Button

Enable only when:

* `RunState.FlowStage == ReadyForNextBattle`
* `RunState.ActiveBattle == null`
* `RunState.PendingRewardOffer == null`

Explicitly disable in:

* `InBattle`
* `ChoosingReward`
* `Victory`
* `Defeat`

---

## Reward Buttons

Show and enable only when:

* run flow stage is `ChoosingReward`
* a pending reward offer exists

Otherwise:

* hide reward option entries
  or
* show a simple placeholder such as `No reward pending`

After click:

* temporarily disable reward buttons to prevent double submit

Reward entries must not appear for:

* player defeat
* simultaneous zero resolved as player defeat
* battle-limit exhaustion
* `Victory`
* `Defeat`

The UI does not decide these eligibility rules.

It simply renders the authoritative pending reward state:

* show reward options only when `PendingRewardOffer != null`
* enable them only when `RunFlowStage == ChoosingReward`

---

## Load Config Button

It may remain enabled in the first implementation.

If clicked during an existing run:

* load and store the new config
* do not silently rebuild the run
* update status message to explain that a new run must be created to use the newly loaded config

---

## New Run Button

It may remain enabled in the first implementation.

A later refinement may add an explicit confirmation if replacing an active run becomes problematic, but this is not required for the first debug version.

---

## Terminal-State Presentation

### Defeat

When `RunState.FlowStage == Defeat`, the UI should show:

* run stage `Defeat`
* final player HP
* final enemy HP
* the latest finalized round
* the latest official battle outcome when the defeat came from player death
* a clear status message explaining the defeat source
* no enabled in-run gameplay action

### Two defeat sources

The status display should distinguish:

#### Player-death defeat

Derived from:

retained `BattleOutcome.CompletionReason ==
BattleCompletionReason.PlayerDefeated`

#### Battle-limit defeat

Derived from:

* run-level command result and final run state after an `AllRoundsCompleted` battle exhausted the configured limit

The UI must not describe battle-limit defeat as player death.

### Victory

When `RunState.FlowStage == Victory`, the UI should show:

* run stage `Victory`
* final HP values
* the latest finalized round
* the latest official enemy-defeat outcome
* no enabled in-run gameplay action

### No additional terminal UI required

The first implementation does not require:

* a modal dialog
* a scene transition
* a death animation
* a separate victory or defeat panel

The existing summary bar, inspector, action bar, and status line are sufficient.

---

## Fixed Skeleton vs Runtime-Generated Repeated Entries

The first debug UI should follow this boundary:

### Fixed skeleton authored in the scene

The following should be fixed scene structure:

* root canvas and root layout containers
* summary bar
* board panel root
* all six board slot views
* player deck panel root
* reward panel root
* inspector panel root
* action bar root
* status message line
* snapshot inspector root
* log list root
* enemy sequence root

These are structural UI regions and should be easy to rearrange manually in the Editor.

### Runtime-generated repeated entries

The following should be instantiated and refreshed at runtime:

* player deck entries
* reward option entries
* enemy sequence rows
* log rows
* slot result rows
* optional snapshot phase buttons if that style is chosen

### Why this boundary exists

This keeps the UI easy to tune in the Editor while still avoiding hardcoding repeated content as many manually duplicated child objects.

---

## Recommended Unity UI Component Mapping

The first implementation should use a clear and explicit mapping between logical UI elements and Unity UI components.

### Mapping Table

| UI Element              | Recommended Unity Type                                | Runtime Generated | Clickable |
| ----------------------- | ----------------------------------------------------- | ----------------: | --------: |
| Player deck entry       | `Button` root + multiple `TMP_Text` children          |               Yes |       Yes |
| Reward option entry     | `Button` root + multiple `TMP_Text` children          |               Yes |       Yes |
| Enemy sequence row      | Read-only row object + `TMP_Text`                     |               Yes |        No |
| Log row                 | Read-only row object + `TMP_Text`                     |               Yes |        No |
| Slot result row         | Read-only row object + `TMP_Text` or simple panel row |               Yes |        No |
| Board slot view         | Fixed panel with multiple `TMP_Text` children         |                No |        No |
| Status message line     | Fixed `TMP_Text`                                      |                No |        No |
| Snapshot phase selector | `TMP_Dropdown` preferred                              |                No |       Yes |
| Summary bar fields      | Fixed `TMP_Text` elements                             |                No |        No |
| Action bar buttons      | Fixed `Button` objects                                |                No |       Yes |

### Important rule

Do not assume that a repeated entry should always be represented by a single multi-line text object.

For several key elements, especially player deck entries and reward option entries, the preferred design is:

* one container or button root
* multiple dedicated `TMP_Text` children

This improves readability and makes binding logic clearer.

---

## Recommended Prefab Structures

## PlayerDeckEntry Prefab

This should be a clickable prefab.

### Recommended structure

* root object with:

  * `RectTransform`
  * `Image`
  * `Button`
* child `TMP_Text` objects:

  * `TitleText`
  * `TraitsText`
  * `StatsText`

### Example hierarchy

```text
PlayerDeckEntry
├── TitleText
├── TraitsText
└── StatsText
```

### Why this structure is chosen

A deck entry is the player's primary input target during battle.
Using a `Button` root makes the whole entry easy to click and easy to disable when interaction is not allowed.

The recommended structure uses multiple text children rather than a single multi-line text field so that:

* formatting remains simpler
* content can be updated more cleanly
* future visual adjustments remain easier

---

## RewardOptionEntry Prefab

This should also be a clickable prefab.

### Recommended structure

* root object with:

  * `RectTransform`
  * `Image`
  * `Button`
* child `TMP_Text` objects:

  * `TypeText`
  * `TitleText`
  * `BodyText`

### Example hierarchy

```text
RewardOptionEntry
├── TypeText
├── TitleText
└── BodyText
```

### Why this structure is chosen

Reward selection is a one-step action.
The whole option should be clickable.

Using multiple text children makes it easier to show:

* option category
* short summary
* readable detail text

---

## EnemySequenceRow Prefab

This should be a read-only row.

### Recommended structure

* root object with:

  * `RectTransform`
  * optional `Image`
* child:

  * `TMP_Text`

### Example hierarchy

```text
EnemySequenceRow
└── Text
```

### Important rule

Do not make enemy sequence rows look like clickable buttons.
They are informational only.

---

## LogRow Prefab

This should be a read-only text row.

### Recommended structure

* root object with:

  * `RectTransform`
* child:

  * `TMP_Text`

### Example hierarchy

```text
LogRow
└── Text
```

An optional background image may be added later, but it is not required.

---

## SlotResultRow Prefab

This should be a read-only result row.

### Recommended first implementation

Use a simple row with:

* root object
* one `TMP_Text`

### Optional richer implementation

A later refinement may split this into multiple texts such as:

* slot index
* matchup summary
* winner
* damage

The first debug version does not require that extra structure.

---

## BoardSlotView Structure

Board slots should not be runtime-generated in the first implementation.
There are always exactly six slots:

* three enemy slots
* three player slots

These should be fixed scene-authored views.

### Recommended structure

Each slot view may contain:

* `SlotIndexText`
* `OccupantText`
* `TraitsText`
* `PowerText`
* `MarkerText`

### Example hierarchy

```text
BoardSlotView
├── SlotIndexText
├── OccupantText
├── TraitsText
├── PowerText
└── MarkerText
```

### Why fixed slot views are preferred

The board structure is fixed and small.
Hand-authoring the six slot views makes the layout simpler and more stable than generating them at runtime.

---

## Status Message Line Structure

The status line should be a simple fixed text display.

### Recommended structure

* root object with:

  * `RectTransform`
* child or root:

  * `TMP_Text`

This does not need to be a button or a repeated prefab.

---

## Snapshot Phase Selector

The recommended first implementation uses:

* `TMP_Dropdown`

This is preferred over a custom button strip because:

* the number of phases is fixed and small
* dropdowns are compact
* they reduce layout clutter in the inspector area

A button strip is allowed later if explicitly preferred, but it is not required for the first debug version.

---

## Data Formatting Rules

## Card Text

Card display text should prioritize:

* type
* trait summary
* base power
* permanent bonus
* used / selectable status markers

Keep the text compact.

---

## Slot Text

Each board slot should display:

* slot index
* open / closed / occupied state
* occupant summary if present
* current power if present
* `New this round` marker when appropriate

---

## Reward Text

Reward options should be readable as one-step actions.

The UI should make it obvious:

* what card is affected
* what change occurs
* what the resulting card or replacement looks like

---

## Snapshot Text

The selected snapshot should show:

* which phase is being viewed
* readable enemy lane state
* readable player lane state

The first implementation may use text-based snapshot display.

---

## Round Result Text

Round-result formatting should clearly distinguish:

* merged damage
* HP after merged damage
* raw healing
* actual healing applied
* final HP

Do not compress these into only:

    HP before / after

because that hides temporary-zero recovery and max-HP clamp.

---

## Outcome Text

Outcome formatting must use:

* `BattleCompletionReason`

It may explain the authoritative result but must not infer a different one.

### Simultaneous-zero example

    Player Defeated
    Player Final HP: 0
    Enemy Final HP: 0
    Both sides reached zero or below.
    Player-death priority applies.

Do not show:

    Enemy Defeated

for that outcome.

---

## Status Text

Status messages should be:

* short
* explicit
* command-result oriented
* clear about terminal gameplay outcomes

Use them for:

* success feedback
* failure feedback
* simple next-step guidance
* terminal gameplay outcomes

Do not use the status line as a full log substitute.

Do not treat player defeat as a failed command.

---

## Debug UI Support for Manual Validation

The first debug scene should support the following manual validation scenarios.

### Scenario A: Config Bootstrap

* load config
* create a new run
* inspect top-summary values
* confirm player and enemy initial values

### Scenario B: Battle Happy Path

* start a battle
* select a valid player card
* inspect the latest round result
* click Continue
* observe transition to the next round or completed battle

### Scenario C: Round Inspection

* inspect board
* inspect slot results
* inspect logs
* inspect all seven snapshots
* inspect the HP application timeline

### Scenario D: Reward Happy Path

* enter reward stage
* inspect all reward options
* choose one reward
* confirm deck and run summary update

### Scenario E: Invalid Input Sanity

* attempt actions at the wrong stage
* confirm no crash
* confirm useful command-failure text
* confirm state remains stable

### Scenario F: Temporary Zero Recovered by Regrow

Verify:

* player HP after merged damage is zero or below
* raw Regrow healing is visible
* actual healing applied is visible
* final player HP is above zero
* no player-defeat outcome is displayed
* battle flow continues normally

### Scenario G: Temporary Zero Recovered by Lifesteal

Verify:

* attributed player-card damage is visible
* player HP after merged damage is zero or below
* raw Lifesteal healing is visible
* final player HP is above zero
* the run does not enter `Defeat`

### Scenario H: Player Death

Verify:

* all seven phase snapshots exist
* final player HP is zero or below
* the latest result is presented before run-level defeat
* Continue is enabled during `PresentingRoundResult`
* after Continue, run stage becomes `Defeat`
* active battle is cleared
* no reward is displayed
* no in-run gameplay action is enabled
* the fatal round remains visible

### Scenario I: Simultaneous Zero

Verify:

* both final HP values are zero or below
* official result is `PlayerDefeated`
* the UI explains player-death priority
* the UI does not report official enemy defeat
* no reward appears
* final-enemy simultaneous zero does not show `Victory`

### Scenario J: Battle-Limit Defeat

Verify:

* both sides survive the final round
* the enemy remains alive after the configured battle limit
* run stage becomes `Defeat`
* no reward is generated for the exhausting battle
* status text distinguishes this from player-death defeat

---

## Relationship to Play Mode Smoke Tests

The debug scene should be designed so that later Play Mode smoke tests are easy to write.

## Stable UI References

The scene should expose stable references for:

* load config button
* new run button
* start battle button
* continue button
* status message text
* latest-round-result root
* latest-outcome text or root
* run-stage text
* battle-stage text
* player-HP text
* enemy-HP text
* key panel roots where useful

## Stable Visible State

The scene should display stable text for:

* run stage
* battle stage
* player HP
* enemy HP

This makes smoke assertions simpler.

## Reward Panel Behavior

Reward panel visibility should be predictable.

Do not allow ambiguous partial reward state.

## Latest Round Result Presence

A stable latest-result container should exist so smoke tests can assert that result presentation is visible.

The stable latest-result container must remain populated after fatal active-battle cleanup.

## Terminal Action State

Smoke tests should be able to determine that:

* player deck buttons are disabled
* Continue is disabled
* Start Battle is disabled
* reward buttons are absent or disabled

after `Defeat`.

---

## Planned First Smoke Tests

The intended Play Mode smoke tests are:

* bootstrap smoke test
* basic nonfatal battle-flow smoke test
* one fatal battle-flow smoke test
* optional later reward-flow smoke test

### Fatal battle-flow smoke

The fatal smoke path should verify:

1. a fatal round can be submitted
2. its finalized result becomes visible
3. Continue completes the presentation gate
4. the controller forwards the completed outcome to `RunService`
5. the run enters `Defeat`
6. the active battle is cleared
7. no gameplay action remains enabled
8. the latest fatal result remains visible

### Scope rule

Smoke tests validate scene and controller wiring.

They do not replace Edit Mode coverage for:

* exact HP arithmetic
* Regrow recovery
* Lifesteal recovery
* simultaneous-zero classification
* reward counters
* outcome invariants

---

## What Play Mode Smoke Tests Should Not Cover

Do not rely on Play Mode smoke tests for:

* detailed `RoundResolver` behavior
* trait edge-case matrices
* reward canonical dedup correctness
* config validation matrices
* fine-grained domain-rule correctness

Those belong in Edit Mode tests.

---

## Expected Unity Editor Impact

The current player-death presentation does not require a new major UI region.

The existing design can reuse:

* top summary bar
* latest-round inspector
* status message line
* Continue button

### No manual Editor change is expected when:

* the latest-round inspector already uses a flexible multi-line text area
* the status line can display the recommended terminal messages
* the controller already has serialized references to those existing UI elements

### A small manual Editor adjustment may be required when:

* the latest-round area uses separate fixed fields and lacks fields for the HP timeline
* the inspector needs more vertical space or scrolling
* a new outcome text field is preferred instead of appending outcome text to the existing latest-result field
* new serialized text references must be connected

### Preferred approach

Prefer reusing the current inspector and status elements before adding:

* a new death panel
* a new button
* a new scene
* a modal dialog

---

## Done Criteria for Debug UI v1

The first debug UI implementation is considered done when it:

* can load config from the StreamingAssets game_config.json file
* can create a new run
* can start a battle
* can show current run and battle summary
* can display the board clearly
* can display the current enemy sequence
* can display the player deck as clickable entries
* can select a player card when allowed
* can display the latest round result
* can display slot results
* can display logs
* can display snapshots
* can continue after round-result presentation
* can display reward offers
* can choose one reward option
* can always show useful status feedback
* contains no gameplay rule logic
* supports manual validation scenarios
* supports later minimal Play Mode smoke tests
* can display HP before damage
* can display HP after merged damage
* can distinguish raw healing from actual healing
* can display final HP
* can display the official battle-completion reason
* can display player-death status
* can explain simultaneous-zero priority
* retains the latest fatal round after active-battle cleanup
* disables all in-run gameplay actions after `Victory` or `Defeat`
* does not generate reward UI after player death or battle-limit exhaustion
* supports temporary-zero, player-death, simultaneous-zero, and battle-limit manual validation
* supports one fatal Play Mode smoke path

---

## Deferred UI Ideas

The following are explicitly deferred:

* animations
* drag-and-drop interaction
* replay history browsing
* card art
* tooltip system
* reward diff highlighting
* advanced filtering or search
* automatic layout intelligence beyond simple layout groups
* polished player-facing visual design

These may be explored later, but are not required for the current debug-first phase.

---

## Summary

The first debug UI is a single-scene debug workbench.

It is built from:

* manually authored major layout containers
* runtime-generated repeated entries
* a controller-driven full-refresh model
* small panel views
* lightweight view data
* lightweight text formatters
* a small presentation-only local state object

For round and battle completion, it must:

* display the finalized authoritative HP timeline
* display the fixed `BattleCompletionReason`
* preserve the latest finalized round and outcome through active-battle cleanup
* use Continue only as a presentation gate
* chain completed battle handoff into `RunService`
* distinguish player death, simultaneous zero, battle-limit defeat, and victory
* disable all in-run gameplay actions in terminal states
* avoid implementing gameplay outcome or reward rules in Presentation

Its job is to make the run, battle, reward, HP-application, and terminal-flow behavior easy to inspect and validate during early development.