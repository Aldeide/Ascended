using CrashKonijn.Agent.Core;
using CrashKonijn.Agent.Runtime;
using Systems.Controllers;
using Unity.Netcode;
using UnityEngine;

namespace AISystem.Runtime.Behaviours
{
    [RequireComponent(typeof(AnimationController))]
    public class AgentMoveBehaviour : NetworkBehaviour
    {
        private AgentBehaviour agent;
        private ITarget currentTarget;
        private bool shouldMove;

        private AnimationController _animationController;
        
        private void Awake()
        {
            agent = GetComponent<AgentBehaviour>();
            _animationController = GetComponent<AnimationController>();
        }

        private void OnEnable()
        {
            agent.Events.OnTargetInRange += OnTargetInRange;
            agent.Events.OnTargetChanged += OnTargetChanged;
            agent.Events.OnTargetNotInRange += TargetNotInRange;
            agent.Events.OnTargetLost += TargetLost;
        }

        private void OnDisable()
        {
            agent.Events.OnTargetInRange -= OnTargetInRange;
            agent.Events.OnTargetChanged -= OnTargetChanged;
            agent.Events.OnTargetNotInRange -= TargetNotInRange;
            agent.Events.OnTargetLost -= TargetLost;
        }

        private void TargetLost()
        {
            currentTarget = null;
            shouldMove = false;
        }

        private void OnTargetInRange(ITarget target)
        {
            shouldMove = false;
        }

        private void OnTargetChanged(ITarget target, bool inRange)
        {
            currentTarget = target;
            shouldMove = !inRange;
        }

        private void TargetNotInRange(ITarget target)
        {
            shouldMove = true;
        }

        public void Update()
        {
            if (!IsServer) return;
            if (agent.IsPaused)
                return;

            if (!shouldMove)
            {
                _animationController.StopMovement();
                return;
            }
            
            if (currentTarget == null)
                return;

            transform.position = Vector3.MoveTowards(transform.position,
                new Vector3(currentTarget.Position.x, transform.position.y, currentTarget.Position.z),
                Time.deltaTime);
            _animationController.SetMoveForward();

        }

        private void OnDrawGizmos()
        {
            if (currentTarget == null)
                return;

            Gizmos.DrawLine(transform.position, currentTarget.Position);
        }
    }
}