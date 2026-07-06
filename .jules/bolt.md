## 2024-05-24 - Avoid duplicate Mathf.Sqrt in distance and normal calculations
**Learning:** Both `.magnitude` and `.normalized` trigger `Mathf.Sqrt`. Calling both sequentially is an anti-pattern.
**Action:** Use `.sqrMagnitude` for early exits. If required, calculate `Mathf.Sqrt` once, cache it, and manually divide the vector by the cached distance to normalize it.
## 2025-02-14 - Remove redundant normalizations after cross product of orthogonal normalized vectors
**Learning:** The cross product of two orthogonal, normalized vectors inherently results in a normalized vector. Calling `.normalized` on the result of `Vector3.Cross` in this scenario is an expensive and redundant `Mathf.Sqrt()` operation that should be avoided.
**Action:** Avoid calling `.normalized` on the result of a cross product if the inputs are already known to be normalized and orthogonal.
## 2024-07-26 - [Unity Object Lookups in Sensors]
**Learning:** `GameObject.FindGameObjectsWithTag()` and `Object.FindObjectsOfType()` were heavily utilized inside AI sensors and decision makers which run on every AI tick/evaluation phase, causing huge garbage generation and traversing the entire scene hierarchy every frame. Standard lifecycle hooks sometimes miss `OnDisable` if the application quits, but maintaining a static `HashSet` with manual null checks `if (comp == null || comp.gameObject == null)` prevents missing reference exceptions.
**Action:** When creating new components that require frequent global lookups (like Player/Enemy components), always register them to a static `HashSet<T>` in `OnEnable`/`OnDisable` and iterate the HashSet during hot paths instead of using Unity's built-in scene traversal methods. Always add null and destroyed checks when iterating static collections in Unity.
