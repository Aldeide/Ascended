## 2024-05-24 - Avoid duplicate Mathf.Sqrt in distance and normal calculations
**Learning:** Both `.magnitude` and `.normalized` trigger `Mathf.Sqrt`. Calling both sequentially is an anti-pattern.
**Action:** Use `.sqrMagnitude` for early exits. If required, calculate `Mathf.Sqrt` once, cache it, and manually divide the vector by the cached distance to normalize it.
## 2025-02-14 - Remove redundant normalizations after cross product of orthogonal normalized vectors
**Learning:** The cross product of two orthogonal, normalized vectors inherently results in a normalized vector. Calling `.normalized` on the result of `Vector3.Cross` in this scenario is an expensive and redundant `Mathf.Sqrt()` operation that should be avoided.
**Action:** Avoid calling `.normalized` on the result of a cross product if the inputs are already known to be normalized and orthogonal.
## 2025-02-27 - Avoid FindObjectsOfType in hot paths by using static registries
**Learning:** `FindObjectsOfType` is extremely slow and should not be used in hot paths like `Sense` or `Update` because it searches the whole scene. It's better to maintain a centralized static registry.
**Action:** Use a `HashSet<T>` static registry populated during `OnEnable`/`OnDisable` in the component. Iterate over this registry instead of using `FindObjectsOfType`, and ensure proper cleanup in test `[TearDown]` methods.
