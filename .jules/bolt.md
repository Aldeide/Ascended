## 2024-05-24 - Avoid duplicate Mathf.Sqrt in distance and normal calculations
**Learning:** Both `.magnitude` and `.normalized` trigger `Mathf.Sqrt`. Calling both sequentially is an anti-pattern.
**Action:** Use `.sqrMagnitude` for early exits. If required, calculate `Mathf.Sqrt` once, cache it, and manually divide the vector by the cached distance to normalize it.
## 2025-02-14 - Remove redundant normalizations after cross product of orthogonal normalized vectors
**Learning:** The cross product of two orthogonal, normalized vectors inherently results in a normalized vector. Calling `.normalized` on the result of `Vector3.Cross` in this scenario is an expensive and redundant `Mathf.Sqrt()` operation that should be avoided.
**Action:** Avoid calling `.normalized` on the result of a cross product if the inputs are already known to be normalized and orthogonal.
## 2024-08-03 - Cache AbilitySystemComponent instances and optimize distance calculations
**Learning:** Found an expensive `Object.FindObjectsOfType<AbilitySystemComponent>()` and multiple `Vector3.Distance` calls happening every time AI sensors trigger (`EnemyTargetSensor` and `TacticalPositionSensor`). This was being called constantly in the hot path.
**Action:** Created a centralized static registry (`HashSet<AbilitySystemComponent> ActiveInstances`) inside `AbilitySystemComponent` maintained in `OnEnable`/`OnDisable` lifecycle events. Replaced `Object.FindObjectsOfType` in AI sensors with a lookup against `AbilitySystemComponent.ActiveInstances`. Further reduced computation by replacing `Vector3.Distance` with `.sqrMagnitude` during distance sorting.
