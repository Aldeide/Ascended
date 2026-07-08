## 2024-05-24 - Avoid duplicate Mathf.Sqrt in distance and normal calculations
**Learning:** Both `.magnitude` and `.normalized` trigger `Mathf.Sqrt`. Calling both sequentially is an anti-pattern.
**Action:** Use `.sqrMagnitude` for early exits. If required, calculate `Mathf.Sqrt` once, cache it, and manually divide the vector by the cached distance to normalize it.
## 2025-02-14 - Remove redundant normalizations after cross product of orthogonal normalized vectors
**Learning:** The cross product of two orthogonal, normalized vectors inherently results in a normalized vector. Calling `.normalized` on the result of `Vector3.Cross` in this scenario is an expensive and redundant `Mathf.Sqrt()` operation that should be avoided.
**Action:** Avoid calling `.normalized` on the result of a cross product if the inputs are already known to be normalized and orthogonal.
## 2025-02-14 - Replace FindObjectsOfType with OnEnable HashSet caching in hot paths
**Learning:** `FindObjectsOfType<T>()` and `GameObject.FindGameObjectsWithTag()` are highly expensive `O(N)` scene traversal operations that allocate GC when called. Calling them in frequent update loops (like AI Sensors `Sense()` or Decision Makers `EvaluateGoal()`) creates major CPU and GC bottlenecks.
**Action:** For frequently queried components (like `AbilitySystemComponent`), maintain a `public static readonly HashSet<T> ActiveInstances = new();` populated via `OnEnable()` and cleared via `OnDisable()`. Hot path scripts should iterate over this cached collection instead of searching the scene.
