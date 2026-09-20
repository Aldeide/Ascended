## 2024-05-24 - Avoid duplicate Mathf.Sqrt in distance and normal calculations
**Learning:** Both `.magnitude` and `.normalized` trigger `Mathf.Sqrt`. Calling both sequentially is an anti-pattern.
**Action:** Use `.sqrMagnitude` for early exits. If required, calculate `Mathf.Sqrt` once, cache it, and manually divide the vector by the cached distance to normalize it.
## 2025-02-14 - Remove redundant normalizations after cross product of orthogonal normalized vectors
**Learning:** The cross product of two orthogonal, normalized vectors inherently results in a normalized vector. Calling `.normalized` on the result of `Vector3.Cross` in this scenario is an expensive and redundant `Mathf.Sqrt()` operation that should be avoided.
**Action:** Avoid calling `.normalized` on the result of a cross product if the inputs are already known to be normalized and orthogonal.
## 2026-09-20 - Replace FindObjectsOfType with ActiveInstances static registry
**Learning:** Object.FindObjectsOfType is extremely slow and allocates memory, making it a severe bottleneck in hot paths like Update or Sense. Dynamic networked objects like AbilitySystemComponent must populate a static HashSet (e.g., ActiveInstances) during OnEnable/OnDisable to enable O(1) collection access, avoiding scene-wide scans.
**Action:** Always maintain a centralized static registry for frequently accessed components and iterate it instead of using FindObjectsOfType.
