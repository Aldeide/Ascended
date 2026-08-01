## 2024-05-24 - Avoid duplicate Mathf.Sqrt in distance and normal calculations
**Learning:** Both `.magnitude` and `.normalized` trigger `Mathf.Sqrt`. Calling both sequentially is an anti-pattern.
**Action:** Use `.sqrMagnitude` for early exits. If required, calculate `Mathf.Sqrt` once, cache it, and manually divide the vector by the cached distance to normalize it.
## 2025-02-14 - Remove redundant normalizations after cross product of orthogonal normalized vectors
**Learning:** The cross product of two orthogonal, normalized vectors inherently results in a normalized vector. Calling `.normalized` on the result of `Vector3.Cross` in this scenario is an expensive and redundant `Mathf.Sqrt()` operation that should be avoided.
**Action:** Avoid calling `.normalized` on the result of a cross product if the inputs are already known to be normalized and orthogonal.
## 2024-08-01 - Avoid Object.FindObjectsOfType and GameObject.FindGameObjectsWithTag in hot paths
**Learning:** Frequent calls to `GameObject.FindGameObjectsWithTag` and `Object.FindObjectsOfType` during frequent sensory loops (like `Update` or `Sense`) allocate garbage and cause performance degradation. In AI targeting logic, calculating distance with `Vector3.Distance` repeatedly in loops also incurs expensive `Mathf.Sqrt` overhead.
**Action:** Instead of `FindObjectsOfType` or `FindGameObjectsWithTag`, utilize centralized static registries populated during Unity lifecycle hooks, such as `AbilitySystemComponent.ActiveInstances`, combined with `gameObject.CompareTag`. Substitute `Vector3.Distance` with `.sqrMagnitude` for distance comparisons within loops.
