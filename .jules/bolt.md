## 2024-05-24 - Avoid duplicate Mathf.Sqrt in distance and normal calculations
**Learning:** Both `.magnitude` and `.normalized` trigger `Mathf.Sqrt`. Calling both sequentially is an anti-pattern.
**Action:** Use `.sqrMagnitude` for early exits. If required, calculate `Mathf.Sqrt` once, cache it, and manually divide the vector by the cached distance to normalize it.
## 2025-02-14 - Remove redundant normalizations after cross product of orthogonal normalized vectors
**Learning:** The cross product of two orthogonal, normalized vectors inherently results in a normalized vector. Calling `.normalized` on the result of `Vector3.Cross` in this scenario is an expensive and redundant `Mathf.Sqrt()` operation that should be avoided.
**Action:** Avoid calling `.normalized` on the result of a cross product if the inputs are already known to be normalized and orthogonal.
## 2025-02-14 - Remove redundant distance and direction calculation for offset vectors
**Learning:** If two points A and B are offset by the same vector C, the distance and direction between them `(B + C) - (A + C)` is identical to `B - A`. Calculating `Mathf.Sqrt` and `.normalized` for the offset vectors inside a loop is redundant if the distance and direction between A and B are already known.
**Action:** Reuse pre-calculated distance and direction variables instead of recalculating them for offset vectors inside loops.
