using AbilityGraph.Runtime.Nodes.Primitives;
using AbilitySystem.Test.Utilities;
using NUnit.Framework;

namespace AbilityGraph.Tests.Runtime.Nodes.Primitives
{
    /// <summary>
    /// Tests for primitive value nodes in the Ability Graph (floats, ints).
    /// </summary>
    public class PrimitivesNodesTests : AbilitySystemTestBase
    {
        /// <summary>
        /// Validates that FloatNode correctly passes its input value to its output.
        /// </summary>
        [Test]
        public void PrimitivesNodesTests_FloatNode_OutputMatchesInput()
        {
            var node = new FloatNode
            {
                Input = 10f
            };
            AbilityGraphTestUtilities.InvokeProcess(node);
            Assert.AreEqual(node.Input, node.Output);
        }
        
        /// <summary>
        /// Validates that IntNode correctly passes its input value to its output.
        /// </summary>
        [Test]
        public void PrimitivesNodesTests_IntNode_OutputMatchesInput()
        {
            var node = new IntNode
            {
                Input = 10
            };
            AbilityGraphTestUtilities.InvokeProcess(node);
            Assert.AreEqual(node.Input, node.Output);
        }
    }
}