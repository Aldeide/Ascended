## 2024-05-24 - Avoid duplicate Mathf.Sqrt in distance and normal calculations
**Learning:** Both `.magnitude` and `.normalized` trigger `Mathf.Sqrt`. Calling both sequentially is an anti-pattern.
**Action:** Use `.sqrMagnitude` for early exits. If required, calculate `Mathf.Sqrt` once, cache it, and manually divide the vector by the cached distance to normalize it.
## 2025-02-14 - Remove redundant normalizations after cross product of orthogonal normalized vectors
**Learning:** The cross product of two orthogonal, normalized vectors inherently results in a normalized vector. Calling `.normalized` on the result of `Vector3.Cross` in this scenario is an expensive and redundant `Mathf.Sqrt()` operation that should be avoided.
**Action:** Avoid calling `.normalized` on the result of a cross product if the inputs are already known to be normalized and orthogonal.
## 2025-02-14 - Replace FindObjectsOfType and FindGameObjectsWithTag in hot loops with centralized registries
**Learning:** `Object.FindObjectsOfType` and `GameObject.FindGameObjectsWithTag` are slow (O(n) over the scene) and allocate garbage (new arrays per call). Calling them in hot paths (like AI sensors' Update or Sense methods) causes massive CPU and GC pressure in Unity.
**Action:** Always maintain a static registry (e.g., `static HashSet<T>`) populated via `OnEnable`/`OnDisable`. Replace scene-wide lookups in hot loops with iteration over this centralized registry, caching `sqrMagnitude` for distance checks to prevent `Mathf.Sqrt()` overhead.
