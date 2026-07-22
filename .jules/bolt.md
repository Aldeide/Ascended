## 2024-05-24 - Avoid duplicate Mathf.Sqrt in distance and normal calculations
**Learning:** Both `.magnitude` and `.normalized` trigger `Mathf.Sqrt`. Calling both sequentially is an anti-pattern.
**Action:** Use `.sqrMagnitude` for early exits. If required, calculate `Mathf.Sqrt` once, cache it, and manually divide the vector by the cached distance to normalize it.
## 2025-02-14 - Remove redundant normalizations after cross product of orthogonal normalized vectors
**Learning:** The cross product of two orthogonal, normalized vectors inherently results in a normalized vector. Calling `.normalized` on the result of `Vector3.Cross` in this scenario is an expensive and redundant `Mathf.Sqrt()` operation that should be avoided.
**Action:** Avoid calling `.normalized` on the result of a cross product if the inputs are already known to be normalized and orthogonal.

## 2024-07-22 - Optimize Component Lookups with Centralized Registries
**Learning:** Heavy reliance on Object.FindObjectsOfType<T>() during AI sensing and updating in Unity introduces significant per-frame overhead and GC allocations.
**Action:** When a component needs to be queried frequently scene-wide, implement a static HashSet (e.g., ActiveInstances) populated in OnEnable/OnDisable to enable O(1) collection access and minimize hot-path performance costs.
