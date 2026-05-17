using Unity.GraphToolkit.Editor;
using Systems.AbilitySystemGraph.VM;

namespace Systems.AbilitySystemGraph.Editor.Nodes
{
    /// <summary>
    /// Base class for all nodes in the Ability Graph.
    /// In GTK, this represents the serializable data model of the node.
    /// </summary>
    public abstract class AbilityNode : Node
    {
        /// <summary>
        /// Defines the VM operation this node performs.
        /// Can be overridden for logic/math nodes to return their respective OpCode.
        /// </summary>
        public virtual OpCode GetOpCode()
        {
            return OpCode.Nop;
        }
    }
}
