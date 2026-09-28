## 2024-05-24 - Avoid duplicate Mathf.Sqrt in distance and normal calculations
**Learning:** Both `.magnitude` and `.normalized` trigger `Mathf.Sqrt`. Calling both sequentially is an anti-pattern.
**Action:** Use `.sqrMagnitude` for early exits. If required, calculate `Mathf.Sqrt` once, cache it, and manually divide the vector by the cached distance to normalize it.
## 2025-02-14 - Remove redundant normalizations after cross product of orthogonal normalized vectors
**Learning:** The cross product of two orthogonal, normalized vectors inherently results in a normalized vector. Calling `.normalized` on the result of `Vector3.Cross` in this scenario is an expensive and redundant `Mathf.Sqrt()` operation that should be avoided.
**Action:** Avoid calling `.normalized` on the result of a cross product if the inputs are already known to be normalized and orthogonal.
## 2026-09-28 - Remove redundant normalizations and cache vectors
**Learning:** `Transform.forward` (and `.up`, `.right`) in Unity are inherently normalized unit vectors. Calling `.normalized` on them triggers an unnecessary `Mathf.Sqrt()` operation. Additionally, accessing `.normalized` multiple times on the same vector without caching it duplicates the expensive square root operation.
**Action:** Remove `.normalized` from `Transform` direction properties. Cache the result of `.normalized` in a local variable if it needs to be used multiple times in the same code path.
