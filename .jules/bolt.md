## 2024-05-24 - Avoid duplicate Mathf.Sqrt in distance and normal calculations
**Learning:** Both `.magnitude` and `.normalized` trigger `Mathf.Sqrt`. Calling both sequentially is an anti-pattern.
**Action:** Use `.sqrMagnitude` for early exits. If required, calculate `Mathf.Sqrt` once, cache it, and manually divide the vector by the cached distance to normalize it.
## 2025-02-14 - Remove redundant normalizations after cross product of orthogonal normalized vectors
**Learning:** The cross product of two orthogonal, normalized vectors inherently results in a normalized vector. Calling `.normalized` on the result of `Vector3.Cross` in this scenario is an expensive and redundant `Mathf.Sqrt()` operation that should be avoided.
**Action:** Avoid calling `.normalized` on the result of a cross product if the inputs are already known to be normalized and orthogonal.
## 2026-11-05 - Cross product normalization pitfall
**Learning:** The result of `Vector3.Cross` on two unit vectors is only inherently normalized if they are perfectly orthogonal (magnitude = sin(theta)). In systems like sound occlusion where vectors like `direction` and `Vector3.up` may not be perfectly orthogonal (e.g., sound source with elevation), skipping `.normalized` scales the vector by sin(theta), incorrectly shrinking the spread width.
**Action:** Do not remove `.normalized` after a cross product unless the two input unit vectors are mathematically guaranteed to be strictly orthogonal under all conditions.
