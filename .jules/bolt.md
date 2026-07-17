## 2024-05-24 - Avoid duplicate Mathf.Sqrt in distance and normal calculations
**Learning:** Both `.magnitude` and `.normalized` trigger `Mathf.Sqrt`. Calling both sequentially is an anti-pattern.
**Action:** Use `.sqrMagnitude` for early exits. If required, calculate `Mathf.Sqrt` once, cache it, and manually divide the vector by the cached distance to normalize it.
## 2025-02-14 - Remove redundant normalizations after cross product of orthogonal normalized vectors
**Learning:** The cross product of two orthogonal, normalized vectors inherently results in a normalized vector. Calling `.normalized` on the result of `Vector3.Cross` in this scenario is an expensive and redundant `Mathf.Sqrt()` operation that should be avoided.
**Action:** Avoid calling `.normalized` on the result of a cross product if the inputs are already known to be normalized and orthogonal.
## 2024-05-27 - Replace FindObjectsOfType with Centralized Registry for High Frequency Queries
**Learning:** Using `Object.FindObjectsOfType<T>()` or `GameObject.FindGameObjectsWithTag` in high frequency operations (like `Update` loops, `EvaluateGoal`, or AI sensors) incurs significant overhead and triggers unnecessary allocations.
**Action:** Cache these references or, for dynamic systems like `AbilitySystemComponent`, implement a centralized static `HashSet<T>` registry that instances self-register to during `OnEnable`/`OnDisable`, significantly improving scene-wide lookups.
