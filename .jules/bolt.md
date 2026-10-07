## 2024-05-24 - Avoid duplicate Mathf.Sqrt in distance and normal calculations
**Learning:** Both `.magnitude` and `.normalized` trigger `Mathf.Sqrt`. Calling both sequentially is an anti-pattern.
**Action:** Use `.sqrMagnitude` for early exits. If required, calculate `Mathf.Sqrt` once, cache it, and manually divide the vector by the cached distance to normalize it.
## 2025-02-14 - Remove redundant normalizations after cross product of orthogonal normalized vectors
**Learning:** The cross product of two orthogonal, normalized vectors inherently results in a normalized vector. Calling `.normalized` on the result of `Vector3.Cross` in this scenario is an expensive and redundant `Mathf.Sqrt()` operation that should be avoided.
**Action:** Avoid calling `.normalized` on the result of a cross product if the inputs are already known to be normalized and orthogonal.
## 2026-10-07 - Replace slow FindObjectsOfType with static registry
**Learning:** To optimize scene-wide lookups in hot paths (e.g., AI sensor updates), maintaining a centralized static registry (e.g., `HashSet<AbilitySystemComponent>`) populated during `OnEnable`/`OnDisable` is far more performant than using slow methods like `Object.FindObjectsOfType`.
**Action:** Always maintain a static registry for components that are frequently queried in hot paths instead of relying on `FindObjectsOfType`.
