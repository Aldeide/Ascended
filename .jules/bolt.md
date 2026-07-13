## 2024-05-24 - Avoid duplicate Mathf.Sqrt in distance and normal calculations
**Learning:** Both `.magnitude` and `.normalized` trigger `Mathf.Sqrt`. Calling both sequentially is an anti-pattern.
**Action:** Use `.sqrMagnitude` for early exits. If required, calculate `Mathf.Sqrt` once, cache it, and manually divide the vector by the cached distance to normalize it.
## 2025-02-14 - Remove redundant normalizations after cross product of orthogonal normalized vectors
**Learning:** The cross product of two orthogonal, normalized vectors inherently results in a normalized vector. Calling `.normalized` on the result of `Vector3.Cross` in this scenario is an expensive and redundant `Mathf.Sqrt()` operation that should be avoided.
**Action:** Avoid calling `.normalized` on the result of a cross product if the inputs are already known to be normalized and orthogonal.
## 2025-02-14 - Optimize expensive GameObject queries in hot paths
**Learning:** Using `GameObject.FindGameObjectsWithTag()` and `Object.FindObjectsOfType()` inside frequent update loops or AI sensors causes massive CPU overhead and garbage collection. Also, `Vector3.Distance` calls `Mathf.Sqrt()`.
**Action:** Centralize tracking using a static registry populated during `OnEnable`/`OnDisable` (e.g., `AbilitySystemComponent.ActiveInstances`) and replace `Vector3.Distance` with `.sqrMagnitude` where only comparisons are needed.
