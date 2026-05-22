using CrashKonijn.Agent.Core;
using CrashKonijn.Agent.Runtime;
using Systems.Animation;
using Systems.Controllers;
using Unity.Netcode;
using UnityEngine;
using Pathfinding;

namespace AISystem.Runtime.Behaviours
{
    [RequireComponent(typeof(AnimationController))]
    public class AgentMoveBehaviour : NetworkBehaviour
    {
        private AgentBehaviour agent;
        private ITarget currentTarget;
        private bool shouldMove;

        private AnimationController _animationController;
        private IAstarAI ai;
        
        private void Awake()
        {
            agent = GetComponent<AgentBehaviour>();
            _animationController = GetComponent<AnimationController>();
            ai = GetComponent<IAstarAI>();
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
            if (ai != null)
            {
                ai.isStopped = true;
            }
        }

        private void OnTargetInRange(ITarget target)
        {
            shouldMove = false;
            if (ai != null)
            {
                ai.isStopped = true;
            }
        }

        private void OnTargetChanged(ITarget target, bool inRange)
        {
            currentTarget = target;
            shouldMove = !inRange;
            if (ai != null)
            {
                if (shouldMove && target != null)
                {
                    ai.destination = target.Position;
                    ai.isStopped = false;
                }
                else
                {
                    ai.isStopped = true;
                }
            }
        }

        private void TargetNotInRange(ITarget target)
        {
            shouldMove = true;
            if (ai != null && target != null)
            {
                ai.destination = target.Position;
                ai.isStopped = false;
            }
        }

        public void Update()
        {
            if (!IsServer) return;
            if (agent.IsPaused)
                return;

            if (!shouldMove)
            {
                _animationController.StopMovement();
                if (ai != null)
                {
                    ai.isStopped = true;
                }
                return;
            }
            
            if (currentTarget == null)
                return;

            if (ai != null)
            {
                // Update destination in case target position is dynamic
                ai.destination = currentTarget.Position;
                ai.isStopped = false;

                if (ai.velocity.sqrMagnitude > 0.01f)
                {
                    _animationController.SetMoveForward();
                }
                else
                {
                    _animationController.StopMovement();
                }
            }
            else
            {
                // Fallback to direct MoveTowards if A* Pathfinding Pro is not attached
                transform.position = Vector3.MoveTowards(transform.position,
                    new Vector3(currentTarget.Position.x, transform.position.y, currentTarget.Position.z),
                    Time.deltaTime);
                _animationController.SetMoveForward();
            }
        }

        private void OnDrawGizmos()
        {
            if (currentTarget == null)
                return;

            Gizmos.DrawLine(transform.position, currentTarget.Position);
        }
    }
}