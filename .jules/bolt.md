## 2024-05-24 - Avoid duplicate Mathf.Sqrt in distance and normal calculations
**Learning:** Both `.magnitude` and `.normalized` trigger `Mathf.Sqrt`. Calling both sequentially is an anti-pattern.
**Action:** Use `.sqrMagnitude` for early exits. If required, calculate `Mathf.Sqrt` once, cache it, and manually divide the vector by the cached distance to normalize it.
## 2025-02-14 - Remove redundant normalizations after cross product of orthogonal normalized vectors
**Learning:** The cross product of two orthogonal, normalized vectors inherently results in a normalized vector. Calling `.normalized` on the result of `Vector3.Cross` in this scenario is an expensive and redundant `Mathf.Sqrt()` operation that should be avoided.
**Action:** Avoid calling `.normalized` on the result of a cross product if the inputs are already known to be normalized and orthogonal.
## 2024-08-07 - Avoid Object.FindObjectsOfType in Unity Update/Hot Paths
**Learning:** Found that AI Sensors (`EnemyTargetSensor`, `TacticalPositionSensor`) were repeatedly calling `Object.FindObjectsOfType<AbilitySystemComponent>()` and calculating `Vector3.Distance()` in `Update`-like methods (`Sense()`), causing massive CPU overhead and GC allocations.
**Action:** Replaced `Object.FindObjectsOfType` with a static centralized registry (`HashSet<AbilitySystemComponent> ActiveInstances`) managed in `OnEnable`/`OnDisable`. Avoided expensive `.magnitude` calculations by using `.sqrMagnitude` for distance comparisons.
