## 2024-05-24 - Avoid duplicate Mathf.Sqrt in distance and normal calculations
**Learning:** Both `.magnitude` and `.normalized` trigger `Mathf.Sqrt`. Calling both sequentially is an anti-pattern.
**Action:** Use `.sqrMagnitude` for early exits. If required, calculate `Mathf.Sqrt` once, cache it, and manually divide the vector by the cached distance to normalize it.
## 2025-02-14 - Remove redundant normalizations after cross product of orthogonal normalized vectors
**Learning:** The cross product of two orthogonal, normalized vectors inherently results in a normalized vector. Calling `.normalized` on the result of `Vector3.Cross` in this scenario is an expensive and redundant `Mathf.Sqrt()` operation that should be avoided.
**Action:** Avoid calling `.normalized` on the result of a cross product if the inputs are already known to be normalized and orthogonal.
## 2024-05-18 - [Centralized Object Registry for Hot Paths]
**Learning:** Frequent calls to `Object.FindObjectsOfType` and `GameObject.FindGameObjectsWithTag` inside GOAP sensors (`Sense()`) are extremely expensive and allocate garbage. Iterating over all active instances directly prevents CPU spikes in game loops.
**Action:** When searching for global entities like Players or Enemies in hot paths, avoid Unity's hierarchy search functions. Instead, implement a `public static readonly HashSet<T> ActiveInstances` populated during `OnEnable` and `OnDisable`, and iterate over this cached collection.
