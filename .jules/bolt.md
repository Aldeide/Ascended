## 2024-05-24 - Avoid duplicate Mathf.Sqrt in distance and normal calculations
**Learning:** Both `.magnitude` and `.normalized` trigger `Mathf.Sqrt`. Calling both sequentially is an anti-pattern.
**Action:** Use `.sqrMagnitude` for early exits. If required, calculate `Mathf.Sqrt` once, cache it, and manually divide the vector by the cached distance to normalize it.
## 2025-02-14 - Remove redundant normalizations after cross product of orthogonal normalized vectors
**Learning:** The cross product of two orthogonal, normalized vectors inherently results in a normalized vector. Calling `.normalized` on the result of `Vector3.Cross` in this scenario is an expensive and redundant `Mathf.Sqrt()` operation that should be avoided.
**Action:** Avoid calling `.normalized` on the result of a cross product if the inputs are already known to be normalized and orthogonal.
## 2024-03-08 - Replaced FindObjectsOfType with centralized HashSet
**Learning:** In Unity hot paths (like AI sensors running every frame or tick), `Object.FindObjectsOfType` and `GameObject.FindGameObjectsWithTag` cause significant CPU overhead and garbage collection allocations because they traverse the entire scene hierarchy.
**Action:** Replace these calls with a static `HashSet<T>` registry on the target component itself. Populate it in `OnEnable()` and clear it in `OnDisable()`. This is called the "Runtime Set" pattern and provides O(1) allocation-free access to all active instances.
