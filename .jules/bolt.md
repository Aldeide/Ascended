## 2024-05-24 - Avoid duplicate Mathf.Sqrt in distance and normal calculations
**Learning:** Both `.magnitude` and `.normalized` trigger `Mathf.Sqrt`. Calling both sequentially is an anti-pattern.
**Action:** Use `.sqrMagnitude` for early exits. If required, calculate `Mathf.Sqrt` once, cache it, and manually divide the vector by the cached distance to normalize it.
## 2025-02-14 - Remove redundant normalizations after cross product of orthogonal normalized vectors
**Learning:** The cross product of two orthogonal, normalized vectors inherently results in a normalized vector. Calling `.normalized` on the result of `Vector3.Cross` in this scenario is an expensive and redundant `Mathf.Sqrt()` operation that should be avoided.
**Action:** Avoid calling `.normalized` on the result of a cross product if the inputs are already known to be normalized and orthogonal.
## 2026-10-11 - Use static registry instead of Object.FindObjectsOfType in hot paths
**Learning:** Calling `FindObjectsOfType` dynamically is an O(n) operation that triggers garbage collection and causes severe performance bottlenecks when used frequently, such as in AI sensors or update loops.
**Action:** Maintain a centralized static registry (e.g., `HashSet`) and populate it during `OnEnable`/`OnDisable` to eliminate expensive scene-wide lookups. Always ensure proper null checks during iteration, and manage lifecycle registrations explicitly in tests to avoid state leakage.
