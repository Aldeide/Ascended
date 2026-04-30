using System;
using AbilityGraph.Runtime.Nodes.Base;
using GraphProcessor;
using UnityEngine;

namespace AbilityGraph.Runtime.Nodes.Spatial
{
    [Serializable, NodeMenuItem("Spatial/Box Trace")]
    public class BoxTraceNode : LinearExecutableNode
    {
        [Input(name = "Start")]
        public Vector3 Start;

        [Input(name = "End")]
        public Vector3 End;

        [Input(name = "Half Extents")]
        public Vector3 HalfExtents = new Vector3(0.5f, 0.5f, 0.5f);

        [Input(name = "Orientation")]
        public Vector3 OrientationEuler = Vector3.zero;

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
            var rotation = Quaternion.Euler(OrientationEuler);

            if (Physics.BoxCast(Start, HalfExtents, direction, out RaycastHit hit, rotation, maxDist, LayerMask))
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
