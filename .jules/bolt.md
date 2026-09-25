## 2024-05-24 - Avoid duplicate Mathf.Sqrt in distance and normal calculations
**Learning:** Both `.magnitude` and `.normalized` trigger `Mathf.Sqrt`. Calling both sequentially is an anti-pattern.
**Action:** Use `.sqrMagnitude` for early exits. If required, calculate `Mathf.Sqrt` once, cache it, and manually divide the vector by the cached distance to normalize it.
## 2025-02-14 - Remove redundant normalizations after cross product of orthogonal normalized vectors
**Learning:** The cross product of two orthogonal, normalized vectors inherently results in a normalized vector. Calling `.normalized` on the result of `Vector3.Cross` in this scenario is an expensive and redundant `Mathf.Sqrt()` operation that should be avoided.
**Action:** Avoid calling `.normalized` on the result of a cross product if the inputs are already known to be normalized and orthogonal.
## 2026-09-25 - Use Static Registry to Optimize FindObjectsOfType
**Learning:** Scene-wide lookups like `Object.FindObjectsOfType` in hot paths (e.g. AI sensors) create massive performance bottlenecks. Furthermore, using dynamically assigned static registries (like `HashSet`) during `OnEnable`/`OnDisable` is crucial for efficiency, but tests with `AddComponent` do not invoke lifecycle methods and require manual assignment/teardown.
**Action:** Replace `FindObjectsOfType` calls with centralized static `HashSet` registries maintained via `OnEnable` and `OnDisable`, and iterate them directly without extra `ToList()` overhead while taking care of null checking.
