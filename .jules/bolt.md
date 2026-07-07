## 2024-05-24 - Avoid duplicate Mathf.Sqrt in distance and normal calculations
**Learning:** Both `.magnitude` and `.normalized` trigger `Mathf.Sqrt`. Calling both sequentially is an anti-pattern.
**Action:** Use `.sqrMagnitude` for early exits. If required, calculate `Mathf.Sqrt` once, cache it, and manually divide the vector by the cached distance to normalize it.
## 2025-02-14 - Remove redundant normalizations after cross product of orthogonal normalized vectors
**Learning:** The cross product of two orthogonal, normalized vectors inherently results in a normalized vector. Calling `.normalized` on the result of `Vector3.Cross` in this scenario is an expensive and redundant `Mathf.Sqrt()` operation that should be avoided.
**Action:** Avoid calling `.normalized` on the result of a cross product if the inputs are already known to be normalized and orthogonal.
## 2025-02-15 - Use pre-allocated global instances instead of Object.FindObjectsOfType in hot paths
**Learning:** `Object.FindObjectsOfType` is extremely slow and allocates GC. In Unity hot paths (e.g., Update loops, frequent Sensor polling), it causes massive performance degradation and GC pressure.
**Action:** Use centralized, static instance tracking (like a static `HashSet<T>`) where instances register themselves on `OnEnable` and unregister on `OnDisable`. This provides O(1) tracking with zero GC allocation during polling.
