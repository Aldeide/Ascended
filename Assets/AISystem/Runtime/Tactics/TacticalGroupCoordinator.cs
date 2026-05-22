using System.Collections.Generic;
using AISystem.Runtime.DecisionMakers;
using UnityEngine;

namespace AISystem.Runtime.Tactics
{
    public class TacticalGroupCoordinator : MonoBehaviour
    {
        public static TacticalGroupCoordinator Instance { get; private set; }

        private readonly List<EnemyDecisionMaker> _agents = new();

        [SerializeField] private float roleReevaluationInterval = 3f;
        private float _reevaluationTimer;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void RegisterAgent(EnemyDecisionMaker agent)
        {
            if (!_agents.Contains(agent))
            {
                _agents.Add(agent);
                ReassignRoles();
            }
        }

        public void UnregisterAgent(EnemyDecisionMaker agent)
        {
            if (_agents.Remove(agent))
            {
                if (TacticalPointManager.Instance != null)
                {
                    TacticalPointManager.Instance.ReleaseAllPointsForAgent(agent.gameObject);
                }
                ReassignRoles();
            }
        }

        private void Update()
        {
            _reevaluationTimer += Time.deltaTime;
            if (_reevaluationTimer >= roleReevaluationInterval)
            {
                _reevaluationTimer = 0f;
                ReassignRoles();
            }
        }

        public void ReassignRoles()
        {
            if (_agents.Count == 0) return;

            for (int i = 0; i < _agents.Count; i++)
            {
                int roleIndex = i % 4;
                EnemyRole assignedRole = EnemyRole.Vanguard;
                switch (roleIndex)
                {
                    case 0:
                        assignedRole = EnemyRole.Vanguard;
                        break;
                    case 1:
                        assignedRole = EnemyRole.Flanker;
                        break;
                    case 2:
                        assignedRole = EnemyRole.Suppressor;
                        break;
                    case 3:
                        assignedRole = EnemyRole.Support;
                        break;
                }
                _agents[i].Role = assignedRole;
            }
        }
    }
}
