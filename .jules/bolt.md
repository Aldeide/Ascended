## 2024-05-24 - Avoid duplicate Mathf.Sqrt in distance and normal calculations
**Learning:** Both `.magnitude` and `.normalized` trigger `Mathf.Sqrt`. Calling both sequentially is an anti-pattern.
**Action:** Use `.sqrMagnitude` for early exits. If required, calculate `Mathf.Sqrt` once, cache it, and manually divide the vector by the cached distance to normalize it.
## 2025-02-14 - Remove redundant normalizations after cross product of orthogonal normalized vectors
**Learning:** The cross product of two orthogonal, normalized vectors inherently results in a normalized vector. Calling `.normalized` on the result of `Vector3.Cross` in this scenario is an expensive and redundant `Mathf.Sqrt()` operation that should be avoided.
**Action:** Avoid calling `.normalized` on the result of a cross product if the inputs are already known to be normalized and orthogonal.
## 2025-02-14 - LINQ allocations in Unity hot paths defeat caching optimizations
**Learning:** Replacing `FindGameObjectsWithTag("Player")` with LINQ queries like `.Where(x => x.CompareTag("Player")).ToArray()` over a cached registry introduces per-frame GC allocations, neutralizing the performance benefits of caching. LINQ should not be used in Unity hot paths (Update loops, AI sensors).
**Action:** When replacing native lookups with cached registries, use standard loops and pre-allocated instance collections instead of LINQ, or stick to native `FindGameObjectsWithTag` if pre-allocation is not feasible.
