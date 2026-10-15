import sys

def main():
    filepath = "Assets/AISystem/Tests/Editor/AISystemTests.cs"
    with open(filepath, "r") as f:
        content = f.read()

    # The compilation error was that action.Perform(agentMock.Object) is passing an IActionReceiver to a method expecting IMonoAgent. But I am keeping Mock<IMonoAgent>, so agentMock.Object is IMonoAgent!
    # Wait, CS1503 is Argument 1: cannot convert from 'CrashKonijn.Agent.Core.IActionReceiver' to 'CrashKonijn.Agent.Core.IMonoAgent'.
    # If agentMock is Mock<IMonoAgent>, why does it say 'cannot convert from IActionReceiver'?
    pass
