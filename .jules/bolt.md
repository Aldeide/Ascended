## 2024-05-24 - Avoid duplicate Mathf.Sqrt in distance and normal calculations
**Learning:** Both `.magnitude` and `.normalized` trigger `Mathf.Sqrt`. Calling both sequentially is an anti-pattern.
**Action:** Use `.sqrMagnitude` for early exits. If required, calculate `Mathf.Sqrt` once, cache it, and manually divide the vector by the cached distance to normalize it.
## 2024-05-24 - Avoid expensive Unity GameObject lookups in hot paths
**Learning:** Unity's `Object.FindObjectsOfType` and `GameObject.FindGameObjectsWithTag` traverse the scene hierarchy and trigger expensive allocations. Calling them in frequent logic loops (like GOAP sensors) severely degrades performance.
**Action:** Implement static component registries (like `HashSet<T>`) populated during `OnEnable`/`OnDisable` to allow fast, direct iteration over specific components without scene traversal.
