using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.Abilities.Targeting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace AbilitySystem.Runtime.Abilities.Targeting
{
    public class GroundTraceTargetActor : AbilityTargetActor
    {
        public LayerMask TraceMask = Physics.DefaultRaycastLayers;
        public float MaxRange = 1000f;
        public ReticleComponent ReticlePrefab;
        
        private ReticleComponent _spawnedReticle;
        private Vector3 _currentHitPoint;
        private bool _hasHit;

        public override void StartTargeting(Ability ability)
        {
            base.StartTargeting(ability);

            if (ReticlePrefab != null)
            {
                _spawnedReticle = Instantiate(ReticlePrefab);
            }
        }

        private void Update()
        {
            if (!IsTargeting) return;

            UpdateReticlePosition();
            HandleInput();
        }

        private void UpdateReticlePosition()
        {
            if (Camera.main == null) return;

            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            
            _hasHit = Physics.Raycast(ray, out RaycastHit hit, MaxRange, TraceMask);

            if (_hasHit)
            {
                _currentHitPoint = hit.point;
                if (_spawnedReticle != null)
                {
                    _spawnedReticle.transform.position = _currentHitPoint;
                    _spawnedReticle.SetValidTarget(true);
                }
            }
            else
            {
                if (_spawnedReticle != null)
                {
                    _spawnedReticle.SetValidTarget(false);
                }
            }
        }

        private void HandleInput()
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                ConfirmTargeting();
            }
            else if (Mouse.current.rightButton.wasPressedThisFrame)
            {
                CancelTargeting();
            }
        }

        protected override TargetDataHandle GetTargetData()
        {
            var handle = new TargetDataHandle();
            
            if (_hasHit)
            {
                // Alternatively, could return TargetDataHitResult if we stored the full RaycastHit
                var locationData = new TargetDataLocation { Position = _currentHitPoint };
                handle.Add(locationData);
            }
            
            return handle;
        }

        private void OnDestroy()
        {
            if (_spawnedReticle != null)
            {
                Destroy(_spawnedReticle.gameObject);
            }
        }
    }
}
