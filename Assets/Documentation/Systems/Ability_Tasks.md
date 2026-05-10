# Ability Tasks

Ability Tasks are an essential part of the Ascended Ability System, designed to handle asynchronous operations within abilities. They provide a clean, delegate-based way to wait for time to pass, listen for gameplay events, or wait for user input without blocking the main execution thread.

This system is heavily inspired by Unreal Engine's `UAbilityTask` pattern, completely replacing the legacy inspector-based `DurationalAbility` system with a robust, code-first approach.

## What is an Ability Task?

An Ability Task is an object that represents a long-running or delayed action during an ability's execution. Typical use cases include:
- Waiting for an animation to finish.
- Waiting for a specific duration (`WaitDelayTask`).
- Waiting for a specific Gameplay Event to be broadcast (`WaitGameplayEventTask`).
- Waiting for the player to select a target (`WaitTargetDataTask`).

## Using Ability Tasks in Abilities

To use an Ability Task inside an `Ability`, you instantiate it, bind delegates to its completion events, and call `ReadyForActivation()`.

```csharp
protected override void ActivateAbility(AbilityData data)
{
    // 1. Instantiate the task
    var delayTask = WaitDelayTask.CreateWaitDelay(this, 2.0f);
    
    // 2. Bind to its completion event
    delayTask.OnFinished += OnMyDelayFinished;
    
    // 3. Activate the task
    delayTask.ReadyForActivation();
}

private void OnMyDelayFinished()
{
    Debug.Log("The delay has finished! Continuing ability logic...");
    EndAbility(); // End the ability when the sequence is done
}
```

### Auto-Cleanup & Rollback Safety

The `Ability` base class automatically registers any task instantiated with `this` as its owner. You do **not** need to manually track or clean up tasks.
- If `EndAbility()` is called, all active tasks are forcefully ended and cleaned up.
- If `TryCancelAbility()` is called (e.g., due to client prediction rollback or an interruption), all active tasks are instantly terminated.
- Rollback Safety: Because cleanup is automatic, any predicted abilities on the client will properly abort their tasks if the server rejects the prediction.

## Creating Custom Ability Tasks

To create a new custom task, inherit from `AbilityTask`:

```csharp
using System;
using AbilitySystem.Runtime.AbilityTasks;
using AbilitySystem.Runtime.Abilities;

public class MyCustomTask : AbilityTask
{
    public event Action OnTaskCompleted;
    
    private float _timer;
    private float _duration;

    // Optional static factory method for convenience
    public static MyCustomTask CreateCustomTask(Ability owningAbility, float duration)
    {
        var task = new MyCustomTask();
        task.Initialize(owningAbility);
        task._duration = duration;
        return task;
    }

    protected override void Activate()
    {
        // Setup logic goes here.
    }

    public override void TickTask()
    {
        // Since TickTask is called automatically by the framework when Active:
        // Use OwnerSystem.GetTime() or Time.deltaTime for delta
        _timer += UnityEngine.Time.deltaTime; // Assuming typical delta approach
        if (_timer >= _duration)
        {
            // Always call your delegates before EndTask()
            OnTaskCompleted?.Invoke();
            EndTask();
        }
    }

    protected override void OnDestroy()
    {
        // Cleanup logic goes here.
        OnTaskCompleted = null;
    }
}
```

### Key Methods to Override
- `Activate()`: Called internally when you call `ReadyForActivation()`. This is where you should begin listening to events or routing ticks.
- `EndTask()`: Called when the task is finished naturally or when it is forcefully terminated by the parent Ability. **Always clean up your event subscriptions here.**

## Built-in Tasks

The standard library includes several built-in tasks:
- `WaitDelayTask`: Waits for a specified number of seconds before firing its completion event. Uses `AbilitySystem.GetTime()` for network-synchronized timekeeping.
- `WaitGameplayEventTask`: Subscribes to the EventManager to listen for a specific Gameplay Event. Supports "Only Match Exact" vs inheritance-based event matching.
- `WaitTargetDataTask`: Waits for the client to confirm target selection (e.g. AoE placement or unit selection) and fires a completion event with the target data. Works seamlessly across Client and Server for predicted abilities.
