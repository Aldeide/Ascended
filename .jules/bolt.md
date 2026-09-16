## 2024-05-24 - Avoid duplicate Mathf.Sqrt in distance and normal calculations
**Learning:** Both `.magnitude` and `.normalized` trigger `Mathf.Sqrt`. Calling both sequentially is an anti-pattern.
**Action:** Use `.sqrMagnitude` for early exits. If required, calculate `Mathf.Sqrt` once, cache it, and manually divide the vector by the cached distance to normalize it.
## 2025-02-14 - Remove redundant normalizations after cross product of orthogonal normalized vectors
**Learning:** The cross product of two orthogonal, normalized vectors inherently results in a normalized vector. Calling `.normalized` on the result of `Vector3.Cross` in this scenario is an expensive and redundant `Mathf.Sqrt()` operation that should be avoided.
**Action:** Avoid calling `.normalized` on the result of a cross product if the inputs are already known to be normalized and orthogonal.
## 2026-09-16 - Replace FindObjectsOfType with static registry
**Learning:** Using `FindObjectsOfType<T>` in hot paths (like AI sensors and decision makers) is extremely slow and causes unnecessary memory allocation. Unity Netcode for GameObjects (NGO) components should maintain a centralized static registry.
**Action:** Create a `static readonly HashSet<T> ActiveInstances` populated during `OnEnable`/`OnDisable` and iterate over it directly. Include null checks during iteration in case of lifecycle mismatches.
