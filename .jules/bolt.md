## 2024-05-24 - Avoid duplicate Mathf.Sqrt in distance and normal calculations
**Learning:** Both `.magnitude` and `.normalized` trigger `Mathf.Sqrt`. Calling both sequentially is an anti-pattern.
**Action:** Use `.sqrMagnitude` for early exits. If required, calculate `Mathf.Sqrt` once, cache it, and manually divide the vector by the cached distance to normalize it.
## 2025-02-14 - Remove redundant normalizations after cross product of orthogonal normalized vectors
**Learning:** The cross product of two orthogonal, normalized vectors inherently results in a normalized vector. Calling `.normalized` on the result of `Vector3.Cross` in this scenario is an expensive and redundant `Mathf.Sqrt()` operation that should be avoided.
**Action:** Avoid calling `.normalized` on the result of a cross product if the inputs are already known to be normalized and orthogonal.
## 2024-06-22 - Replacing FindObjectsOfType with OnEnable/OnDisable Registry
**Learning:** When creating static registries to avoid `FindObjectsOfType<T>` in Unity, always populate them using `OnEnable()` and `OnDisable()` rather than `Awake()`/`OnDestroy()` or NetworkSpawn/Despawn methods. This ensures the registry accurately reflects the active scene state, preventing bugs where disabled components are incorrectly processed.
**Action:** Always map static instance tracking directly to the Unity component lifecycle (`OnEnable` / `OnDisable`) when refactoring expensive scene lookups.
