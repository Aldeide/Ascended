## 2024-05-24 - Avoid duplicate Mathf.Sqrt in distance and normal calculations
**Learning:** Both `.magnitude` and `.normalized` trigger `Mathf.Sqrt`. Calling both sequentially is an anti-pattern.
**Action:** Use `.sqrMagnitude` for early exits. If required, calculate `Mathf.Sqrt` once, cache it, and manually divide the vector by the cached distance to normalize it.
## 2025-02-14 - Remove redundant normalizations after cross product of orthogonal normalized vectors
**Learning:** The cross product of two orthogonal, normalized vectors inherently results in a normalized vector. Calling `.normalized` on the result of `Vector3.Cross` in this scenario is an expensive and redundant `Mathf.Sqrt()` operation that should be avoided.
**Action:** Avoid calling `.normalized` on the result of a cross product if the inputs are already known to be normalized and orthogonal.
## 2025-02-28 - Optimize Unity GameObject.Find and Vector3.Distance
**Learning:** Frequent calls to `FindObjectsOfType` or `GameObject.FindGameObjectsWithTag` combined with `Vector3.Distance` in `Update` loops or recurring AI sensors create significant GC allocation and CPU bottlenecks due to scene traversal and `Mathf.Sqrt`.
**Action:** Use a static `HashSet` registry (e.g., `ActiveInstances`) populated via `OnEnable`/`OnDisable` for scene-wide lookups, and replace `Vector3.Distance(a, b) < dist` with `(a - b).sqrMagnitude < dist * dist` to bypass expensive square root operations while maintaining correctness.
