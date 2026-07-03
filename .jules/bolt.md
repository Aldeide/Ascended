## 2024-05-24 - Avoid duplicate Mathf.Sqrt in distance and normal calculations
**Learning:** Both `.magnitude` and `.normalized` trigger `Mathf.Sqrt`. Calling both sequentially is an anti-pattern.
**Action:** Use `.sqrMagnitude` for early exits. If required, calculate `Mathf.Sqrt` once, cache it, and manually divide the vector by the cached distance to normalize it.
## 2025-02-14 - Remove redundant normalizations after cross product of orthogonal normalized vectors
**Learning:** The cross product of two orthogonal, normalized vectors inherently results in a normalized vector. Calling `.normalized` on the result of `Vector3.Cross` in this scenario is an expensive and redundant `Mathf.Sqrt()` operation that should be avoided.
**Action:** Avoid calling `.normalized` on the result of a cross product if the inputs are already known to be normalized and orthogonal.
## 2025-02-14 - Optimize AI Target Lookups
**Learning:** `FindObjectsOfType<AbilitySystemComponent>` and `GameObject.FindGameObjectsWithTag("Player")` were being called in tight loops inside AI sensors and decision makers, causing severe GC and iteration overhead across the scene hierarchy.
**Action:** Implemented a centralized `ActiveInstances` HashSet within `AbilitySystemComponent` managed via `OnEnable`/`OnDisable` lifecycle hooks. AI components now iterate over this statically cached collection (an O(N) operation over a small subset) instead of traversing the entire scene, yielding a massive performance boost for hot paths.
