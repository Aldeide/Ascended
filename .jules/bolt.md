## 2024-05-24 - Avoid duplicate Mathf.Sqrt in distance and normal calculations
**Learning:** Both `.magnitude` and `.normalized` trigger `Mathf.Sqrt`. Calling both sequentially is an anti-pattern.
**Action:** Use `.sqrMagnitude` for early exits. If required, calculate `Mathf.Sqrt` once, cache it, and manually divide the vector by the cached distance to normalize it.
## 2025-02-14 - Remove redundant normalizations after cross product of orthogonal normalized vectors
**Learning:** The cross product of two orthogonal, normalized vectors inherently results in a normalized vector. Calling `.normalized` on the result of `Vector3.Cross` in this scenario is an expensive and redundant `Mathf.Sqrt()` operation that should be avoided.
**Action:** Avoid calling `.normalized` on the result of a cross product if the inputs are already known to be normalized and orthogonal.
## 2025-08-09 - Optimize scene-wide lookups for AbilitySystemComponent
**Learning:** `FindObjectsOfType` is an extremely expensive, O(n) operation when called frequently, and since AI sensors run in loops and `EnemyDecisionMaker` runs every frame, this causes serious performance degradation.
**Action:** Maintain a centralized static registry (e.g., `HashSet<AbilitySystemComponent> ActiveInstances`) populated during Unity lifecycle events (`OnEnable`/`OnDisable`) instead of using slow methods like `Object.FindObjectsOfType` or `GameObject.FindGameObjectsWithTag`. When iterating, always include null checks (`comp == null || comp.gameObject == null`) to avoid `NullReferenceException`s from destroyed objects.
