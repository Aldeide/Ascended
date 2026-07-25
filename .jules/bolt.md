## 2024-05-24 - Avoid duplicate Mathf.Sqrt in distance and normal calculations
**Learning:** Both `.magnitude` and `.normalized` trigger `Mathf.Sqrt`. Calling both sequentially is an anti-pattern.
**Action:** Use `.sqrMagnitude` for early exits. If required, calculate `Mathf.Sqrt` once, cache it, and manually divide the vector by the cached distance to normalize it.
## 2025-02-14 - Remove redundant normalizations after cross product of orthogonal normalized vectors
**Learning:** The cross product of two orthogonal, normalized vectors inherently results in a normalized vector. Calling `.normalized` on the result of `Vector3.Cross` in this scenario is an expensive and redundant `Mathf.Sqrt()` operation that should be avoided.
**Action:** Avoid calling `.normalized` on the result of a cross product if the inputs are already known to be normalized and orthogonal.
## 2025-02-15 - Replace FindObjectsOfType with Centralized Static Registry
**Learning:** Unity's `FindObjectsOfType<T>()` and `FindGameObjectsWithTag()` are notoriously slow and cause unnecessary per-frame overhead and garbage collection, especially when used within sensor logic that updates frequently (e.g. AI systems scanning for targets or allies).
**Action:** Replaced dynamic lookup functions in `AllyNeedsHealingSensor`, `TacticalPositionSensor`, `HealTargetSensor`, `EnemyTargetSensor`, and `EnemyDecisionMaker` with a static HashSet tracking active `AbilitySystemComponent` instances populated via Unity's `OnEnable` and `OnDisable` lifecycle methods. This ensures real-time O(1) addition/removal and efficient O(N) iteration without allocations. Always ensure mock test components manually add themselves to the registry since lifecycle methods might not trigger properly in some test environments.
