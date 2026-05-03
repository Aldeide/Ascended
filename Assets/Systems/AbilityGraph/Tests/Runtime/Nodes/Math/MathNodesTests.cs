using AbilityGraph.Runtime.Nodes.Math;
using AbilitySystem.Test.Utilities;
using NUnit.Framework;
using UnityEngine;

namespace AbilityGraph.Tests.Runtime.Nodes.Math
{
    /// <summary>
    /// Tests for math and vector manipulation nodes in the Ability Graph.
    /// </summary>
    public class MathNodesTests : AbilitySystemTestBase
    {
        /// <summary>
        /// Validates that FloatArithmeticNode performs basic arithmetic operations correctly.
        /// </summary>
        [Test]
        public void MathNodesTests_FloatArithmeticNode_PerformsBasicOperations()
        {
            var node = new FloatArithmeticNode();
            node.A = 10f;
            node.B = 5f;

            node.Operation = FloatArithmeticType.Add;
            AbilityGraphTestUtilities.InvokeProcess(node);
            Assert.AreEqual(15f, node.Result);

            node.Operation = FloatArithmeticType.Multiply;
            AbilityGraphTestUtilities.InvokeProcess(node);
            Assert.AreEqual(50f, node.Result);

            node.Operation = FloatArithmeticType.Divide;
            AbilityGraphTestUtilities.InvokeProcess(node);
            Assert.AreEqual(2f, node.Result);
            
            node.Operation = FloatArithmeticType.Power;
            AbilityGraphTestUtilities.InvokeProcess(node);
            Assert.AreEqual(100000f, node.Result);
        }

        /// <summary>
        /// Validates that ComparisonNode correctly evaluates numerical comparisons.
        /// </summary>
        [Test]
        public void MathNodesTests_ComparisonNode_EvaluatesComparisonsCorrectly()
        {
            var node = new ComparisonNode();
            node.A = 10f;
            node.B = 5f;

            node.Comparison = ComparisonType.Greater;
            AbilityGraphTestUtilities.InvokeProcess(node);
            Assert.IsTrue(node.Result);

            node.Comparison = ComparisonType.Less;
            AbilityGraphTestUtilities.InvokeProcess(node);
            Assert.IsFalse(node.Result);
        }

        /// <summary>
        /// Validates that Vector3 composition and decomposition nodes work symmetrically.
        /// </summary>
        [Test]
        public void MathNodesTests_Vector3Nodes_ComposeAndDecomposeCorrectly()
        {
            var compose = new Vector3ComposeNode();
            compose.X = 1; compose.Y = 2; compose.Z = 3;
            AbilityGraphTestUtilities.InvokeProcess(compose);
            Assert.AreEqual(new Vector3(1, 2, 3), compose.Vector);

            var decompose = new Vector3DecomposeNode();
            decompose.Vector = new Vector3(4, 5, 6);
            AbilityGraphTestUtilities.InvokeProcess(decompose);
            Assert.AreEqual(4f, decompose.X);
            Assert.AreEqual(5f, decompose.Y);
            Assert.AreEqual(6f, decompose.Z);
        }

        /// <summary>
        /// Validates that Vector3DistanceNode calculates the Euclidean distance between two vectors.
        /// </summary>
        [Test]
        public void MathNodesTests_Vector3DistanceNode_CalculatesEuclideanDistance()
        {
            var node = new Vector3DistanceNode();
            node.A = Vector3.zero;
            node.B = new Vector3(0, 0, 10);
            AbilityGraphTestUtilities.InvokeProcess(node);
            Assert.AreEqual(10f, node.Distance);
        }
    }
}
