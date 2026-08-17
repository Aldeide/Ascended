## 2024-05-24 - AI Sensor Optimization
**Learning:** Avoid `Object.FindObjectsOfType` in hot paths like `Sense` inside AI sensors. Maintain a centralized static registry of active instances to optimize lookups. Also use `.sqrMagnitude` instead of `Vector3.Distance` to save redundant `Mathf.Sqrt` calls.
**Action:** When creating new components that need frequent lookups by AI sensors, implement a static `HashSet` registry that is updated in standard lifecycle methods like `OnEnable` and `OnDisable`.

## 2024-05-24 - AI Sensor Optimization
**Learning:** Avoid `Object.FindObjectsOfType` in hot paths like `Sense` inside AI sensors. Maintain a centralized static registry of active instances to optimize lookups. Also use `.sqrMagnitude` instead of `Vector3.Distance` to save redundant `Mathf.Sqrt` calls.
**Action:** When creating new components that need frequent lookups by AI sensors, implement a static `HashSet` registry that is updated in standard lifecycle methods like `OnEnable` and `OnDisable`.
