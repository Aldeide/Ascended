using AISystem.Runtime.Goals;
using AISystem.Runtime.Tactics;
using AbilitySystem.Scripts;
using CrashKonijn.Agent.Runtime;
using CrashKonijn.Goap.Runtime;
using Unity.Netcode;
using UnityEngine;
using System;

namespace AISystem.Runtime.DecisionMakers
{
    public enum EnemyRole
    {
        Vanguard,
        Flanker,
        Suppressor,
        Support
    }

    [RequireComponent(typeof(AgentBehaviour))]
    [RequireComponent(typeof(GoapActionProvider))]
    public class EnemyDecisionMaker : NetworkBehaviour
    {
        private AgentBehaviour _agent;
        private GoapActionProvider _provider;
        private AbilitySystemComponent _asc;
        private GoapBehaviour _goap;

        public EnemyRole Role { get; set; } = EnemyRole.Vanguard;

        private Type _currentGoalType;

        private void Awake()
        {
            _goap = FindObjectOfType<GoapBehaviour>();
            _agent = GetComponent<AgentBehaviour>();
            _provider = GetComponent<GoapActionProvider>();
            _asc = GetComponent<AbilitySystemComponent>();

            // Associate with AgentType config if not set in Inspector
            if (_provider.AgentTypeBehaviour == null && _goap != null)
            {
                _provider.AgentType = _goap.GetAgentType("DefaultAgent");
            }
        }

        private void Start()
        {
            if (TacticalGroupCoordinator.Instance != null)
            {
                TacticalGroupCoordinator.Instance.RegisterAgent(this);
            }
            EvaluateGoal();
        }

        private void OnDestroy()
        {
            if (TacticalGroupCoordinator.Instance != null)
            {
                TacticalGroupCoordinator.Instance.UnregisterAgent(this);
            }
        }

        private void Update()
        {
            if (!IsServer) return;
            EvaluateGoal();
        }

        private void EvaluateGoal()
        {
            Type desiredGoalType = typeof(KillEnemyGoal);

            // 1. Check if health is low
            if (_asc != null && _asc.IsInitialized)
            {
                var health = _asc.AbilitySystem.AttributeSetManager.GetAttribute("Health");
                var maxHealth = _asc.AbilitySystem.AttributeSetManager.GetAttribute("MaxHealth");
                if (health != null && maxHealth != null && maxHealth.CurrentValue > 0f)
                {
                    if ((health.CurrentValue / maxHealth.CurrentValue) < 0.3f)
                    {
                        desiredGoalType = typeof(SurvivalGoal);
                    }
                }
            }

            // 2. Support role checks if allies need healing
            if (desiredGoalType == typeof(KillEnemyGoal) && Role == EnemyRole.Support)
            {
                if (CheckAlliesNeedHealing())
                {
                    desiredGoalType = typeof(HealAllyGoal);
                }
            }

            // Request goal if changed
            if (_currentGoalType != desiredGoalType)
            {
                _currentGoalType = desiredGoalType;
                if (_provider != null && (_provider.AgentType != null || _provider.AgentTypeBehaviour != null))
                {
                    _provider.RequestGoal(_currentGoalType, false);
                }
            }
        }

        private bool CheckAlliesNeedHealing()
        {
            // BOLT: Replaced FindObjectsOfType with centralized static registry for better performance
            var components = AbilitySystemComponent.ActiveInstances;
            foreach (var comp in components)
            {
                if (comp == null || comp.gameObject == null || comp.gameObject == gameObject) continue;
                if (!comp.CompareTag("Enemy") && comp.GetComponent<EnemyDecisionMaker>() == null)
                    continue;

                if (comp.IsInitialized)
                {
                    var health = comp.AbilitySystem.AttributeSetManager.GetAttribute("Health");
                    var maxHealth = comp.AbilitySystem.AttributeSetManager.GetAttribute("MaxHealth");
                    if (health != null && maxHealth != null && maxHealth.CurrentValue > 0f)
                    {
                        if ((health.CurrentValue / maxHealth.CurrentValue) < 0.5f)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }
}
