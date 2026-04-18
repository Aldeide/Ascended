# AbilityGraph Node Reference

This document provides a comprehensive reference for the high-utility nodes added to the **AbilityGraph** visual scripting system. These nodes are designed to integrate seamlessly with the Gameplay Ability System (GAS) while providing advanced logic, math, and utility functionality.

---

## 🛠️ Ability System Nodes

Nodes that interact directly with the GAS core (Tags, Effects, Attributes).

### 1. `HasTagNode`
Checks if the ability owner possesses a specific Gameplay Tag.
- **Inputs**: Exec (In), Tag (GameplayTag)
- **Outputs**: Exec (True), Exec (False), Result (Bool)
- **Usage**: Used for branching logic based on the owner's state (e.g., checking for "State.Stunned").

### 2. `RemoveEffectByTagNode`
Removes all active Gameplay Effects from the owner that grant the specified tag.
- **Inputs**: Exec (In), Tag (GameplayTag)
- **Outputs**: Exec (Out)
- **Usage**: Useful for "Cleanse" or "Dispel" abilities.

### 3. `ModifyAttributeBaseNode`
Permanently modifies the **BaseValue** of an attribute.
- **Inputs**: Exec (In), AttributeName (String), Value (Float)
- **Outputs**: Exec (Out)
- **Usage**: Used for permanent upgrades or intrinsic character progression via graphs.

### 4. `CheckCooldownNode`
Queries the remaining time and status of a cooldown associated with a tag.
- **Inputs**: Exec (In), CooldownTag (GameplayTag)
- **Outputs**: Exec (Out), IsOnCooldown (Bool), RemainingTime (Float)
- **Usage**: Pre-activation checks or UI data flow.

### 5. `GetAbilityLevelNode`
Retrieves the current level of the ability instance.
- **Inputs**: None
- **Outputs**: Level (Int)
- **Usage**: Scaling damage or effect magnitudes based on ability rank.

---

## 📐 Math & Vector Nodes

Generic mathematical operations optimized for graph processing.

### 1. `FloatArithmeticNode`
Performs basic binary operations on floats.
- **Operations**: Add, Subtract, Multiply, Divide
- **Inputs**: A (Float), B (Float)
- **Outputs**: Result (Float)

### 2. `ComparisonNode`
Compares two floats and returns a boolean result.
- **Operations**: Equal, NotEqual, Greater, Less, GreaterOrEqual, LessOrEqual
- **Inputs**: A (Float), B (Float)
- **Outputs**: Result (Bool)

### 3. `Vector3Compose / Decompose`
Converts between `Vector3` and individual `float` components.
- **Compose**: X, Y, Z -> Vector3
- **Decompose**: Vector3 -> X, Y, Z

### 4. `SelectNode`
A truly generic selection node that picks between two values based on a boolean condition.
- **Inputs**: Condition (Bool), True Value (Dynamic), False Value (Dynamic)
- **Outputs**: Result (Dynamic)
- **Dynamic Behavior**: The node automatically adapts its port types to match the first connection. If you connect a `float` to "True Value", the "False Value" and "Result" ports will also become `float` ports.
- **Supported Types**: Any serializable Unity type (Floats, Vectors, Ints, Strings, etc.).
- **Usage**: Used for conditional logic within the graph without branching the execution flow.

### 5. `RandomFloatInRangeNode`
Generates a random value between a minimum and maximum.
- **Inputs**: Min (Float), Max (Float)
- **Outputs**: Result (Float)

### 6. `Vector3DistanceNode`
Calculates the Euclidean distance between two 3D points.
- **Inputs**: A (Vector3), B (Vector3)
- **Outputs**: Distance (Float)

---

## 🏗️ Utility & Spatial Nodes

Nodes for world interaction and graph flow.

### 1. `WaitUntilTagRemovedNode`
Asynchronously pauses the graph execution until a specific tag is removed from the owner.
- **Inputs**: Exec (In), Tag (GameplayTag)
- **Outputs**: Exec (Done)
- **Type**: `WaitableNode` (Async)

### 2. `GetOwnerTransformNode`
Retrieves the position and rotation of the actor owning the ability system.
- **Outputs**: Position (Vector3), Rotation (Vector3/Euler)
- **Usage**: Spawning projectiles or calculating relative offsets.

### 3. `GetTargetLocationNode`
Extracts `MuzzlePosition` and `TargetPosition` from the `AbilityData` provided at activation.
- **Outputs**: MuzzleLocation (Vector3), TargetLocation (Vector3)
- **Usage**: Connecting the visual "muzzle" point to the logical target.

### 4. `PlayCueAtLocationNode`
Triggers a Gameplay Cue at a specific world position rather than on an actor.
- **Inputs**: Exec (In), CueTag (GameplayTag), Location (Vector3)
- **Outputs**: Exec (Out)

---

## 🔗 Integration with GraphRunner

The **`GraphRunner`** ensures that:
1. **Network Safety**: All state-changing nodes (like `ModifyAttributeBaseNode`) are executed with appropriate checks.
2. **Execution Order**: The graph follows standard depth-first execution for linear nodes.
3. **Async Handling**: `WaitableNodes` correctly suspend the runner until their internal conditions are met, allowing for complex multi-stage abilities without blocking the main game thread.
