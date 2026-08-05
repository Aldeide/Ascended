## 2024-05-24 - Avoid duplicate Mathf.Sqrt in distance and normal calculations
**Learning:** Both `.magnitude` and `.normalized` trigger `Mathf.Sqrt`. Calling both sequentially is an anti-pattern.
**Action:** Use `.sqrMagnitude` for early exits. If required, calculate `Mathf.Sqrt` once, cache it, and manually divide the vector by the cached distance to normalize it.
## 2025-02-14 - Remove redundant normalizations after cross product of orthogonal normalized vectors
**Learning:** The cross product of two orthogonal, normalized vectors inherently results in a normalized vector. Calling `.normalized` on the result of `Vector3.Cross` in this scenario is an expensive and redundant `Mathf.Sqrt()` operation that should be avoided.
**Action:** Avoid calling `.normalized` on the result of a cross product if the inputs are already known to be normalized and orthogonal.
## 2024-05-19 - Optimize AI Sensors Scene-wide Lookups
**Learning:** Using `GameObject.FindGameObjectsWithTag("Player")` or `Object.FindObjectsOfType<AbilitySystemComponent>()` inside AI sensor update loops is a significant performance bottleneck due to the $O(N)$ traversal of the entire scene graph on every tick.
**Action:** Implement a static `HashSet<AbilitySystemComponent> ActiveInstances` maintained via standard `OnEnable` and `OnDisable` Unity lifecycle hooks. Replace scene-wide lookups in hot paths with iterations over this centralized registry and use `.sqrMagnitude` instead of `Vector3.Distance` to eliminate redundant square root computations.
