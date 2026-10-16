## 2024-05-24 - Avoid duplicate Mathf.Sqrt in distance and normal calculations
**Learning:** Both `.magnitude` and `.normalized` trigger `Mathf.Sqrt`. Calling both sequentially is an anti-pattern.
**Action:** Use `.sqrMagnitude` for early exits. If required, calculate `Mathf.Sqrt` once, cache it, and manually divide the vector by the cached distance to normalize it.
## 2025-02-14 - Remove redundant normalizations after cross product of orthogonal normalized vectors
**Learning:** The cross product of two orthogonal, normalized vectors inherently results in a normalized vector. Calling `.normalized` on the result of `Vector3.Cross` in this scenario is an expensive and redundant `Mathf.Sqrt()` operation that should be avoided.
**Action:** Avoid calling `.normalized` on the result of a cross product if the inputs are already known to be normalized and orthogonal.

## 2026-10-16 - Avoid redundant Sqrt calls with transform.forward and pre-calculated magnitudes
**Learning:** Unity's `transform.forward` inherently returns a normalized vector, so calling `.normalized` on it adds an unnecessary `Mathf.Sqrt` calculation. Furthermore, when `magnitude` is already calculated, `vector.normalized` triggers an additional redundant square root.
**Action:** Remove `.normalized` from `transform.forward`, `transform.up`, and `transform.right`. Re-use existing `magnitude` values to compute normalized directions instead of calling `.normalized` directly.
