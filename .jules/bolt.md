## 2024-05-24 - Avoid duplicate Mathf.Sqrt in distance and normal calculations
**Learning:** Both `.magnitude` and `.normalized` trigger `Mathf.Sqrt`. Calling both sequentially is an anti-pattern.
**Action:** Use `.sqrMagnitude` for early exits. If required, calculate `Mathf.Sqrt` once, cache it, and manually divide the vector by the cached distance to normalize it.
## 2025-02-14 - Remove redundant normalizations after cross product of orthogonal normalized vectors
**Learning:** The cross product of two orthogonal, normalized vectors inherently results in a normalized vector. Calling `.normalized` on the result of `Vector3.Cross` in this scenario is an expensive and redundant `Mathf.Sqrt()` operation that should be avoided.
**Action:** Avoid calling `.normalized` on the result of a cross product if the inputs are already known to be normalized and orthogonal.
## 2026-09-27 - Optimize scene-wide lookups in hot paths using centralized static registry
**Learning:** Calling `Object.FindObjectsOfType<T>()` during hot paths like `Update` or `Sense` methods (such as AI sensors) creates significant performance bottlenecks and memory allocations.
**Action:** Maintain a centralized static registry (e.g., `HashSet<T> ActiveInstances`) inside the component's `OnEnable` and `OnDisable` methods. Iterate over this registry directly in hot paths, avoiding slow scene-wide lookups. Always include null checks when iterating (`comp == null || comp.gameObject == null`).
