## 2024-05-24 - Avoid duplicate Mathf.Sqrt in distance and normal calculations
**Learning:** Both `.magnitude` and `.normalized` trigger `Mathf.Sqrt`. Calling both sequentially is an anti-pattern.
**Action:** Use `.sqrMagnitude` for early exits. If required, calculate `Mathf.Sqrt` once, cache it, and manually divide the vector by the cached distance to normalize it.
## 2025-02-14 - Remove redundant normalizations after cross product of orthogonal normalized vectors
**Learning:** The cross product of two orthogonal, normalized vectors inherently results in a normalized vector. Calling `.normalized` on the result of `Vector3.Cross` in this scenario is an expensive and redundant `Mathf.Sqrt()` operation that should be avoided.
**Action:** Avoid calling `.normalized` on the result of a cross product if the inputs are already known to be normalized and orthogonal.
## 2026-08-29 - Replace FindObjectsOfType with Centralized Static Registry
**Learning:** Object.FindObjectsOfType is slow. In Unity, maintaining a static HashSet<T> of active component instances (updated in OnEnable/OnDisable) is much faster for hot path lookups like sensors and updates.
**Action:** Always use a static registry instead of FindObjectsOfType for frequent queries, ensuring null checks when iterating to handle destroyed instances properly.
