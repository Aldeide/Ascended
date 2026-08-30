## 2024-05-24 - Avoid duplicate Mathf.Sqrt in distance and normal calculations
**Learning:** Both `.magnitude` and `.normalized` trigger `Mathf.Sqrt`. Calling both sequentially is an anti-pattern.
**Action:** Use `.sqrMagnitude` for early exits. If required, calculate `Mathf.Sqrt` once, cache it, and manually divide the vector by the cached distance to normalize it.
## 2025-02-14 - Remove redundant normalizations after cross product of orthogonal normalized vectors
**Learning:** The cross product of two orthogonal, normalized vectors inherently results in a normalized vector. Calling `.normalized` on the result of `Vector3.Cross` in this scenario is an expensive and redundant `Mathf.Sqrt()` operation that should be avoided.
**Action:** Avoid calling `.normalized` on the result of a cross product if the inputs are already known to be normalized and orthogonal.
## 2025-03-01 - Avoid Object.FindObjectsOfType in hot paths by using a centralized HashSet registry populated in OnEnable/OnDisable
**Learning:** Calling FindObjectsOfType inside AI sensors' update hot paths is very slow and allocates memory heavily. Iterating a centralized HashSet is much faster, but requires null checks to avoid exceptions from destroyed game objects, and must be populated in normal Unity lifecycles (OnEnable/OnDisable) rather than network events to ensure non-networked instances are caught.
**Action:** Replace `FindObjectsOfType` with a centralized static registry (e.g., `ActiveInstances`) maintained during `OnEnable`/`OnDisable`. Ensure all iteration loops include `comp == null || comp.gameObject == null`.
