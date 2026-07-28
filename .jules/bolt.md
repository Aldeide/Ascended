## 2024-05-24 - Avoid duplicate Mathf.Sqrt in distance and normal calculations
**Learning:** Both `.magnitude` and `.normalized` trigger `Mathf.Sqrt`. Calling both sequentially is an anti-pattern.
**Action:** Use `.sqrMagnitude` for early exits. If required, calculate `Mathf.Sqrt` once, cache it, and manually divide the vector by the cached distance to normalize it.
## 2025-02-14 - Remove redundant normalizations after cross product of orthogonal normalized vectors
**Learning:** The cross product of two orthogonal, normalized vectors inherently results in a normalized vector. Calling `.normalized` on the result of `Vector3.Cross` in this scenario is an expensive and redundant `Mathf.Sqrt()` operation that should be avoided.
**Action:** Avoid calling `.normalized` on the result of a cross product if the inputs are already known to be normalized and orthogonal.
## 2025-02-14 - Replace FindObjectsOfType with a centralized active instance registry
**Learning:** `FindObjectsOfType<T>()` is very expensive when called frequently (e.g., inside AI `Sense` methods or `Update` loops), as it scans the entire scene hierarchy.
**Action:** Replace `FindObjectsOfType` in hot paths with a centralized static `HashSet<T>` populated during `OnEnable` and cleared during `OnDisable` of the target component to ensure fast O(1) tracking of active instances without heavy allocations.
