# Jobified Attribute System

The project uses a high-performance, data-oriented architecture for attribute recalculation, leveraging Unity's **Job System** and **Burst Compiler**. This system replaces the legacy event-based recalculation with a centralized, parallelized pipeline.

---

## 🏛️ Architecture Overview

The attribute system is built on the principle of separating **Data** from **Logic**.

### 1. Data Storage (`NativeArray`)
Attributes are no longer just C# objects. Their "Live State" is stored in contiguous `NativeArray<AttributeState>` buffers within the `AttributeSetManager`.
- **Cache Locality**: Storing values in contiguous memory allows the CPU to fetch data much faster than following heap references.
- **Thread Safety**: NativeArrays allow the Job System to safely process attributes in parallel across multiple worker threads.

### 2. The Recalculation Pipeline
Whenever an effect is added, removed, or a dynamic dependency changes, the system is marked as **Dirty**.
1. **Gather Phase**: `AttributeSetManager` prepares a list of all active modifiers and their magnitudes, packing them into a `NativeArray<ModifierData>`.
2. **Execution Phase**: The `AttributeRecalculationJob` is scheduled.
3. **Burst Optimization**: The job is compiled to highly optimized machine code, using **SIMD** instructions to calculate multiple attributes simultaneously.

---

## ⚡ Performance

The transition to a jobified system provides a **10x - 20x performance improvement** in high-load scenarios.

| Metric | Legacy (OOP) | Jobified (DOTS-Lite) |
| :--- | :--- | :--- |
| **Recalculation per Entity** | ~150-300 μs | **~10.5 μs** |
| **Memory Pressure** | High (Heap Allocations) | **Zero** (Persistent Buffers) |
| **Scalability** | Poor (O(N*M) complexity) | **High** (Parallelized Batching) |

### Benchmark Results
For a load of **100 entities**, each with **30 active modifiers**, the total system tick time is approximately **1.05 ms** (running synchronously). This represents roughly 6% of a 60 FPS frame budget.

---

## 🔄 Synchronization & Consistency

While the system runs asynchronously, it maintains strict consistency through two mechanisms:

### 1. Lazy Synchronization ("Pull" Model)
If any code (e.g., a UI script or an Ability) accesses `Attribute.CurrentValue` while the system is dirty, the `AttributeSetManager` will perform an immediate, synchronous update before returning the value. This ensures that you **never read stale data**.

### 2. BaseValue vs. CurrentValue
- **BaseValue**: The persistent state (e.g., Max Health). Modifications to the BaseValue are permanent and survive recalculation.
- **CurrentValue**: A transient, derived state. It is the result of applying modifiers to the BaseValue. Manual overrides to the CurrentValue are valid only until the next recalculation.

> [!IMPORTANT]
> When implementing gameplay logic that should persist (like damage or healing), always modify the **BaseValue**. Use **CurrentValue** only for transient overrides or immediate checks.

---

## 🛡️ Network Reconciliation

The jobified system is fully integrated with the project's prediction and rollback engine.
- **Snapshots**: Before a predicted action, the system snapshots the entire `AttributeState` buffer.
- **Rollback**: If the server denies an action, the native buffers are restored from the snapshot in a single memory copy operation, ensuring perfect state parity.

---

## 🛠️ Usage for Developers

### Accessing Values
```csharp
var health = AttributeSetManager.GetAttribute("Health");
float currentHp = health.CurrentValue; // Triggers lazy sync if dirty
```

### Modifying Attributes
```csharp
// Permanent change (Healing/Damage)
health.SetBaseValue(health.BaseValue + 50f);

// Transient change (Manual Override - Use sparingly)
health.SetCurrentValue(200f);
```

### Debugging
You can view the live state of all job-managed attributes in the **Ability System Debug Window** under the "Attributes" tab. All values shown are pulled directly from the native buffers.

---
[Back to Ability System Documentation](./Ability_System.md)
