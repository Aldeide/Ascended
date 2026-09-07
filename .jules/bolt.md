## 2024-05-24 - Avoid duplicate Mathf.Sqrt in distance and normal calculations
**Learning:** Both `.magnitude` and `.normalized` trigger `Mathf.Sqrt`. Calling both sequentially is an anti-pattern.
**Action:** Use `.sqrMagnitude` for early exits. If required, calculate `Mathf.Sqrt` once, cache it, and manually divide the vector by the cached distance to normalize it.
## 2025-02-14 - Remove redundant normalizations after cross product of orthogonal normalized vectors
**Learning:** The cross product of two orthogonal, normalized vectors inherently results in a normalized vector. Calling `.normalized` on the result of `Vector3.Cross` in this scenario is an expensive and redundant `Mathf.Sqrt()` operation that should be avoided.
**Action:** Avoid calling `.normalized` on the result of a cross product if the inputs are already known to be normalized and orthogonal.
## 2025-02-14 - Optimize scene-wide lookups in hot paths
**Learning:** `Object.FindObjectsOfType` is a very slow method that should be avoided in hot paths like AI sensor updates, as it traverses the entire scene hierarchy and allocates memory. Additionally, `Vector3.Distance` triggers `Mathf.Sqrt` which is unnecessary for simple distance comparisons.
**Action:** Maintain a centralized static registry (e.g., `HashSet<AbilitySystemComponent> ActiveInstances`) populated during Unity lifecycle events (`OnEnable`/`OnDisable`). Iterate this registry directly and use `.sqrMagnitude` instead of `Vector3.Distance` for faster distance comparisons.
