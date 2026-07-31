## 2024-05-24 - Avoid duplicate Mathf.Sqrt in distance and normal calculations
**Learning:** Both `.magnitude` and `.normalized` trigger `Mathf.Sqrt`. Calling both sequentially is an anti-pattern.
**Action:** Use `.sqrMagnitude` for early exits. If required, calculate `Mathf.Sqrt` once, cache it, and manually divide the vector by the cached distance to normalize it.
## 2025-02-14 - Remove redundant normalizations after cross product of orthogonal normalized vectors
**Learning:** The cross product of two orthogonal, normalized vectors inherently results in a normalized vector. Calling `.normalized` on the result of `Vector3.Cross` in this scenario is an expensive and redundant `Mathf.Sqrt()` operation that should be avoided.
**Action:** Avoid calling `.normalized` on the result of a cross product if the inputs are already known to be normalized and orthogonal.
## 2025-02-14 - Replace Object.FindObjectsOfType with Centralized Static Registry
**Learning:** Using `Object.FindObjectsOfType` and `GameObject.FindGameObjectsWithTag` on hot paths like AI sensors scales extremely poorly. Maintaining a centralized static `HashSet` during standard Unity lifecycles (`OnEnable`/`OnDisable`) provides O(1) overhead and O(n) iteration without GC allocation.
**Action:** Use a centralized static registry (e.g. `ActiveInstances`) combined with `sqrMagnitude` distance checks for scene-wide lookups in hot paths instead of relying on Unity's expensive native lookup methods.
