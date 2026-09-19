## 2024-05-24 - Avoid duplicate Mathf.Sqrt in distance and normal calculations
**Learning:** Both `.magnitude` and `.normalized` trigger `Mathf.Sqrt`. Calling both sequentially is an anti-pattern.
**Action:** Use `.sqrMagnitude` for early exits. If required, calculate `Mathf.Sqrt` once, cache it, and manually divide the vector by the cached distance to normalize it.
## 2025-02-14 - Remove redundant normalizations after cross product of orthogonal normalized vectors
**Learning:** The cross product of two orthogonal, normalized vectors inherently results in a normalized vector. Calling `.normalized` on the result of `Vector3.Cross` in this scenario is an expensive and redundant `Mathf.Sqrt()` operation that should be avoided.
**Action:** Avoid calling `.normalized` on the result of a cross product if the inputs are already known to be normalized and orthogonal.
## 2026-09-19 - Remove redundant .normalized calls on Transform direction properties
**Learning:** Properties like `Transform.forward` inherently return normalized unit vectors. Calling `.normalized` on them forces an unnecessary magnitude recalculation (Mathf.Sqrt). Additionally, calling `.normalized` repeatedly after already calculating `.magnitude` wastes CPU cycles.
**Action:** Remove `.normalized` from `Transform.forward`, `Transform.up`, etc., and reuse cached magnitude divisions (e.g., `vector / distance`) for normalizations to avoid redundant `Mathf.Sqrt` calls.
