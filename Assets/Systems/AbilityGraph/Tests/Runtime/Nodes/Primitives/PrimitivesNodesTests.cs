using AbilityGraph.Runtime.Nodes.Primitives;
using NUnit.Framework;

namespace AbilityGraph.Tests.Runtime.Nodes.Primitives
{
    public class PrimitivesNodesTests
    {
        [Test]
        public void PrimitiveNodeTest_FloatNode_OutputIsEqualToInput()
        {
            var node = new FloatNode
            {
                Input = 10
            };
            node.OnProcess();
            Assert.AreEqual(node.Input, node.Output);
        }
        
        [Test]
        public void PrimitiveNodeTest_IntNode_OutputIsEqualToInput()
        {
            var node = new IntNode
            {
                Input = 10
            };
            node.OnProcess();
            Assert.AreEqual(node.Input, node.Output);
        }
    }
}