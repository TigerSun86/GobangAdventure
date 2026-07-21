# Round Resolution

## Purpose

This document defines the design of `RoundResolver`, the component responsible for resolving exactly one round of gameplay.

It explains:

* the responsibility and boundaries of `RoundResolver`
* the expected inputs and outputs
* the fixed round pipeline
* what each phase reads and writes
* which values persist and which are recalculated each round
* the role of logs and phase snapshots
* how round-level consequences relate to later battle-layer HP application

This document is one of the most important engineering references for the current demo because `RoundResolver` is the core rule engine.

---

## Role of RoundResolver

`RoundResolver` resolves the rule-evaluation portion of exactly one round.

It executes the complete fixed seven-phase pipeline and produces the raw round consequences that will later be applied to authoritative runtime HP by `BattleService`.

### RoundResolver is responsible for:

* entering the newly played cards into the board
* recalculating round-local combat values
* applying movement
* applying board-derived effects
* resolving slot combat
* calculating merged damage totals
* calculating raw post-resolve healing totals
* applying non-HP post-resolve effects such as `Growth`
* producing slot combat results
* producing logs and phase snapshots
* creating the non-finalized `RoundResult` containing raw round consequences

### RoundResolver is not responsible for:

* deciding whether a battle should start
* waiting for player input
* selecting the enemy card sequence
* authoritatively mutating player or enemy HP
* applying max-HP clamp
* establishing final authoritative HP
* classifying player death or enemy defeat
* classifying battle completion
* advancing battle-level flow state
* advancing run-level flow state
* generating rewards
* deciding victory or defeat
* performing UI transitions

Those responsibilities belong to battle-level, run-level, or presentation orchestration.

### Important terminology

A round is **rule-resolved** when `RoundResolver` has completed all seven phases.

A round is **fully resolved and applied** only after `BattleService` has:

* applied merged damage
* recorded HP after merged damage
* applied post-resolve healing
* enforced max-HP clamp
* recorded actual healing
* established final HP
* finalized the authoritative application fields in `RoundResult`

---

## Why the Name Is RoundResolver

The game uses simultaneous play and round-based resolution.

The gameplay concept here is a round, not an alternating turn in the traditional sense.

For that reason:

* `RoundResolver` is preferred over `TurnResolver`
* `Resolver` is preferred over `Service`

This reflects the intended responsibility:

* `RunService`, `BattleService`, and `RewardService` orchestrate flow
* `RoundResolver` resolves rules

---

## Inputs and Outputs

### Conceptual Inputs

`RoundResolver` needs access to:

* the current `BattleState`
* the newly selected player card for this round
* the enemy card for this round
* current player HP as read-only round context
* current enemy HP as read-only round context

The exact method signature may evolve slightly during implementation, but the conceptual inputs are stable.

Current HP values supplied to `RoundResolver` are not permission to mutate authoritative runtime HP or perform survival classification.

### Conceptual Output

`RoundResolver` creates and returns a non-finalized `RoundResult`.

The resolver-populated portion contains:

* round index
* selected player and enemy card references
* raw merged damage totals
* raw post-resolve healing totals
* per-slot combat results
* logs
* per-phase snapshots

`BattleService` later finalizes the authoritative application fields:

* `PlayerHpBefore`
* `PlayerHpAfterMergedDamage`
* `PlayerHealingApplied`
* `PlayerHpAfter`
* `EnemyHpBefore`
* `EnemyHpAfterMergedDamage`
* `EnemyHealingApplied`
* `EnemyHpAfter`

### Important Design Choice

`RoundResolver` returns explicit round consequences rather than owning higher-level application or flow progression.

Battle and run progression remain the responsibility of higher-level services.

### RoundResult finalization rule

A `RoundResult` created by `RoundResolver` must not be added to finalized battle history or presented as the complete authoritative result until `BattleService` has populated all HP application fields.

The lifecycle is:

```
RoundResolver creates raw RoundResult
→ BattleService applies consequences
→ BattleService finalizes RoundResult
→ finalized RoundResult enters battle history
→ Presentation displays the finalized result
```

### No duplicate application logic

`RoundResolver` must not calculate an independent authoritative final HP result.

`BattleService` must not independently recalculate raw trait damage or healing rules already resolved by `RoundResolver`.

---

## Fixed Round Pipeline

The round pipeline is locked and must not be reordered or terminated early.

The seven phases are:

1. Enter
2. Fixed Self
3. Movement
4. Board Derived
5. Resolve Open Slots
6. Apply Merged Damage
7. Post Resolve

The order is part of the gameplay design and is not an implementation detail.

### Full-pipeline rule

Once a valid round begins resolution, all seven phases must complete.

Projected or intermediate player or enemy HP reaching zero or below must not cause:

* Phase 7 to be skipped
* `Regrow` to be skipped
* `Lifesteal` to be skipped
* `Growth` to be skipped
* premature player-death classification
* premature enemy-defeat classification
* premature battle completion

### No eighth phase

Player death, enemy defeat, battle completion, and battle continuation are classified after battle-layer HP application.

That classification is not a new eighth `RoundPhase`.

---

## Resolution Philosophy

The round system follows these core ideas:

### 1. Board position persists across rounds

Cards remain on the board during the battle unless future gameplay rules say otherwise.

### 2. Combat values do not persist across rounds

Combat power is recalculated every round from persistent data plus current board state.

### 3. Damage is accumulated before being applied

Slot combat does not immediately reduce HP. Damage is first accumulated and then treated as a simultaneous round consequence.

### 4. Post-resolve effects only apply to cards newly played this round

Cards already on the board do not retrigger these effects in later rounds.

### 5. Debug visibility is part of the design

The resolver should produce enough information to inspect each phase clearly.

### 6. Runtime HP state remains authoritative at battle layer

`RoundResolver` produces explicit consequence data, but the authoritative live HP state is updated later by `BattleService`.

### 7. Temporary zero does not end rule resolution

Merged damage may imply that player or enemy HP will temporarily reach zero or below.

That intermediate condition does not stop the seven-phase pipeline.

Post-resolve effects still execute normally.

### 8. Only final applied HP determines survival

Player death and enemy defeat are determined only after:

* merged damage has been applied
* post-resolve healing has been applied
* max-HP clamp has been enforced
* final authoritative HP has been established

### 9. Presentation does not participate in classification

Round-result presentation may display the completed result, but it does not calculate or alter player death, enemy defeat, or battle completion.

---

## Round-Local Context

Inside `RoundResolver`, it is useful to maintain a temporary round-local context object.

This is not authoritative runtime state. It is only a temporary working structure used during one round resolution.

It should conceptually track:

* round index
* HP before resolution
* references to the newly entered player and enemy board cards
* accumulated damage totals
* accumulated healing totals
* slot combat results
* logs
* snapshots
* temporary power deltas for board-derived effects

This object does not need to become a long-lived public domain object unless implementation later benefits from it.

---

## Phase 1: Enter

### Purpose

This phase places the newly selected player card and the enemy card for the current round onto the board.

### Reads

* `BattleState.RoundIndex`
* player selected `CardInstance`
* enemy selected card
* `BattleState.PlayerLane`
* `BattleState.EnemyLane`
* `BattleState.UsedPlayerCardIds`

### Writes

* current slot `IsOpen = true` on both sides
* current slot `Occupant`
* `UsedPlayerCardIds`
* references to the new board cards in round-local context
* logs
* snapshot

### Behavior

The round index determines which slot is opened this round.

The player and enemy cards enter the matching slot index for this round.

The player card is marked as used for this battle.

### Important Notes

* This phase does not calculate combat power.
* This phase does not move cards.
* This phase does not resolve combat.

---

## Phase 2: Fixed Self

### Purpose

This phase recalculates fixed self-based power for all cards currently on the board.

### Reads

* every occupied `BoardCard`
* `BoardCard.SourceCard.BasePower`
* `BoardCard.SourceCard.PermanentPowerBonus`
* `BoardCard.SourceCard.Traits`

### Writes

* `BoardCard.FixedSelfPower`
* `BoardCard.CurrentPower`
* `BoardCard.DamageDealtThisRound = 0`
* logs
* snapshot

### Behavior

All occupied board cards are recalculated from persistent source data.

The current formula is:

base power  
+ permanent power bonus  
+ fixed-self trait effects

At the current design stage, the main fixed-self effect is:

* `Empower`: +3

### Important Rule

This phase recalculates values for all on-board cards every round.

This means:

* board presence persists
* board position persists
* current combat power does not persist

### Important Reason

Without full recalculation here, power values from previous rounds would leak incorrectly into later rounds.

---

## Phase 3: Movement

### Purpose

This phase applies board movement effects.

### Reads

* current board occupancy on each side
* card traits on occupied slots

### Writes

* slot occupancy order on each lane
* logs
* snapshot

### Behavior

Movement is processed separately for the player lane and enemy lane.

For each lane:

* inspect slots from right to left
* if a card has `ShiftLeft`, swap with the left adjacent ally if one exists
* otherwise, if a card has `ShiftRight`, swap with the right adjacent ally if one exists

### Locked Rule

Movement is processed from right to left.

This is a gameplay rule and must not be replaced by a collect-then-apply batch approach.

### Important Notes

* movement swaps occupied allied positions
* movement does not change `EnterRoundIndex`
* movement does not directly change combat power
* movement result persists to the next round because slot occupancy has changed

---

## Phase 4: Board Derived

### Purpose

This phase applies board-dependent derived effects after movement has finished.

### Reads

* moved board positions
* current occupied slots
* traits on occupied cards
* current `BoardCard.CurrentPower`

### Writes

* updated `BoardCard.CurrentPower`
* logs
* snapshot

### Current Effects

The current board-derived effects are:

* `AdjacentAid`
* `Suppress`

### Behavior

Derived effects should be calculated in two steps:

#### Step 1

Collect deltas into temporary arrays for each side.

#### Step 2

Apply the deltas to `CurrentPower`.

### Why Use Deltas

Directly modifying `CurrentPower` while scanning would create order-dependent behavior inside the same phase.

Using temporary deltas keeps this phase easier to reason about and debug.

### Current Trait Rules

#### AdjacentAid

A card with `AdjacentAid` gives +2 power to each adjacent allied card.

#### Suppress

A card with `Suppress` gives -2 power to the opposing enemy card in the same slot.

### Important Notes

* this phase happens after movement
* this phase does not apply damage
* no artificial lower bound should be introduced unless gameplay rules explicitly require one

---

## Phase 5: Resolve Open Slots

### Purpose

This phase resolves rock-paper-scissors combat at every open slot.

### Reads

* all open slots
* both sides' occupied board cards at those slots
* each card's `CurrentPower`
* each card's `RpsType`

### Writes

* per-slot combat results
* accumulated damage totals in round-local context
* `BoardCard.DamageDealtThisRound`
* logs
* snapshot

### Behavior

For each open slot:

* compare player and enemy RPS types
* determine winner or tie
* if tie, deal no damage
* if one side wins, deal:

`max(1, winner power - loser power)`

Damage is accumulated, not immediately applied to HP.

### Important Notes

* slot combat should be represented explicitly using `SlotCombatResult`
* damage dealt by the specific winning board card must be recorded
* this phase must not directly modify player or enemy HP

### Why BoardCard Damage Tracking Matters

`Lifesteal` depends on damage dealt by the newly played card itself, not by the team total.

---

## Phase 6: Apply Merged Damage

### Purpose

This phase finalizes the simultaneous merged-damage consequence produced by all resolved open slots.

The accepted gameplay phase name remains `ApplyMergedDamage`.

Within the current architecture, `RoundResolver` records the finalized merged-damage consequence, while `BattleService` later performs the authoritative runtime HP mutation.

### Reads

* accumulated damage to player
* accumulated damage to enemy
* slot combat results
* per-card `DamageDealtThisRound`

### Writes

* finalized merged damage totals in round-local context
* `DamageToPlayer`
* `DamageToEnemy`
* logs
* phase snapshot

### Behavior

Damage from all open slots is accumulated before this phase.

Player-side and enemy-side damage are treated as simultaneous round consequences.

`RoundResolver` must not:

* subtract damage from authoritative runtime HP
* classify either side as defeated
* stop resolution because projected HP would reach zero or below

### Important Rule

Damage must be accumulated first and represented as one merged consequence per side.

The resolver must not reduce HP slot-by-slot during combat resolution.

### Relationship to authoritative application

After `RoundResolver` returns, `BattleService` applies:

```
PlayerHpBefore - DamageToPlayer
EnemyHpBefore - DamageToEnemy
```

and records:

* `PlayerHpAfterMergedDamage`
* `EnemyHpAfterMergedDamage`

Those values may be zero or negative.

They remain intermediate application values, not official survival results.

### Snapshot interpretation

The `ApplyMergedDamage` phase snapshot represents the board and the finalized merged-damage consequence at that rule phase.

It is not, by itself, the authoritative HP-after-damage record.

The authoritative HP timeline is stored in the finalized `RoundResult`.

---

## Phase 7: Post Resolve

### Purpose

This phase evaluates post-resolve effects for cards newly played this round.

It always executes after the merged-damage consequence has been finalized.

### Reads

* newly entered player board card
* newly entered enemy board card
* each new card's traits
* `DamageDealtThisRound`
* finalized slot combat and merged-damage consequences

### Writes

* `HealToPlayer`
* `HealToEnemy`
* source card permanent growth when applicable
* logs
* phase snapshot

### Current Effects

The current post-resolve effects are:

* `Regrow`
* `Lifesteal`
* `Growth`

### Behavior

This phase should only inspect the two cards that entered this round.

#### Regrow

Add the configured Regrow amount to the appropriate raw healing total.

#### Lifesteal

If the newly played card dealt damage this round, add that card's `DamageDealtThisRound` to the appropriate raw healing total.

#### Growth

Increase the source card's permanent power bonus by the configured Growth amount.

### Full-execution rule

Post Resolve must execute even when the merged-damage consequence would temporarily reduce player or enemy HP to zero or below.

The resolver must not perform an early survival check between Phase 6 and Phase 7.

Examples of behavior that must remain valid:

```
projected player HP after merged damage <= 0
Regrow produces raw healing
final applied player HP > 0
```

and:

```
projected player HP after merged damage <= 0
Lifesteal produces raw healing
final applied player HP > 0
```

### Growth rule

`Growth` modifies persistent card state, not current-round board combat power.

It should update:

* `BoardCard.SourceCard.PermanentPowerBonus`

It must not retroactively affect combat already resolved in the current round.

`Growth` still executes during a round that later results in player defeat because the full seven-phase pipeline must complete.

### Suggested Internal Order

A stable internal order is recommended:

1. `Regrow`
2. `Lifesteal`
3. `Growth`

This keeps behavior deterministic and easy to inspect.

### HP Application Boundary

`HealToPlayer` and `HealToEnemy` are raw healing consequences.

`RoundResolver` does not:

* add healing to authoritative HP
* clamp healing against max HP
* calculate actual healing applied
* establish final HP
* classify survival

Those steps belong to `BattleService`.

### Current-v1 boundary assumption

The current v1 post-resolve traits can be evaluated without authoritatively mutating HP inside `RoundResolver`.

If a future accepted post-resolve effect explicitly depends on authoritative HP after merged damage, the resolver/application boundary must be reviewed deliberately rather than silently introducing a second HP calculation.

---

## Rule Resolution and Authoritative Application

The system has two related but distinct steps.

### Step 1: rule resolution

`RoundResolver` completes all seven phases and produces:

* raw merged damage
* raw post-resolve healing
* slot combat results
* logs
* snapshots
* persistent non-HP effects such as `Growth`

At the end of this step, the round is rule-resolved but `RoundResult` is not yet finalized.

### Step 2: authoritative application

`BattleService` applies the resolved consequences in this order:

```
read HP before application
→ apply merged damage
→ record HP after merged damage
→ apply raw post-resolve healing
→ clamp to max HP
→ record actual healing applied
→ establish final HP
→ finalize RoundResult
→ classify battle completion
```

### Classification priority

After final HP is established, `BattleService` classifies the round in this order:

```
if final player HP <= 0
    PlayerDefeated
else if final enemy HP <= 0
    EnemyDefeated
else if the third round was resolved
    AllRoundsCompleted
else
    battle continues
```

This classification belongs to battle-level application, not to `RoundResolver`.

### Simultaneous zero

If both final HP values are zero or below, the official completion reason is:

```
BattleCompletionReason.PlayerDefeated
```

`RoundResolver` does not make this decision.

### Not an additional RoundPhase

Authoritative application and completion classification do not add a new value to `RoundPhase`.

The accepted seven-phase gameplay pipeline remains unchanged.

---

## Logs

### Purpose

Round logs are intended to support:

* debugging
* implementation verification
* future step-by-step presentation

### Expectations

Logs should be:

* concise
* phase-aware
* useful for understanding what happened

### Suggested Content

Typical log entries may describe:

* entered cards
* movement swaps
* derived buffs and debuffs
* slot combat results
* post-resolve healing and growth
* finalized merged-damage consequences
* raw Regrow and Lifesteal healing
* persistent Growth application
* clear distinction between raw healing and actual healing after clamp

Logs are not the authoritative state, but they are an important inspection tool.

### Application-value logging

`RoundResolver` logs may describe raw rule consequences.

Final HP, HP after merged damage, and actual healing applied are established later by `BattleService`.

Presentation should prefer finalized `RoundResult` fields for the authoritative HP timeline rather than attempting to infer final HP from resolver logs.

---

## Phase Snapshots

### Purpose

Phase snapshots capture board state after each resolution phase.

### Why They Exist

They support:

* debugging
* round inspection
* future presentation sequencing
* clearer failure analysis during testing

### Scope

Snapshots should be captured after each of the seven phases, not only once at the end.

That means a round can produce snapshots for:

* Enter
* Fixed Self
* Movement
* Board Derived
* Resolve Open Slots
* Apply Merged Damage
* Post Resolve

### Important Note

Snapshots are not authoritative gameplay state.  
They are derived inspection data and normally should not be treated as mandatory save data.

### HP visibility boundary

Phase snapshots primarily capture rule-phase board state.

They do not replace the authoritative HP application fields in `RoundResult`.

In particular:

* the `ApplyMergedDamage` snapshot may show the finalized damage consequence
* the `PostResolve` snapshot may show raw healing and persistent trait effects
* `PlayerHpAfterMergedDamage` and `EnemyHpAfterMergedDamage` are finalized by `BattleService`
* final HP is finalized by `BattleService`

Debug presentation should combine:

* phase snapshots for board evolution
* finalized `RoundResult` fields for HP evolution

---

## What Persists After a Round

After battle-layer application of the resolved round, the following values persist:

* player HP
* enemy HP
* current board positions
* slot open state
* cards already used this battle
* permanent power growth from `Growth`

---

## What Does Not Persist After a Round

The following should be recalculated or reset next round:

* `CurrentPower`
* `FixedSelfPower`
* `DamageDealtThisRound`
* board-derived temporary deltas
* accumulated damage totals
* accumulated healing totals

---

## Important Summary Rule

A good mental model is:

position persists  
combat values recalculate  
growth persists  
damage totals are temporary

This is one of the most important conceptual rules in the whole combat system.

---

## Relationship to BattleService

`RoundResolver` and `BattleService` have deliberately different responsibilities.

### `RoundResolver` owns

* the fixed seven-phase rule pipeline
* round-local combat calculation
* slot combat resolution
* raw merged damage totals
* raw post-resolve healing totals
* per-card damage attribution used by Lifesteal
* persistent non-HP post-resolve effects such as Growth
* logs
* phase snapshots
* creation of the non-finalized `RoundResult`

### `BattleService` owns

* reading authoritative HP before application
* applying merged damage
* recording HP after merged damage
* applying post-resolve healing
* enforcing max-HP clamp
* recording actual healing applied
* establishing final authoritative HP
* finalizing the `RoundResult`
* adding the finalized result to battle history
* classifying player defeat, enemy defeat, all-round completion, or continuation
* creating and storing any `PendingBattleOutcome`
* advancing battle-level flow

### Why this separation matters

If both layers independently calculate final applied HP, healing clamp, or survival outcome, their results may diverge.

The intended design is:

```
RoundResolver computes rule consequences
→ BattleService applies consequences once
→ BattleService establishes final HP once
→ BattleService classifies battle completion once
```

### Presentation boundary

Presentation may display:

* raw consequences
* intermediate HP
* actual healing
* final HP
* the fixed battle outcome

Presentation must not calculate or alter any of them.

---

## Testing Implications

`RoundResolver` remains one of the highest-value Edit Mode test targets.

### Resolver-owned tests

High-value resolver tests include:

* RPS outcome rules
* minimum damage rule
* empower recalculation
* movement order
* adjacent aid behavior
* suppress behavior
* simultaneous merged-damage consequence calculation
* Regrow raw healing
* Lifesteal raw healing
* Growth persistent update
* post-resolve effects only applying to newly played cards
* snapshot count and exact seven-phase order

### Full-pipeline tests

`RoundResolverPostResolveTests` should explicitly verify:

* Post Resolve executes when projected player HP after merged damage would be zero or below
* Regrow raw healing is still produced after projected lethal damage
* Lifesteal raw healing is still produced after projected lethal damage
* Growth still executes after projected lethal damage
* no player-death or enemy-defeat classification occurs inside `RoundResolver`
* all seven snapshots are produced in a temporary-zero scenario

### Mutation-boundary tests

Resolver tests should verify that:

* authoritative player HP is not mutated by `RoundResolver`
* authoritative enemy HP is not mutated by `RoundResolver`
* max-HP clamp is not performed by `RoundResolver`
* final HP is not independently calculated by `RoundResolver`

### Companion BattleService tests

The following behavior belongs in `BattleServiceTests`, not resolver tests:

* HP before application
* HP after merged damage
* raw healing versus actual healing
* healing clamp
* final HP
* temporary zero followed by survival
* final HP exactly zero
* player-death classification
* enemy-defeat classification
* simultaneous-zero priority
* first- or second-round battle completion
* pending battle outcome creation

### Important test boundary

Resolver tests prove that the seven-phase rule pipeline produces the correct consequences.

BattleService tests prove that those consequences are applied exactly once and produce the correct final authoritative state.

---

## Summary

`RoundResolver` is the rule engine for exactly one round.

It must:

* follow the locked seven-phase pipeline
* complete all seven phases without early death interruption
* recalculate combat state explicitly each round
* keep movement and board-derived behavior deterministic
* accumulate damage before representing it as one merged consequence
* evaluate Post Resolve even when projected HP after damage is zero or below
* apply post-resolve effects only to newly played cards
* produce raw merged damage and raw healing consequences
* produce logs and snapshots as first-class debug outputs
* create a non-finalized `RoundResult`
* avoid authoritative HP mutation, max-HP clamp, survival classification, and battle progression

`BattleService` then:

* applies damage and healing
* records intermediate and final HP
* enforces clamp
* finalizes `RoundResult`
* classifies battle completion

This preserves the locked seven-phase gameplay rule while keeping authoritative HP application, battle outcome classification, and Presentation responsibilities clearly separated.