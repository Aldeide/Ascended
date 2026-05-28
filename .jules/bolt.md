## 2024-05-24 - Avoid duplicate Mathf.Sqrt in distance and normal calculations
**Learning:** Both `.magnitude` and `.normalized` trigger `Mathf.Sqrt`. Calling both sequentially is an anti-pattern.
**Action:** Use `.sqrMagnitude` for early exits. If required, calculate `Mathf.Sqrt` once, cache it, and manually divide the vector by the cached distance to normalize it.
## 2024-05-15 - Replace O(N) scene searches with O(1) static registries
**Learning:** `Object.FindObjectsOfType` is a severely expensive operation that transverses the whole scene and should never be used in runtime hot paths like sensors or Update. Caching active component references in a static `HashSet` during their `OnEnable`/`OnDisable` lifecycle reduces the search time from N to O(1) cache access.
**Action:** When a script needs to repeatedly locate specific components globally, implement a static registry pattern. Furthermore, use `.sqrMagnitude` instead of `.Distance` during spatial queries to avoid unnecessary `Mathf.Sqrt()` calculations.
