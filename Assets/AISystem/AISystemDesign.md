# Enemy AI System Design Document
**Goal-Oriented Action Programming (GOAP), Gameplay Ability System (GAS) Integration, A* Pathfinding Pro, and Parallel Job Tactics**

This document describes the architecture, components, and implementation details for the rewritten Enemy AI System.

---

## 1. Architectural Overview

The AI system is built around four main layers:

1. **GOAP Decision Layer**: Utilizes the `com.crashkonijn.goap` package to run action planning. The planner runs asynchronously using the Unity Job System.
2. **GAS Integration Layer**: Links the GOAP world/target states to the Gameplay Ability System. Actions are mapped to Gameplay Abilities, while sensors read attributes, active effects, and gameplay tags.
3. **Tactical Cover & Flanking Layer**: An `IJobParallelFor` query system that scores tactical points (cover, vantage, flank) based on distance, exposure, and line of fire relative to target and group positions.
4. **Group Coordination Layer**: A centralized `TacticalGroupCoordinator` that distributes roles (Vanguard, Suppressor, Flanker, Support) and manages cover point reservations to ensure group coordination.

```
       +----------------------------+
       |  TacticalGroupCoordinator  | <------------------------+
       +----------------------------+                          |
                     | (Assigns Roles & Reserves Cover)        |
                     v                                         | (Queries / Claims)
       +----------------------------+                          |
       |  GoapActionProvider (GOAP) |                          |
       +----------------------------+                          |
          | (Plans Goals & Actions)                            |
          v                                                    |
       +----------------------------+                          |
       |     Active GoapAction      | -------------------------+
       +----------------------------+
          | (Move To / Execute)
          +-----------------------+
          |                       |
          v                       v
+------------------+     +-------------------------+
|     IAstarAI     |     | AbilitySystemComponent  |
|  (A* Pathfinding)|     |      (GAS Effects)      |
+------------------+     +-------------------------+
```

---

## 2. GOAP & GAS Integration (Easy to Author)

To minimize the boilerplate of creating distinct C# classes for every goal, action, sensor, target, and world key, the system provides standard, configurable components:

### 2.1 Configurable World & Target Keys
Rather than writing empty class files, we define a set of core keys in the namespace `AISystem.Runtime.WorldKeys` and `AISystem.Runtime.TargetKeys`:
* **World Keys**:
  * `IsIdle`: True if the agent has no other goals.
  * `TargetDead`: True if the agent's target is dead or invalid.
  * `HealthLow`: True if agent's health is below a threshold.
  * `HasCover`: True if agent is in cover from its threat.
  * `TargetInMeleeRange`: True if target is within melee range.
  * `TargetInRangedRange`: True if target is within ranged range.
  * `AllyNeedsHealing`: True if an ally requires healing.
  * `IsRoleVanguard`, `IsRoleFlanker`, `IsRoleSupport`: True if assigned these roles.
* **Target Keys**:
  * `ClosestEnemyTarget`: Sensed closest enemy.
  * `NeedyAllyTarget`: Sensed ally needing help.
  * `CoverPositionTarget`: Sensed cover point.
  * `FlankingPositionTarget`: Sensed flanking point.

### 2.2 Reusable Sensors
* **`AttributeSensor`**: Reads agent's `AbilitySystemComponent` attributes (Health, Shields) and updates `HealthLow`.
* **`TagSensor`**: Reads active gameplay tags on self and targets to check for status effects (e.g. stunned, frozen, suppressed).
* **`AbilitySensor`**: Checks cooldown and charge state of mapped abilities on the `AbilityManager`.
* **`TacticalPositionSensor`**: Triggers tactical jobs to find cover/flank locations and sets `CoverPositionTarget`/`FlankingPositionTarget`.

### 2.3 Reusable Actions
* **`GoapAbilityAction`**: Generic action that executes a Gameplay Ability by name.
  * Inputs: Target Key, Ability Name, Stopping Distance.
  * Behavior: If target is out of range, requests movement. If in range, triggers the ability via `AbilitySystemComponent.TryActivateAbility`.

---

## 3. Unity Job System: Tactical Cover & Flanking (Killzone Tactics)

To support procedural tactical behaviors without performance degradation, cover points are evaluated in parallel on worker threads.

### 3.1 Tactical Point Data
We define a lightweight struct for jobs:
```csharp
public struct TacticalPointData
{
    public Vector3 Position;
    public Vector3 Normal;
    public int CoverType; // 0 = None, 1 = Low, 2 = High
    public bool IsOccupied;
}
```

### 3.2 Evaluation Job
The `TacticalPointEvaluationJob` is an `IJobParallelFor` Burst-compiled job:
```csharp
public struct TacticalPointEvaluationJob : IJobParallelFor
{
    public NativeArray<TacticalPointData> Points;
    public Vector3 AgentPosition;
    public Vector3 TargetPosition;
    public Vector3 TargetForward;
    public float ProximityWeight;
    public float CoverWeight;
    public float FlankWeight;
    
    public NativeArray<float> Scores;

    public void Execute(int index)
    {
        var point = Points[index];
        if (point.IsOccupied)
        {
            Scores[index] = -9999f;
            return;
        }

        // 1. Proximity Score (closer to agent is better)
        float distToAgent = Vector3.Distance(point.Position, AgentPosition);
        float proxScore = 1f / (1f + distToAgent);

        // 2. Cover Score (Dot product of cover normal and target direction)
        Vector3 dirToTarget = (TargetPosition - point.Position).normalized;
        float coverDot = Vector3.Dot(point.Normal, dirToTarget);
        // If cover normal points AWAY from the target, it blocks the line of sight (good cover)
        float coverScore = coverDot < -0.2f ? -coverDot : 0f;

        // 3. Flanking Score (Is the point to the side/rear of the target?)
        float flankDot = Vector3.Dot(TargetForward, -dirToTarget);
        // If flankDot is around 0 or negative, we are to the side or behind the target
        float flankScore = 1f - Mathf.Abs(flankDot);

        Scores[index] = (proxScore * ProximityWeight) + (coverScore * CoverWeight) + (flankScore * FlankWeight);
    }
}
```

---

## 4. Group Coordination Layer

A centralized `TacticalGroupCoordinator` enables group behavior:

1. **Role Allocation**: Dynamically balances the group. If there are 3 active enemies:
   * 1 is assigned **Vanguard** (rushes target).
   * 1 is assigned **Flanker** (queries flanking positions).
   * 1 is assigned **Support** (uses suppressing/ranged fire and healing abilities).
2. **Cover Reservations**: When an agent selects a cover position, it notifies the coordinator. The coordinator reserves the point, marking it occupied so other agents skip it.
3. **Target Aggregation**: Spotted targets are broadcasted to the coordinator, alerting nearby group members who might not have visual contact.

---

## 5. Navigation Integration: A* Pathfinding Pro

We integrate Aron Granberg's A* Pathfinding Pro to manage agent movement:
* The `AgentMoveBehaviour` caches the `IAstarAI` interface (which handles `RichAI`, `AIPath`, or `AILerp` components).
* When moving, the agent sets `ai.destination` and calls `ai.isStopped = false`.
* On arrival or cancellation, it sets `ai.isStopped = true`.
* Fallback to direct translation remains active for non-pathfinding test scenes.

---

## 6. How to Author an Enemy

To create an enemy AI:
1. Attach `GoapActionProvider`, `EnemyDecisionMaker`, `AgentMoveBehaviour`, and an A* pathfinding component (e.g. `RichAI`).
2. Attach the `AbilitySystemComponent` containing the enemy's abilities, attributes, and tags.
3. In the Unity Editor, configure the GOAP Goals and Actions on the `EnemyDecisionMaker` using our capability builder presets. No new C# code is needed for standard melee, ranged, healer, or flanking AI variants.
