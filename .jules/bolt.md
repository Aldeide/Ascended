## 2024-05-24 - Avoid duplicate Mathf.Sqrt in distance and normal calculations
**Learning:** Both `.magnitude` and `.normalized` trigger `Mathf.Sqrt`. Calling both sequentially is an anti-pattern.
**Action:** Use `.sqrMagnitude` for early exits. If required, calculate `Mathf.Sqrt` once, cache it, and manually divide the vector by the cached distance to normalize it.
## 2025-02-14 - Remove redundant normalizations after cross product of orthogonal normalized vectors
**Learning:** The cross product of two orthogonal, normalized vectors inherently results in a normalized vector. Calling `.normalized` on the result of `Vector3.Cross` in this scenario is an expensive and redundant `Mathf.Sqrt()` operation that should be avoided.
**Action:** Avoid calling `.normalized` on the result of a cross product if the inputs are already known to be normalized and orthogonal.
## 2025-02-14 - Optimize scene-wide lookups using a static registry
**Learning:** Frequent use of `Object.FindObjectsOfType` in hot paths like AI sensors creates significant CPU overhead, especially as the number of entities scales.
**Action:** Replace `Object.FindObjectsOfType` calls with lookups into a centralized static registry (e.g., `HashSet<AbilitySystemComponent> ActiveInstances`) maintained within standard lifecycle hooks like `OnEnable`/`OnDisable`.
