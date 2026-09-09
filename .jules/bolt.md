## 2024-05-24 - Avoid duplicate Mathf.Sqrt in distance and normal calculations
**Learning:** Both `.magnitude` and `.normalized` trigger `Mathf.Sqrt`. Calling both sequentially is an anti-pattern.
**Action:** Use `.sqrMagnitude` for early exits. If required, calculate `Mathf.Sqrt` once, cache it, and manually divide the vector by the cached distance to normalize it.
## 2025-02-14 - Remove redundant normalizations after cross product of orthogonal normalized vectors
**Learning:** The cross product of two orthogonal, normalized vectors inherently results in a normalized vector. Calling `.normalized` on the result of `Vector3.Cross` in this scenario is an expensive and redundant `Mathf.Sqrt()` operation that should be avoided.
**Action:** Avoid calling `.normalized` on the result of a cross product if the inputs are already known to be normalized and orthogonal.
## 2024-05-24 - AI Sensor Registry Optimization
**Learning:** Object.FindObjectsOfType is extremely slow and was being called frequently within AI sensors, creating a significant performance bottleneck. Unity's FindObjectsOfType should be avoided in hot paths.
**Action:** Implemented a centralized static registry (ActiveInstances) within AbilitySystemComponent that registers components on OnNetworkSpawn and unregisters on OnNetworkDespawn. Replaced FindObjectsOfType calls in sensors to iterate over this HashSet directly, drastically reducing overhead. Note: Added explicit teardown logic in Edit Mode tests to clear this static registry between runs.
