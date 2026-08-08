## 2024-05-24 - Avoid duplicate Mathf.Sqrt in distance and normal calculations
**Learning:** Both `.magnitude` and `.normalized` trigger `Mathf.Sqrt`. Calling both sequentially is an anti-pattern.
**Action:** Use `.sqrMagnitude` for early exits. If required, calculate `Mathf.Sqrt` once, cache it, and manually divide the vector by the cached distance to normalize it.
## 2025-02-14 - Remove redundant normalizations after cross product of orthogonal normalized vectors
**Learning:** The cross product of two orthogonal, normalized vectors inherently results in a normalized vector. Calling `.normalized` on the result of `Vector3.Cross` in this scenario is an expensive and redundant `Mathf.Sqrt()` operation that should be avoided.
**Action:** Avoid calling `.normalized` on the result of a cross product if the inputs are already known to be normalized and orthogonal.
## 2025-10-24 - Avoid FindObjectsOfType in frequent loops
**Learning:** `FindObjectsOfType` is an expensive O(N) operation that scans all active objects in the scene. Calling it in hot paths like `Sense` or `Update` methods causes severe performance degradation.
**Action:** Implement a centralized static registry (e.g. `HashSet<T> ActiveInstances`) populated during `OnEnable`/`OnDisable` lifecycle hooks to perform O(1) lookups and O(K) iterations instead of scene-wide scans.
