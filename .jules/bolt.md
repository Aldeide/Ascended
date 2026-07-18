## 2024-05-24 - Avoid duplicate Mathf.Sqrt in distance and normal calculations
**Learning:** Both `.magnitude` and `.normalized` trigger `Mathf.Sqrt`. Calling both sequentially is an anti-pattern.
**Action:** Use `.sqrMagnitude` for early exits. If required, calculate `Mathf.Sqrt` once, cache it, and manually divide the vector by the cached distance to normalize it.
## 2025-02-14 - Remove redundant normalizations after cross product of orthogonal normalized vectors
**Learning:** The cross product of two orthogonal, normalized vectors inherently results in a normalized vector. Calling `.normalized` on the result of `Vector3.Cross` in this scenario is an expensive and redundant `Mathf.Sqrt()` operation that should be avoided.
**Action:** Avoid calling `.normalized` on the result of a cross product if the inputs are already known to be normalized and orthogonal.
## 2025-02-14 - Replace FindObjectsOfType in hot paths with centralized static registry
**Learning:** `Object.FindObjectsOfType<T>()` and `GameObject.FindGameObjectsWithTag()` are extremely expensive, particularly in Unity hot paths (e.g. AI sensors or `Update` loops) as they generate per-frame garbage collection allocations and scan the entire scene.
**Action:** Replace scene-wide lookups in hot paths with a centralized static registry (like `HashSet<T>`) populated exclusively during the `OnEnable` and `OnDisable` lifecycle events of the target component. When iterating through the instances, ensure null checks are performed to avoid `NullReferenceException` for recently destroyed objects.
