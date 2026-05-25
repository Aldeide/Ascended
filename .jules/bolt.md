## 2024-05-24 - Avoid duplicate Mathf.Sqrt in distance and normal calculations
**Learning:** Both `.magnitude` and `.normalized` trigger `Mathf.Sqrt`. Calling both sequentially is an anti-pattern.
**Action:** Use `.sqrMagnitude` for early exits. If required, calculate `Mathf.Sqrt` once, cache it, and manually divide the vector by the cached distance to normalize it.
## 2024-05-24 - Avoid Vector3.Distance in hot AI Sensor loops
**Learning:** `Vector3.Distance` internally computes `Mathf.Sqrt()`, which can become a significant CPU bottleneck when iterating over many objects (like players or all AbilitySystemComponents) inside high-frequency loops like AI GOAP sensors (`Sense()` methods).
**Action:** Use `(a - b).sqrMagnitude` for relative distance comparisons. If the exact distance is required at the end, compute `Mathf.Sqrt()` exactly once on the final minimum squared value instead of recalculating it for every element in the loop.
