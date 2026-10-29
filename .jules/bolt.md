## 2024-05-24 - Avoid duplicate Mathf.Sqrt in distance and normal calculations
**Learning:** Both `.magnitude` and `.normalized` trigger `Mathf.Sqrt`. Calling both sequentially is an anti-pattern.
**Action:** Use `.sqrMagnitude` for early exits. If required, calculate `Mathf.Sqrt` once, cache it, and manually divide the vector by the cached distance to normalize it.
## 2025-02-14 - Remove redundant normalizations after cross product of orthogonal normalized vectors
**Learning:** The cross product of two orthogonal, normalized vectors inherently results in a normalized vector. Calling `.normalized` on the result of `Vector3.Cross` in this scenario is an expensive and redundant `Mathf.Sqrt()` operation that should be avoided.
**Action:** Avoid calling `.normalized` on the result of a cross product if the inputs are already known to be normalized and orthogonal.

## 2026-10-29 - Optimize Scene-Wide Lookups in AI Sensors
**Learning:** Found AI sensors frequently using slow `Object.FindObjectsOfType<AbilitySystemComponent>()` during their `Sense` methods (which run often in hot paths). I need to adhere to the principle: to optimize scene-wide lookups, maintain a centralized static registry (`HashSet<AbilitySystemComponent> ActiveInstances`) populated during `OnEnable`/`OnDisable` even for Netcode NetworkBehaviours, as `OnNetworkSpawn`/`OnNetworkDespawn` creates a lifecycle mismatch.
**Action:** Replace `Object.FindObjectsOfType<AbilitySystemComponent>()` with a lookup in `AbilitySystemComponent.ActiveInstances`. Add `OnEnable` and `OnDisable` methods to `AbilitySystemComponent` to manage the registry.
