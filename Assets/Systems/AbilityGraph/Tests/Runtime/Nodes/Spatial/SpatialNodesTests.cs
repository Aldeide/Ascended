using AbilityGraph.Runtime.Nodes.Spatial;
using NUnit.Framework;
using UnityEngine;

namespace AbilityGraph.Tests.Runtime.Nodes.Spatial
{
    public class SpatialNodesTests
    {
        private GameObject _targetObject;
        
        [SetUp]
        public void Setup()
        {
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
            Object.DestroyImmediate(_targetObject);
        }

        [Test]
        public void LineTraceNode_HitsTarget_OutputsCorrectly()
        {
            var node = new LineTraceNode
            {
                Start = Vector3.zero,
                End = new Vector3(0, 0, 20),
                MaxDistance = 50f,
                LayerMask = ~0
            };

            // Using reflection to call the protected Process method
            var method = typeof(LineTraceNode).GetMethod("Process", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            method.Invoke(node, null);

            Assert.IsTrue(node.DidHit);
            Assert.AreEqual(_targetObject, node.HitGameObject);
            Assert.AreEqual(new Vector3(0, 0, 9f), node.HitLocation); // Surface of the 2x2x2 box at z=10
        }

        [Test]
        public void LineTraceNode_MissesTarget_OutputsCorrectly()
        {
            var node = new LineTraceNode
            {
                Start = Vector3.zero,
                End = new Vector3(0, 10, 20), // Aiming above
                MaxDistance = 50f,
                LayerMask = ~0
            };

            var method = typeof(LineTraceNode).GetMethod("Process", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            method.Invoke(node, null);

            Assert.IsFalse(node.DidHit);
            Assert.IsNull(node.HitGameObject);
        }

        [Test]
        public void SphereTraceNode_HitsTarget_OutputsCorrectly()
        {
            var node = new SphereTraceNode
            {
                Start = new Vector3(2f, 0, 0), // Offset slightly
                End = new Vector3(2f, 0, 20),
                Radius = 2f, // Big enough to clip the edge of the 2x2x2 box at x=0
                MaxDistance = 50f,
                LayerMask = ~0
            };

            var method = typeof(SphereTraceNode).GetMethod("Process", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            method.Invoke(node, null);

            Assert.IsTrue(node.DidHit);
            Assert.AreEqual(_targetObject, node.HitGameObject);
        }

        [Test]
        public void BoxTraceNode_HitsTarget_OutputsCorrectly()
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

            var method = typeof(BoxTraceNode).GetMethod("Process", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            method.Invoke(node, null);

            Assert.IsTrue(node.DidHit);
            Assert.AreEqual(_targetObject, node.HitGameObject);
        }
    }
}
