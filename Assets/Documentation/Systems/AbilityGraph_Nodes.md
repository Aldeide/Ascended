# AbilityGraph Node Reference

This document provides a comprehensive reference for all nodes available in the **AbilityGraph** visual scripting system. These nodes integrate with the Gameplay Ability System (GAS) and provide logic, math, and utility functionality for building complex abilities.

---

## 🚀 Entry / Exit Nodes

Graph lifecycle hooks. Every graph must start with one of these.

### `ActivateAbilityNode`
The entry point node executed when the ability activates.
- **Type**: `AbilityStartNode` (entry point)
- **Menu**: `Abilities/ActivateAbility`

### `EndAbilityNode`
The entry point node executed when the ability ends.
- **Type**: `AbilityStartNode` (entry point)
- **Menu**: `Abilities/EndAbility`

---

## 🛠️ Ability System Nodes

Nodes that interact directly with the GAS core (Tags, Effects, Attributes, Abilities).

### `ApplyEffectToOwnerNode`
Instantiates and applies an `EffectDefinition` to the ability owner.
- **Inputs**: Exec (In)
- **Outputs**: Exec (Out)
- **Properties**: `EffectDefinition` (ScriptableObject reference)
- **Menu**: `Abilities/ApplyEffectToOwner`

### `ApplyEffectToTargetNode`
Instantiates and applies an `EffectDefinition` to a specified target GameObject.
- **Inputs**: Exec (In), Target (GameObject), Level (Int)
- **Outputs**: Exec (Out)
- **Properties**: `EffectDefinition` (ScriptableObject reference), `ServerOnly` (Bool - if true, only applies on the server)
- **Menu**: `Abilities/ApplyEffectToTarget`

### `CallEndAbilityNode`
Calls `TryEndAbility()` on the current ability — used to end it from within the graph.
- **Inputs**: Exec (In)
- **Outputs**: Exec (Out)
- **Menu**: `Abilities/CallEndAbility`

### `CommitCostAndCooldown`
Commits the ability's cost and cooldown, consuming resources and starting the cooldown timer.
- **Inputs**: Exec (In)
- **Outputs**: Exec (Out)
- **Menu**: `Abilities/CommitCostAndCooldown`

### `HasTagNode`
Checks if the ability owner possesses a specific Gameplay Tag.
- **Inputs**: Exec (In), Tag (GameplayTag)
- **Outputs**: Exec (True), Exec (False), Result (Bool)
- **Menu**: `Abilities/HasTag`
- **Usage**: Branching logic based on owner state (e.g., `State.Stunned`).

### `AddTagToOwnerNode`
Adds a loose Gameplay Tag to the ability owner.
- **Inputs**: Exec (In), Tag (GameplayTag)
- **Outputs**: Exec (Out)
- **Menu**: `Abilities/Add Tag To Owner`
- **Usage**: Dynamically applying transient tags during ability execution.

### `RemoveTagFromOwnerNode`
Removes a loose Gameplay Tag from the ability owner.
- **Inputs**: Exec (In), Tag (GameplayTag)
- **Outputs**: Exec (Out)
- **Menu**: `Abilities/Remove Tag From Owner`
- **Usage**: Reverting a tag applied during the ability.

### `RemoveEffectByTagNode`
Removes all active Gameplay Effects from the owner that grant the specified tag.
- **Inputs**: Exec (In), Tag (GameplayTag)
- **Outputs**: Exec (Out)
- **Menu**: `Abilities/RemoveEffectByTag`
- **Usage**: "Cleanse" or "Dispel" abilities.

### `CheckCooldownNode`
Queries the remaining time and active status of a cooldown.
- **Inputs**: Exec (In), CooldownTag (GameplayTag)
- **Outputs**: Exec (Out), IsOnCooldown (Bool), RemainingTime (Float)
- **Menu**: `Abilities/CheckCooldown`

### `GetAbilityLevelNode`
Retrieves the current level of the ability instance.
- **Outputs**: Level (Int)
- **Usage**: Scaling damage or effect magnitudes by ability rank.

### `SendGameplayEventNode`
Dispatches a `DynamicGameplayEvent` with a Tag and Magnitude to a target Actor.
- **Inputs**: Exec (In), Target (GameObject), Event Tag (GameplayTag), Magnitude (Float)
- **Outputs**: Exec (Out)
- **Menu**: `Abilities/Send Gameplay Event`
- **Usage**: Notifying other systems or triggering abilities on other targets.

---

## 📊 Attribute Nodes

### `GetAttributeNode`
Reads the `CurrentValue` and `BaseValue` of any attribute on the owner.
- **Outputs**: Current Value (Float), Base Value (Float)
- **Properties**: `attributeFullName` (dropdown: `AttributeSet.AttributeName`)
- **Menu**: `Attributes/GetAttribute`

### `GetAttributePercentNode`
Calculates the percentage/ratio between a Current Attribute and a Max Attribute.
- **Outputs**: Percent (Float)
- **Properties**: `CurrentAttributeFullName` (dropdown), `MaxAttributeFullName` (dropdown)
- **Menu**: `Attributes/Get Attribute Percent`
- **Usage**: Checking percentage-based thresholds (e.g. current health / max health).

### `ModifyAttributeBaseNode`
Permanently modifies the **BaseValue** of an attribute.
- **Inputs**: Exec (In), AttributeName (String), Value (Float)
- **Outputs**: Exec (Out)
- **Usage**: Permanent upgrades or character progression via graphs.

---

## ⏳ Ability Tasks (Async Waitable Nodes)

Special async nodes that yield graph execution until a specific task completes.

### `WaitTargetDataNode`
Spawns a targeting actor and waits for the client to confirm or cancel targeting.
- **Inputs**: Exec (In)
- **Outputs**: Exec (Out), Target Data (TargetDataHandle)
- **Properties**: `TargetActorPrefab` (AbilityTargetActor reference)
- **Type**: `WaitableNode` (Async)
- **Menu**: `Abilities/Wait Target Data`

### `WaitInputPressNode`
Asynchronously waits until the local player presses the ability activation input again.
- **Inputs**: Exec (In)
- **Outputs**: Exec (Out)
- **Type**: `WaitableNode` (Async)
- **Menu**: `Abilities/Wait Input Press`

### `WaitInputReleaseNode`
Asynchronously waits until the local player releases the ability activation input.
- **Inputs**: Exec (In)
- **Outputs**: Exec (Out)
- **Type**: `WaitableNode` (Async)
- **Menu**: `Abilities/Wait Input Release`

### `WaitNetSyncNode`
Synchronizes the client and server execution before proceeding.
- **Inputs**: Exec (In)
- **Outputs**: Exec (Out)
- **Properties**: `SyncType` (AbilityNetSyncType enum)
- **Type**: `WaitableNode` (Async)
- **Menu**: `Abilities/Wait Net Sync`

---

## 🎵 Cue Nodes

### `SendCue`
Dispatches a `CueDefinition` on the owner. Automatically handles local-client prediction suppression.
- **Inputs**: Exec (In)
- **Outputs**: Exec (Out)
- **Properties**: `CueDefinition` (ScriptableObject reference)
- **Menu**: `Cues/SendCue`

### `PlayAudioCue`
Dispatches a `CueAudioDefinition` on the owner for audio-only cues.
- **Inputs**: Exec (In)
- **Outputs**: Exec (Out)
- **Properties**: `CueDefinition` (CueAudioDefinition reference)
- **Menu**: `Cues/PlayAudioCue`

### `PlayCueAtLocationNode`
Triggers a Gameplay Cue at a specific world position rather than on an actor.
- **Inputs**: Exec (In), CueTag (GameplayTag), Location (Vector3)
- **Outputs**: Exec (Out)

---

## 🌍 Spatial / Character Nodes

### `SpawnPrefabNode`
Instantiates a prefab and, if it has a `NetworkObject`, spawns it over the network.
- **Inputs**: Parent (Transform), InstantiateInWorldSpace (Bool)
- **Properties**: `Prefab` (GameObject reference)
- **Menu**: `Spawn/SpawnPrefab`

### `RigidbodyAddForceNode`
Applies a `VelocityChange` force to the owner's `Rigidbody`. Cached on `Initialise`.
- **Inputs**: Exec (In), Force (Vector3)
- **Outputs**: Exec (Out)
- **Menu**: `Character/RigidbodyAddForce`

### `WaitForGroundedEvent`
Asynchronously pauses the graph until the character's movement controller is grounded.
- **Inputs**: Exec (In)
- **Outputs**: Exec (Out)
- **Type**: `WaitableNode` (Async)
- **Menu**: `Character/WaitForGroundedEvent`

### `GetOwnerTransformNode`
Retrieves the position and rotation of the ability system owner.
- **Outputs**: Position (Vector3), Rotation (Vector3/Euler)

### `GetTargetLocationNode`
Extracts `MuzzlePosition` and `TargetPosition` from the `AbilityData` provided at activation.
- **Outputs**: MuzzleLocation (Vector3), TargetLocation (Vector3)
- **Usage**: Connecting the visual muzzle point to the logical target.

---

## 🔀 Logic & Flow Control Nodes

### `BranchNode`
Routes execution to `ExecutesIfTrue` or `ExecutesIfFalse` based on a boolean condition.
- **Inputs**: Condition (Bool)
- **Outputs**: ExecutesIfTrue (Exec), ExecutesIfFalse (Exec)

### `ChanceNode`
Probabilistic branch — routes to `Success` or `Failure` based on a 0–1 probability.
- **Inputs**: Probability (Float, 0–1)
- **Outputs**: Success (Exec), Failure (Exec)
- **Menu**: `Logic/Chance`

### `GateNode`
Passes-through execution only when `IsOpen` is true. Acts as a boolean gate on the execution flow.
- **Inputs**: Open (Exec), Close (Exec), Toggle (Exec)
- **Outputs**: Out (Exec)
- **Menu**: `Logic/Gate`

### `TriggerOnceNode`
Fires the `Out` path only the first time it is executed per ability instance. A `Reset` exec re-arms it.
- **Inputs**: Reset (Exec)
- **Outputs**: Out (Exec)
- **Menu**: `Logic/Trigger Once`

### `DoOnceNode`
Fires the `Executes` path only once until `Reset` or `ResetTrigger()` is called.
- **Inputs**: Reset (Exec), Start Closed (Bool)
- **Outputs**: Executes (Exec)
- **Menu**: `Logic/Do Once`

### `SequenceNode`
Executes multiple execution paths sequentially (`Then 0`, `Then 1`, `Then 2`).
- **Outputs**: Then 0 (Exec), Then 1 (Exec), Then 2 (Exec)
- **Menu**: `Logic/Sequence`

### `SelectNode`
Generic type-adaptive selector — picks between two values based on a boolean. Automatically infers port types from the first connected value.
- **Inputs**: Condition (Bool), True Value (Dynamic), False Value (Dynamic)
- **Outputs**: Result (Dynamic)
- **Supported types**: Any serializable Unity type (floats, vectors, ints, strings, etc.)

---

## 📐 Math & Vector Nodes

### `FloatArithmeticNode`
Binary float operations: Add, Subtract, Multiply, Divide.
- **Inputs**: A (Float), B (Float)
- **Outputs**: Result (Float)

### `ComparisonNode`
Float comparison: Equal, NotEqual, Greater, Less, GreaterOrEqual, LessOrEqual.
- **Inputs**: A (Float), B (Float)
- **Outputs**: Result (Bool)

### `ClampFloatNode`
Clamps a float value between a Min and Max range.
- **Inputs**: Value (Float), Min (Float), Max (Float)
- **Outputs**: Result (Float)
- **Menu**: `Math/Clamp Float`

### `RandomFloatInRangeNode`
Generates a random float between Min and Max.
- **Inputs**: Min (Float), Max (Float)
- **Outputs**: Result (Float)

### `Vector3Compose / Decompose`
Converts between `Vector3` and individual float components.
- **Compose**: X, Y, Z → Vector3
- **Decompose**: Vector3 → X, Y, Z

### `Vector3DistanceNode`
Euclidean distance between two 3D points.
- **Inputs**: A (Vector3), B (Vector3)
- **Outputs**: Distance (Float)

---

## 🔧 Primitive Value Nodes

Constant-value nodes for supplying inline data to other nodes.

| Node | Output Type | Menu |
|---|---|---|
| `FloatNode` | Float | `Primitives/Float` |
| `IntNode` | Int | `Primitives/Int` |
| `StringNode` | String | `Primitives/String` |
| `Vector2Node` | Vector2 | `Primitives/Vector2` |
| `Vector3Node` | Vector3 | `Primitives/Vector3` |

---

## 🛠️ Utility Nodes

### `WaitNode`
Asynchronously pauses graph execution for a fixed duration.
- **Properties**: `Duration` (Float, seconds)
- **Type**: `WaitableNode` (Async)
- **Menu**: `Utilities/WaitForSeconds`

### `WaitUntilTagRemovedNode`
Asynchronously pauses the graph until a specific Gameplay Tag is removed from the owner.
- **Inputs**: Exec (In), Tag (GameplayTag)
- **Outputs**: Exec (Done)
- **Type**: `WaitableNode` (Async)

### `IsServerNode`
Checks if the ability system owner runs with server authority.
- **Outputs**: Is Server (Bool)
- **Menu**: `Utilities/Is Server`

### `IsLocalClientNode`
Checks if the ability is running on the local predicted client.
- **Outputs**: Is Local (Bool)
- **Menu**: `Utilities/Is Local Client`

### `DebugNode`
Logs a value or string to the Unity console. Supports Log, Warning, Error, and Assert levels.
- **Inputs**: Exec (In), Debug Object (object), Log (String)
- **Outputs**: Exec (Out)
- **Settings**: Log Type (LogType)
- **Menu**: `Utilities/Debug`

---

## 🔗 Integration with GraphRunner

The **`GraphRunner`** guarantees:
1. **Network Safety**: State-changing nodes (e.g., `ModifyAttributeBaseNode`) respect server-authority checks.
2. **Execution Order**: Standard depth-first execution for linear nodes.
3. **Async Handling**: `WaitableNode` subclasses correctly suspend the runner until their internal condition resolves, enabling multi-stage abilities without blocking the main thread.
