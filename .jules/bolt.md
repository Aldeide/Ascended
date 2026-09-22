## 2024-05-24 - Avoid duplicate Mathf.Sqrt in distance and normal calculations
**Learning:** Both `.magnitude` and `.normalized` trigger `Mathf.Sqrt`. Calling both sequentially is an anti-pattern.
**Action:** Use `.sqrMagnitude` for early exits. If required, calculate `Mathf.Sqrt` once, cache it, and manually divide the vector by the cached distance to normalize it.
## 2025-02-14 - Remove redundant normalizations after cross product of orthogonal normalized vectors
**Learning:** The cross product of two orthogonal, normalized vectors inherently results in a normalized vector. Calling `.normalized` on the result of `Vector3.Cross` in this scenario is an expensive and redundant `Mathf.Sqrt()` operation that should be avoided.
**Action:** Avoid calling `.normalized` on the result of a cross product if the inputs are already known to be normalized and orthogonal.
## 2024-05-24 - AI Sensor Lookups Optimization
**Learning:** Calling `Object.FindObjectsOfType<AbilitySystemComponent>()` continuously inside AI sensor loops and Update logic causes significant frame drops due to full scene traversal and heap allocations. Unity Netcode components still need proper initialization and teardown of such registries on `OnEnable`/`OnDisable` manually, not on network spawn events, because AI could search for inactive objects.
**Action:** Created a static centralized registry (`AbilitySystemComponent.ActiveInstances`) managed via `OnEnable`/`OnDisable` and updated AI sensors to iterate over this registry directly instead of using `FindObjectsOfType`. Added tests teardown step to clear the static list (`AbilitySystemComponent.ActiveInstances.Clear()`).
