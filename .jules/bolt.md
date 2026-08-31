## 2024-05-24 - Avoid duplicate Mathf.Sqrt in distance and normal calculations
**Learning:** Both `.magnitude` and `.normalized` trigger `Mathf.Sqrt`. Calling both sequentially is an anti-pattern.
**Action:** Use `.sqrMagnitude` for early exits. If required, calculate `Mathf.Sqrt` once, cache it, and manually divide the vector by the cached distance to normalize it.
## 2025-02-14 - Remove redundant normalizations after cross product of orthogonal normalized vectors
**Learning:** The cross product of two orthogonal, normalized vectors inherently results in a normalized vector. Calling `.normalized` on the result of `Vector3.Cross` in this scenario is an expensive and redundant `Mathf.Sqrt()` operation that should be avoided.
**Action:** Avoid calling `.normalized` on the result of a cross product if the inputs are already known to be normalized and orthogonal.

## 2026-08-31 - Optimize AI Lookups
**Learning:** `Object.FindObjectsOfType<T>()` in hot paths (like sensors updating frequently) causes significant performance overhead and garbage collection, particularly when called by many agents. Iterating a dynamic static registry populated in standard lifecycle events (`OnEnable`/`OnDisable`) is vastly superior for performance in a Unity environment, although care must be taken in test teardowns to prevent cross-test state leakage.
**Action:** Replaced `Object.FindObjectsOfType<AbilitySystemComponent>()` with a static `HashSet` registry updated via `OnEnable()`/`OnDisable()` and `Clear()` on `[TearDown]` for Edit Mode tests to eliminate allocation and CPU overhead during periodic sensor checks.
