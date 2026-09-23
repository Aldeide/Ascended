## 2024-05-24 - Avoid duplicate Mathf.Sqrt in distance and normal calculations
**Learning:** Both `.magnitude` and `.normalized` trigger `Mathf.Sqrt`. Calling both sequentially is an anti-pattern.
**Action:** Use `.sqrMagnitude` for early exits. If required, calculate `Mathf.Sqrt` once, cache it, and manually divide the vector by the cached distance to normalize it.
## 2025-02-14 - Remove redundant normalizations after cross product of orthogonal normalized vectors
**Learning:** The cross product of two orthogonal, normalized vectors inherently results in a normalized vector. Calling `.normalized` on the result of `Vector3.Cross` in this scenario is an expensive and redundant `Mathf.Sqrt()` operation that should be avoided.
**Action:** Avoid calling `.normalized` on the result of a cross product if the inputs are already known to be normalized and orthogonal.
## 2026-09-23 - Replace FindObjectsOfType with centralized static registry
**Learning:** Using `FindObjectsOfType` repeatedly in hot paths like AI sensors is expensive. The framework's default behavior led to multiple full scene graph traversals on every tick/Sense phase. When dynamically adding components via `AddComponent` in EditMode tests, the `OnEnable` method is not invoked without `[ExecuteAlways]`, leading to failing tests if the static registry is not manually populated during test setup.
**Action:** Instead of finding objects, track them centrally. Add `public static readonly HashSet<T> ActiveInstances` to heavily queried components and register/unregister them in `OnEnable`/`OnDisable`. In EditMode tests, manually add dynamically created components to the static registry, and ensure it is cleared in `[TearDown]` to prevent state leakage.
