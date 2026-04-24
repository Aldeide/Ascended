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

---

## 📊 Attribute Nodes

### `GetAttributeNode`
Reads the `CurrentValue` and `BaseValue` of any attribute on the owner.
- **Outputs**: Current Value (Float), Base Value (Float)
- **Properties**: `attributeFullName` (dropdown: `AttributeSet.AttributeName`)
- **Menu**: `Attributes/GetAttribute`

### `ModifyAttributeBaseNode`
Permanently modifies the **BaseValue** of an attribute.
- **Inputs**: Exec (In), AttributeName (String), Value (Float)
- **Outputs**: Exec (Out)
- **Usage**: Permanent upgrades or character progression via graphs.

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

### `GetOwnerTransformNode`
Retrieves the position and rotation of the ability system owner.
- **Outputs**: Position (Vector3), Rotation (Vector3/Euler)

### `GetTargetLocationNode`
Extracts `MuzzlePosition` and `TargetPosition` from the `AbilityData` provided at activation.
- **Outputs**: MuzzleLocation (Vector3), TargetLocation (Vector3)
- **Usage**: Connecting the visual muzzle point to the logical target.

---

## 🔀 Logic Nodes

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
