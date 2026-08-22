## 2024-05-24 - Avoid duplicate Mathf.Sqrt in distance and normal calculations
**Learning:** Both `.magnitude` and `.normalized` trigger `Mathf.Sqrt`. Calling both sequentially is an anti-pattern.
**Action:** Use `.sqrMagnitude` for early exits. If required, calculate `Mathf.Sqrt` once, cache it, and manually divide the vector by the cached distance to normalize it.
## 2025-02-14 - Remove redundant normalizations after cross product of orthogonal normalized vectors
**Learning:** The cross product of two orthogonal, normalized vectors inherently results in a normalized vector. Calling `.normalized` on the result of `Vector3.Cross` in this scenario is an expensive and redundant `Mathf.Sqrt()` operation that should be avoided.
**Action:** Avoid calling `.normalized` on the result of a cross product if the inputs are already known to be normalized and orthogonal.
## 2025-02-14 - Replace Expensive Scene Queries with Centralized Static Registries
**Learning:** `Object.FindObjectsOfType` and `GameObject.FindGameObjectsWithTag` are extremely expensive in Unity and can cause major per-frame GC pressure if executed repeatedly (e.g., inside AI update/sense loops).
**Action:** Use a centralized static registry (e.g., a static `HashSet<T> ActiveInstances` property on the component class, populated during `OnEnable` and `OnDisable`) to allow O(1) tracking of active instances across the scene without performing costly global queries. If you need to filter the registry by tags, iterate over `ActiveInstances` and use `comp.gameObject.CompareTag()` instead.
## 2025-02-14 - Test Environment Setup for Centralized Registries
**Learning:** In Unity EditMode tests, component lifecycle methods like `OnEnable()` may not execute automatically when components are dynamically instantiated via `AddComponent<T>()`. This leads to `NullReferenceException` or assertion failures when testing systems that rely on static registries populated during `OnEnable()`.
**Action:** Always manually invoke `OnEnable()` (e.g., via reflection `typeof(AbilitySystemComponent).GetMethod("OnEnable", ...).Invoke(asc, null)`) during test setup when mocking components that manage static state. Explicitly clear the registry and call `OnDisable()` during `[TearDown]` to prevent state pollution across test runs.
