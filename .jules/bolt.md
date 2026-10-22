## 2024-05-24 - Avoid duplicate Mathf.Sqrt in distance and normal calculations
**Learning:** Both `.magnitude` and `.normalized` trigger `Mathf.Sqrt`. Calling both sequentially is an anti-pattern.
**Action:** Use `.sqrMagnitude` for early exits. If required, calculate `Mathf.Sqrt` once, cache it, and manually divide the vector by the cached distance to normalize it.
## 2025-02-14 - Remove redundant normalizations after cross product of orthogonal normalized vectors
**Learning:** The cross product of two orthogonal, normalized vectors inherently results in a normalized vector. Calling `.normalized` on the result of `Vector3.Cross` in this scenario is an expensive and redundant `Mathf.Sqrt()` operation that should be avoided.
**Action:** Avoid calling `.normalized` on the result of a cross product if the inputs are already known to be normalized and orthogonal.
## 2026-10-22 - Use centralized registries for frequent lookups in hot paths
**Learning:**  is extremely slow and allocates garbage, which is detrimental when used in frequently running AI sensor logic.
**Action:** Replace  with a static centralized registry (e.g., ) maintained in / to convert O(N) allocation-heavy searches into O(1) allocation-free lookups.

## 2026-10-22 - Optimize distance checks with sqrMagnitude
**Learning:**  calls , which is computationally expensive compared to a simple squared distance check.
**Action:** Replace  with  and compare it against the squared ranges (e.g., ) for better performance in hot paths.
## 2026-10-22 - Use centralized registries for frequent lookups in hot paths
**Learning:** `Object.FindObjectsOfType` is extremely slow and allocates garbage, which is detrimental when used in frequently running AI sensor logic.
**Action:** Replace `Object.FindObjectsOfType` with a static centralized registry (e.g., `HashSet<T>`) maintained in `OnEnable`/`OnDisable` to convert O(N) allocation-heavy searches into O(1) allocation-free lookups.

## 2026-10-22 - Optimize distance checks with sqrMagnitude
**Learning:** `Vector3.Distance` calls `Mathf.Sqrt`, which is computationally expensive compared to a simple squared distance check.
**Action:** Replace `Vector3.Distance(a, b)` with `(a - b).sqrMagnitude` and compare it against the squared ranges (e.g., `MinRange * MinRange`) for better performance in hot paths.
