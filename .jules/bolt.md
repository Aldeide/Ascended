## 2024-05-24 - Avoid duplicate Mathf.Sqrt in distance and normal calculations
**Learning:** Both `.magnitude` and `.normalized` trigger `Mathf.Sqrt`. Calling both sequentially is an anti-pattern.
**Action:** Use `.sqrMagnitude` for early exits. If required, calculate `Mathf.Sqrt` once, cache it, and manually divide the vector by the cached distance to normalize it.
## 2025-02-14 - Remove redundant normalizations after cross product of orthogonal normalized vectors
**Learning:** The cross product of two orthogonal, normalized vectors inherently results in a normalized vector. Calling `.normalized` on the result of `Vector3.Cross` in this scenario is an expensive and redundant `Mathf.Sqrt()` operation that should be avoided.
**Action:** Avoid calling `.normalized` on the result of a cross product if the inputs are already known to be normalized and orthogonal.

## 2024-05-14 - Optimize hotpath target lookup in EnemyTargetSensor
**Learning:** `EnemyTargetSensor` frequently called `Object.FindObjectsOfType<AbilitySystemComponent>()` as a fallback, which scans the entire scene hierarchy every time. It also used `Vector3.Distance`, which involves a slow `Mathf.Sqrt`.
**Action:** Introduced a static `HashSet<AbilitySystemComponent> ActiveInstances` maintained via `OnEnable`/`OnDisable` in `AbilitySystemComponent`, completely avoiding scene-wide searches. Replaced `Vector3.Distance` with `.sqrMagnitude` across the sensor code for faster distance comparisons.
