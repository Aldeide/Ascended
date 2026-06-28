using UnityEngine;

namespace AISystem.Runtime.Tactics
{
    public enum CoverType
    {
        None,
        Low,
        High
    }

    public class TacticalPoint : MonoBehaviour
    {
        public CoverType Type = CoverType.None;
        
        [Tooltip("The facing direction of the cover. If zero, transform.forward will be used.")]
        public Vector3 DirectionNormal = Vector3.zero;

        public Vector3 Position => transform.position;

        public Vector3 Normal => DirectionNormal == Vector3.zero ? transform.forward : DirectionNormal.normalized;

        public GameObject Occupier { get; set; }
        public bool IsOccupied => Occupier != null;

        private void OnEnable()
        {
            if (TacticalPointManager.Instance != null)
            {
                TacticalPointManager.Instance.RegisterPoint(this);
            }
        }

        private void OnDisable()
        {
            if (TacticalPointManager.Instance != null)
            {
                TacticalPointManager.Instance.UnregisterPoint(this);
            }
        }


        private void OnDrawGizmos()
        {
            Gizmos.color = IsOccupied ? Color.red : Color.blue;
            Gizmos.DrawWireSphere(transform.position, 0.5f);
            
            // Draw normal direction
            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(transform.position, Normal * 1f);
        }
    }
}
