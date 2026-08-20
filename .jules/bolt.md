## 2024-05-24 - Avoid duplicate Mathf.Sqrt in distance and normal calculations
**Learning:** Both `.magnitude` and `.normalized` trigger `Mathf.Sqrt`. Calling both sequentially is an anti-pattern.
**Action:** Use `.sqrMagnitude` for early exits. If required, calculate `Mathf.Sqrt` once, cache it, and manually divide the vector by the cached distance to normalize it.
## 2025-02-14 - Remove redundant normalizations after cross product of orthogonal normalized vectors
**Learning:** The cross product of two orthogonal, normalized vectors inherently results in a normalized vector. Calling `.normalized` on the result of `Vector3.Cross` in this scenario is an expensive and redundant `Mathf.Sqrt()` operation that should be avoided.
**Action:** Avoid calling `.normalized` on the result of a cross product if the inputs are already known to be normalized and orthogonal.
## 2025-02-14 - Cache Vector3.normalized property to avoid redundant Mathf.Sqrt
**Learning:** Calling `.normalized` on a `Vector3` multiple times in a function is an anti-pattern because it recalculates `Mathf.Sqrt` every time.
**Action:** Cache the result of `.normalized` into a local variable and reuse it. Do not attempt raw vector division by magnitude unless adding explicit safety checks for zero or near-zero magnitudes to prevent `NaN` propagation.
