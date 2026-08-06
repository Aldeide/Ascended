## 2024-05-24 - Avoid duplicate Mathf.Sqrt in distance and normal calculations
**Learning:** Both `.magnitude` and `.normalized` trigger `Mathf.Sqrt`. Calling both sequentially is an anti-pattern.
**Action:** Use `.sqrMagnitude` for early exits. If required, calculate `Mathf.Sqrt` once, cache it, and manually divide the vector by the cached distance to normalize it.
## 2025-02-14 - Remove redundant normalizations after cross product of orthogonal normalized vectors
**Learning:** The cross product of two orthogonal, normalized vectors inherently results in a normalized vector. Calling `.normalized` on the result of `Vector3.Cross` in this scenario is an expensive and redundant `Mathf.Sqrt()` operation that should be avoided.
**Action:** Avoid calling `.normalized` on the result of a cross product if the inputs are already known to be normalized and orthogonal.
## 2024-08-06 - Redundant vector normalizations in AbilityTick
**Learning:** `AbilityTick()` is called frequently per frame. Calling `.normalized` inside it after having already calculated `.magnitude` causes multiple redundant `Mathf.Sqrt` calls per frame during movement checks (e.g., SphereCast wall checks).
**Action:** Cache the result of division `moveDelta / moveDist` as a local `moveDir` variable in `AbilityTick()` and reuse it across all physics casts and vector math within the frame.
