using AbilityGraph.Runtime.Nodes.Spatial;
using AbilitySystem.Test.Utilities;
using NUnit.Framework;
using UnityEngine;

namespace AbilityGraph.Tests.Runtime.Nodes.Spatial
{
    /// <summary>
    /// Tests for spatial and physics-based nodes in the Ability Graph (traces, collisions).
    /// </summary>
    public class SpatialNodesTests : AbilitySystemTestBase
    {
        private GameObject _targetObject;
        
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            _targetObject = new GameObject("TestTarget");
            _targetObject.transform.position = new Vector3(0, 0, 10);
            var collider = _targetObject.AddComponent<BoxCollider>();
            collider.size = new Vector3(2, 2, 2);
            // Ensure physics updates its internal state
            Physics.SyncTransforms();
        }

        [TearDown]
        public void Teardown()
        {
            if (_targetObject != null) Object.DestroyImmediate(_targetObject);
        }

        /// <summary>
        /// Validates that LineTraceNode correctly identifies a hit on a target within its path.
        /// </summary>
        [Test]
        public void SpatialNodesTests_LineTraceNode_HitsTargetCorrectly()
        {
            var node = new LineTraceNode
            {
                Start = Vector3.zero,
                End = new Vector3(0, 0, 20),
                MaxDistance = 50f,
                LayerMask = ~0
            };

            AbilityGraphTestUtilities.InvokeProcess(node);

            Assert.IsTrue(node.DidHit);
            Assert.AreEqual(_targetObject, node.HitGameObject);
            Assert.AreEqual(new Vector3(0, 0, 9f), node.HitLocation); // Surface of the 2x2x2 box at z=10
        }

        /// <summary>
        /// Validates that LineTraceNode correctly reports a miss when the path does not intersect any objects.
        /// </summary>
        [Test]
        public void SpatialNodesTests_LineTraceNode_MissesTargetCorrectly()
        {
            var node = new LineTraceNode
            {
                Start = Vector3.zero,
                End = new Vector3(0, 10, 20), // Aiming above
                MaxDistance = 50f,
                LayerMask = ~0
            };

            AbilityGraphTestUtilities.InvokeProcess(node);

            Assert.IsFalse(node.DidHit);
            Assert.IsNull(node.HitGameObject);
        }

        /// <summary>
        /// Validates that SphereTraceNode correctly identifies a hit using a spherical volume.
        /// </summary>
        [Test]
        public void SpatialNodesTests_SphereTraceNode_HitsTargetCorrectly()
        {
            var node = new SphereTraceNode
            {
                Start = new Vector3(2f, 0, 0), // Offset slightly
                End = new Vector3(2f, 0, 20),
                Radius = 2f, // Big enough to clip the edge of the 2x2x2 box at x=0
                MaxDistance = 50f,
                LayerMask = ~0
            };

            AbilityGraphTestUtilities.InvokeProcess(node);

            Assert.IsTrue(node.DidHit);
            Assert.AreEqual(_targetObject, node.HitGameObject);
        }

        /// <summary>
        /// Validates that BoxTraceNode correctly identifies a hit using a box-shaped volume.
        /// </summary>
        [Test]
        public void SpatialNodesTests_BoxTraceNode_HitsTargetCorrectly()
        {
            var node = new BoxTraceNode
            {
                Start = new Vector3(0, 15, 0), // Above
                End = new Vector3(0, -5, 0), // Aiming straight down
                HalfExtents = new Vector3(5, 5, 5), // Massive box
                MaxDistance = 50f,
                LayerMask = ~0
            };
            
            // Move target underneath
            _targetObject.transform.position = new Vector3(0, 2, 0);
            Physics.SyncTransforms();

            AbilityGraphTestUtilities.InvokeProcess(node);

            Assert.IsTrue(node.DidHit);
            Assert.AreEqual(_targetObject, node.HitGameObject);
        }
    }
}
