## 2024-05-24 - Avoid duplicate Mathf.Sqrt in distance and normal calculations
**Learning:** Both `.magnitude` and `.normalized` trigger `Mathf.Sqrt`. Calling both sequentially is an anti-pattern.
**Action:** Use `.sqrMagnitude` for early exits. If required, calculate `Mathf.Sqrt` once, cache it, and manually divide the vector by the cached distance to normalize it.
## 2025-02-14 - Remove redundant normalizations after cross product of orthogonal normalized vectors
**Learning:** The cross product of two orthogonal, normalized vectors inherently results in a normalized vector. Calling `.normalized` on the result of `Vector3.Cross` in this scenario is an expensive and redundant `Mathf.Sqrt()` operation that should be avoided.
**Action:** Avoid calling `.normalized` on the result of a cross product if the inputs are already known to be normalized and orthogonal.
## 2025-02-14 - Use static registry instead of FindObjectsOfType
**Learning:** `Object.FindObjectsOfType<T>()` is highly inefficient for frequently executed code paths (e.g., AI sensor `Sense` methods or `Update` loops) because it performs an O(n) scene-wide traversal and allocates memory.
**Action:** Replace `FindObjectsOfType<T>()` with a centralized static registry pattern (e.g., `HashSet<T>`) managed via `OnEnable` and `OnDisable`. In Edit Mode tests, ensure the static registry is manually populated for dynamically added components without `[ExecuteAlways]`, and cleared in `[TearDown]` to prevent state leakage.
