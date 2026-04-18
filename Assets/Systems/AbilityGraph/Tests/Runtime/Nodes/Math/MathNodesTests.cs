using AbilityGraph.Runtime.Nodes.Math;
using NUnit.Framework;
using UnityEngine;

namespace AbilityGraph.Tests.Runtime.Nodes.Math
{
    public class MathNodesTests
    {
        [Test]
        public void FloatArithmeticNode_OperationsWork()
        {
            var node = new FloatArithmeticNode();
            node.A = 10f;
            node.B = 5f;

            node.Operation = FloatArithmeticType.Add;
            node.OnProcess();
            Assert.AreEqual(15f, node.Result);

            node.Operation = FloatArithmeticType.Multiply;
            node.OnProcess();
            Assert.AreEqual(50f, node.Result);

            node.Operation = FloatArithmeticType.Divide;
            node.OnProcess();
            Assert.AreEqual(2f, node.Result);
            
            node.Operation = FloatArithmeticType.Power;
            node.OnProcess();
            Assert.AreEqual(100000f, node.Result);
        }

        [Test]
        public void ComparisonNode_ComparisonsWork()
        {
            var node = new ComparisonNode();
            node.A = 10f;
            node.B = 5f;

            node.Comparison = ComparisonType.Greater;
            node.OnProcess();
            Assert.IsTrue(node.Result);

            node.Comparison = ComparisonType.Less;
            node.OnProcess();
            Assert.IsFalse(node.Result);
        }

        [Test]
        public void Vector3ComposeDecompose_Works()
        {
            var compose = new Vector3ComposeNode();
            compose.X = 1; compose.Y = 2; compose.Z = 3;
            compose.OnProcess();
            Assert.AreEqual(new Vector3(1, 2, 3), compose.Vector);

            var decompose = new Vector3DecomposeNode();
            decompose.Vector = new Vector3(4, 5, 6);
            decompose.OnProcess();
            Assert.AreEqual(4f, decompose.X);
            Assert.AreEqual(5f, decompose.Y);
            Assert.AreEqual(6f, decompose.Z);
        }

        [Test]
        public void Vector3DistanceNode_CalculatesCorrectly()
        {
            var node = new Vector3DistanceNode();
            node.A = Vector3.zero;
            node.B = new Vector3(0, 0, 10);
            node.OnProcess();
            Assert.AreEqual(10f, node.Distance);
        }
    }
}
