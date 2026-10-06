## 2024-05-24 - Avoid duplicate Mathf.Sqrt in distance and normal calculations
**Learning:** Both `.magnitude` and `.normalized` trigger `Mathf.Sqrt`. Calling both sequentially is an anti-pattern.
**Action:** Use `.sqrMagnitude` for early exits. If required, calculate `Mathf.Sqrt` once, cache it, and manually divide the vector by the cached distance to normalize it.
## 2025-02-14 - Remove redundant normalizations after cross product of orthogonal normalized vectors
**Learning:** The cross product of two orthogonal, normalized vectors inherently results in a normalized vector. Calling `.normalized` on the result of `Vector3.Cross` in this scenario is an expensive and redundant `Mathf.Sqrt()` operation that should be avoided.
**Action:** Avoid calling `.normalized` on the result of a cross product if the inputs are already known to be normalized and orthogonal.
## 2026-10-06 - Avoid redundant .normalized on Unity's directional vectors
**Learning:** Unity properties like `Transform.forward`, `Transform.up`, and `Transform.right` inherently return normalized directional unit vectors. Calling `.normalized` on them forces an unnecessary magnitude recalculation (square root and division) under the hood. Also, when magnitude is already calculated, manually normalize (with a zero-check > 1E-05f) rather than calling `.normalized` again.
**Action:** Avoid calling `.normalized` on `Transform.forward/up/right` vectors, as they are already normalized by definition. Cache manual normalization (with a zero-check > 1E-05f) if magnitude is already available.
