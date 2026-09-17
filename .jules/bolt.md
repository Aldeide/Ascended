## 2024-05-24 - Avoid duplicate Mathf.Sqrt in distance and normal calculations
**Learning:** Both `.magnitude` and `.normalized` trigger `Mathf.Sqrt`. Calling both sequentially is an anti-pattern.
**Action:** Use `.sqrMagnitude` for early exits. If required, calculate `Mathf.Sqrt` once, cache it, and manually divide the vector by the cached distance to normalize it.
## 2025-02-14 - Remove redundant normalizations after cross product of orthogonal normalized vectors
**Learning:** The cross product of two orthogonal, normalized vectors inherently results in a normalized vector. Calling `.normalized` on the result of `Vector3.Cross` in this scenario is an expensive and redundant `Mathf.Sqrt()` operation that should be avoided.
**Action:** Avoid calling `.normalized` on the result of a cross product if the inputs are already known to be normalized and orthogonal.
## 2025-02-20 - Replace slow FindObjectsOfType with HashSet registry in hot paths
**Learning:** `Object.FindObjectsOfType` is extremely slow when used frequently in hot paths like AI sensors (`Sense` methods) or `Update` loops. In Unity Netcode, `FindObjectsOfType` fetches all active objects regardless of their network spawn state, creating potential lifecycle mismatches if components manage their state via `OnNetworkSpawn`/`OnNetworkDespawn`.
**Action:** Maintain a centralized static registry (e.g., `HashSet<AbilitySystemComponent> ActiveInstances`) and iterate over it instead. Always populate this registry during the `OnEnable`/`OnDisable` lifecycle events to accurately reflect the active state of components, avoiding `FindObjectsOfType` completely for scene-wide lookups. Include null checks when iterating.
