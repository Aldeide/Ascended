## 2024-05-24 - Avoid duplicate Mathf.Sqrt in distance and normal calculations
**Learning:** Both `.magnitude` and `.normalized` trigger `Mathf.Sqrt`. Calling both sequentially is an anti-pattern.
**Action:** Use `.sqrMagnitude` for early exits. If required, calculate `Mathf.Sqrt` once, cache it, and manually divide the vector by the cached distance to normalize it.
## 2025-02-14 - Remove redundant normalizations after cross product of orthogonal normalized vectors
**Learning:** The cross product of two orthogonal, normalized vectors inherently results in a normalized vector. Calling `.normalized` on the result of `Vector3.Cross` in this scenario is an expensive and redundant `Mathf.Sqrt()` operation that should be avoided.
**Action:** Avoid calling `.normalized` on the result of a cross product if the inputs are already known to be normalized and orthogonal.
## 2025-02-14 - Replace FindObjectsOfType with centralized HashSet registries
**Learning:** `Object.FindObjectsOfType` and `GameObject.FindGameObjectsWithTag` are extremely slow and generate GC allocations. When called frequently from sensors or decision makers in the AI system, they cause severe performance bottlenecks.
**Action:** Replace scene-wide lookups with a centralized, statically accessible `HashSet<T>` on the target component (e.g., `AbilitySystemComponent.ActiveInstances`). Populate it during `OnEnable` and clear it during `OnDisable` to ensure deterministic lifecycle tracking without race conditions.
