## 2024-05-24 - Avoid duplicate Mathf.Sqrt in distance and normal calculations
**Learning:** Both `.magnitude` and `.normalized` trigger `Mathf.Sqrt`. Calling both sequentially is an anti-pattern.
**Action:** Use `.sqrMagnitude` for early exits. If required, calculate `Mathf.Sqrt` once, cache it, and manually divide the vector by the cached distance to normalize it.
## 2025-02-14 - Remove redundant normalizations after cross product of orthogonal normalized vectors
**Learning:** The cross product of two orthogonal, normalized vectors inherently results in a normalized vector. Calling `.normalized` on the result of `Vector3.Cross` in this scenario is an expensive and redundant `Mathf.Sqrt()` operation that should be avoided.
**Action:** Avoid calling `.normalized` on the result of a cross product if the inputs are already known to be normalized and orthogonal.
## 2026-10-09 - Avoid redundant Mathf.Sqrt by utilizing Unity's inherent normalization and caching
**Learning:** Unity properties like `Transform.forward` are already normalized, so calling `.normalized` on them incurs unnecessary `Mathf.Sqrt` overhead. Furthermore, when `.normalized` is needed multiple times for the same vector, calling it repeatedly recalculates the magnitude each time.
**Action:** Omit `.normalized` on inherently normalized vectors. Cache the result of `.normalized` in a local variable when it's needed multiple times for the same vector to avoid redundant `Mathf.Sqrt` calls.
