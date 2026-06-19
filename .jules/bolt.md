## 2024-05-24 - Avoid duplicate Mathf.Sqrt in distance and normal calculations
**Learning:** Both `.magnitude` and `.normalized` trigger `Mathf.Sqrt`. Calling both sequentially is an anti-pattern.
**Action:** Use `.sqrMagnitude` for early exits. If required, calculate `Mathf.Sqrt` once, cache it, and manually divide the vector by the cached distance to normalize it.
## 2025-02-14 - Remove redundant normalizations after cross product of orthogonal normalized vectors
**Learning:** The cross product of two orthogonal, normalized vectors inherently results in a normalized vector. Calling `.normalized` on the result of `Vector3.Cross` in this scenario is an expensive and redundant `Mathf.Sqrt()` operation that should be avoided.
**Action:** Avoid calling `.normalized` on the result of a cross product if the inputs are already known to be normalized and orthogonal.
## 2024-05-24 - Centralized Registry for Global Objects
**Learning:** Calling `GameObject.FindGameObjectsWithTag("Player")` and `Object.FindObjectsOfType<AbilitySystemComponent>()` every time AI sensors run (which is in the hot path of AI logic updates) is highly unoptimized and puts huge pressure on the Unity garbage collector because of allocations for returning arrays.
**Action:** Replace scene-wide `GameObject` or `Object` lookups with a static registry `HashSet` maintained inside the `OnEnable` and `OnDisable` methods of a core class like `AbilitySystemComponent`.
