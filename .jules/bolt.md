## 2024-05-24 - Avoid duplicate Mathf.Sqrt in distance and normal calculations
**Learning:** Both `.magnitude` and `.normalized` trigger `Mathf.Sqrt`. Calling both sequentially is an anti-pattern.
**Action:** Use `.sqrMagnitude` for early exits. If required, calculate `Mathf.Sqrt` once, cache it, and manually divide the vector by the cached distance to normalize it.
## 2025-02-14 - Remove redundant normalizations after cross product of orthogonal normalized vectors
**Learning:** The cross product of two orthogonal, normalized vectors inherently results in a normalized vector. Calling `.normalized` on the result of `Vector3.Cross` in this scenario is an expensive and redundant `Mathf.Sqrt()` operation that should be avoided.
**Action:** Avoid calling `.normalized` on the result of a cross product if the inputs are already known to be normalized and orthogonal.
## 2025-02-14 - Replace Object.FindObjectsOfType with a Centralized Static Registry
**Learning:** Using `Object.FindObjectsOfType<T>()` or `GameObject.FindGameObjectsWithTag()` inside sensors and AI evaluation loops causes extreme performance bottlenecks due to per-frame global DOM traversals and garbage allocation in Unity.
**Action:** Replace `Object.FindObjectsOfType<T>()` with a centralized, statically maintained `HashSet<T> ActiveInstances` property inside the component itself, hooked into native Unity `OnEnable()` and `OnDisable()` lifecycles for O(1) performance. Update unit tests to mock and clean the registry manually.
