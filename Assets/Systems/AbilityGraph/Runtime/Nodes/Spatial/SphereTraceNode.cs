using System;
using AbilityGraph.Runtime.Nodes.Base;
using GraphProcessor;
using UnityEngine;

namespace AbilityGraph.Runtime.Nodes.Spatial
{
    [Serializable, NodeMenuItem("Spatial/Sphere Trace")]
    public class SphereTraceNode : LinearExecutableNode
    {
        [Input(name = "Start")]
        public Vector3 Start;

        [Input(name = "End")]
        public Vector3 End;

        [Input(name = "Radius")]
        public float Radius = 0.5f;

        [Input(name = "Max Distance")]
        public float MaxDistance = 100f;

        [SerializeField]
        public LayerMask LayerMask = Physics.DefaultRaycastLayers;

        [Output(name = "Hit GameObject")]
        public GameObject HitGameObject;

        [Output(name = "Hit Location")]
        public Vector3 HitLocation;

        [Output(name = "Did Hit")]
        public bool DidHit;

        protected override void Process()
        {
            var direction = End - Start;
            var maxDist = Mathf.Min(MaxDistance, direction.magnitude);
            direction.Normalize();

            if (Physics.SphereCast(Start, Radius, direction, out RaycastHit hit, maxDist, LayerMask))
            {
                DidHit = true;
                HitGameObject = hit.collider.gameObject;
                HitLocation = hit.point;
            }
            else
            {
                DidHit = false;
                HitGameObject = null;
                HitLocation = Start + direction * maxDist;
            }
        }
    }
}
