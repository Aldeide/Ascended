## 2024-05-24 - Avoid duplicate Mathf.Sqrt in distance and normal calculations
**Learning:** Both `.magnitude` and `.normalized` trigger `Mathf.Sqrt`. Calling both sequentially is an anti-pattern.
**Action:** Use `.sqrMagnitude` for early exits. If required, calculate `Mathf.Sqrt` once, cache it, and manually divide the vector by the cached distance to normalize it.
## 2025-02-14 - Remove redundant normalizations after cross product of orthogonal normalized vectors
**Learning:** The cross product of two orthogonal, normalized vectors inherently results in a normalized vector. Calling `.normalized` on the result of `Vector3.Cross` in this scenario is an expensive and redundant `Mathf.Sqrt()` operation that should be avoided.
**Action:** Avoid calling `.normalized` on the result of a cross product if the inputs are already known to be normalized and orthogonal.
## 2024-08-27 - Centralize AI System Components Tracking
**Learning:** Found several hot paths and sensors in the codebase utilizing the expensive `Object.FindObjectsOfType<AbilitySystemComponent>()` method for scene-wide lookups, slowing down AI decision-making loops.
**Action:** Implemented a centralized static registry (`HashSet<AbilitySystemComponent> ActiveInstances`) inside `AbilitySystemComponent.cs`, populated during standard Unity lifecycle hooks (`OnEnable`/`OnDisable`). Updated sensors and `EnemyDecisionMaker` to iterate the `ActiveInstances` collection directly for O(N) lookup. Mock instances created during tests must be added manually and cleaned up in `[TearDown]` to avoid test state bleeding.
