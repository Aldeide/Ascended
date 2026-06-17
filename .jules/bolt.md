## 2024-05-24 - Avoid duplicate Mathf.Sqrt in distance and normal calculations
**Learning:** Both `.magnitude` and `.normalized` trigger `Mathf.Sqrt`. Calling both sequentially is an anti-pattern.
**Action:** Use `.sqrMagnitude` for early exits. If required, calculate `Mathf.Sqrt` once, cache it, and manually divide the vector by the cached distance to normalize it.
## 2025-02-14 - Remove redundant normalizations after cross product of orthogonal normalized vectors
**Learning:** The cross product of two orthogonal, normalized vectors inherently results in a normalized vector. Calling `.normalized` on the result of `Vector3.Cross` in this scenario is an expensive and redundant `Mathf.Sqrt()` operation that should be avoided.
**Action:** Avoid calling `.normalized` on the result of a cross product if the inputs are already known to be normalized and orthogonal.

## 2025-02-14 - Replace GameObject.FindGameObjectsWithTag in GOAP sensors with static registry
**Learning:** `GameObject.FindGameObjectsWithTag("Player")` and `Object.FindObjectsOfType<AbilitySystemComponent>()` are extremely expensive when called in GOAP sensors (`Sense()` methods), which run frequently and act as hot paths in the AI system.
**Action:** Use a centralized static registry (e.g., `AbilitySystemComponent.ActiveInstances`) populated during `OnEnable`/`OnDisable` combined with `comp.gameObject.CompareTag("Player")` for scene-wide lookups to eliminate per-frame GC pressure and overhead from Unity object searches.
