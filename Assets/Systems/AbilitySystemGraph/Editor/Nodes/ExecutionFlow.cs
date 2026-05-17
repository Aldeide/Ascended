namespace Systems.AbilitySystemGraph.Editor.Nodes
{
    /// <summary>
    /// A dummy type used strictly for validating connections between nodes.
    /// Execution ports will require this type, preventing Data ports (like float) 
    /// from connecting to the execution flow.
    /// </summary>
    public struct ExecutionFlow { }
}
