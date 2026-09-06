## 2024-05-24 - Avoid duplicate Mathf.Sqrt in distance and normal calculations
**Learning:** Both `.magnitude` and `.normalized` trigger `Mathf.Sqrt`. Calling both sequentially is an anti-pattern.
**Action:** Use `.sqrMagnitude` for early exits. If required, calculate `Mathf.Sqrt` once, cache it, and manually divide the vector by the cached distance to normalize it.
## 2025-02-14 - Remove redundant normalizations after cross product of orthogonal normalized vectors
**Learning:** The cross product of two orthogonal, normalized vectors inherently results in a normalized vector. Calling `.normalized` on the result of `Vector3.Cross` in this scenario is an expensive and redundant `Mathf.Sqrt()` operation that should be avoided.
**Action:** Avoid calling `.normalized` on the result of a cross product if the inputs are already known to be normalized and orthogonal.
## 2025-02-14 - Use static registry for frequently accessed Unity components
**Learning:** Calling `Object.FindObjectsOfType` in AI evaluation loops (like sensors) creates a significant CPU and GC bottleneck due to $O(N)$ scene searches.
**Action:** Centralize the collection of components into a static `HashSet` registry populated via `OnEnable`/`OnDisable` to convert scene searches into $O(1)$ iterating lookups. Ensure null checks are present when iterating over the registry.
