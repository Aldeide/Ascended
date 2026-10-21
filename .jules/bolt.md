## 2024-05-24 - Avoid duplicate Mathf.Sqrt in distance and normal calculations
**Learning:** Both `.magnitude` and `.normalized` trigger `Mathf.Sqrt`. Calling both sequentially is an anti-pattern.
**Action:** Use `.sqrMagnitude` for early exits. If required, calculate `Mathf.Sqrt` once, cache it, and manually divide the vector by the cached distance to normalize it.
## 2025-02-14 - Remove redundant normalizations after cross product of orthogonal normalized vectors
**Learning:** The cross product of two orthogonal, normalized vectors inherently results in a normalized vector. Calling `.normalized` on the result of `Vector3.Cross` in this scenario is an expensive and redundant `Mathf.Sqrt()` operation that should be avoided.
**Action:** Avoid calling `.normalized` on the result of a cross product if the inputs are already known to be normalized and orthogonal.
## 2026-10-21 - Replace FindObjectsOfType with centralized registry
**Learning:** Using `Object.FindObjectsOfType` in hot paths like AI sensors causes significant overhead and memory allocation. Maintaining a static centralized registry (`HashSet`) during `OnEnable`/`OnDisable` allows for O(1) lookups and highly efficient iteration, preventing large frame drops.
**Action:** Always use a static `HashSet` registry maintained via `OnEnable`/`OnDisable` to track instances for frequently queried component types across the scene, instead of using `FindObjectsOfType`. Ensure robust null checking when iterating through these registries.
