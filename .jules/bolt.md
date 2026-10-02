## 2024-05-24 - Avoid duplicate Mathf.Sqrt in distance and normal calculations
**Learning:** Both `.magnitude` and `.normalized` trigger `Mathf.Sqrt`. Calling both sequentially is an anti-pattern.
**Action:** Use `.sqrMagnitude` for early exits. If required, calculate `Mathf.Sqrt` once, cache it, and manually divide the vector by the cached distance to normalize it.
## 2025-02-14 - Remove redundant normalizations after cross product of orthogonal normalized vectors
**Learning:** The cross product of two orthogonal, normalized vectors inherently results in a normalized vector. Calling `.normalized` on the result of `Vector3.Cross` in this scenario is an expensive and redundant `Mathf.Sqrt()` operation that should be avoided.
**Action:** Avoid calling `.normalized` on the result of a cross product if the inputs are already known to be normalized and orthogonal.

## 2026-10-02 - Optimize scene-wide lookups in AI sensors
**Learning:** `Object.FindObjectsOfType` is an extremely slow operation when used in hot paths like AI sensor Updates. Since the framework operates continuously, these calls accumulate significant overhead.
**Action:** Maintain a centralized static registry (`HashSet<AbilitySystemComponent> ActiveInstances`) and populate it during `OnEnable`/`OnDisable`. Iterate this collection instead of using `FindObjectsOfType`.
