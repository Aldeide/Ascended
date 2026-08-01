using AISystem.Runtime.Goals;
using CrashKonijn.Agent.Runtime;
using CrashKonijn.Goap.Runtime;
using Unity.Netcode;

namespace AISystem.Runtime.DecisionMakers
{
    public class DefaultAgentDecisionMaker : NetworkBehaviour
    {
        private AgentBehaviour _agent;
        private GoapActionProvider _provider;
        private GoapBehaviour _goap;
        private void Awake()
        {
            _goap = FindAnyObjectByType<GoapBehaviour>();
            _agent = GetComponent<AgentBehaviour>();
            _provider = GetComponent<GoapActionProvider>();
            
            // This only applies to the code demo
            if (_provider != null && _provider.AgentTypeBehaviour == null && _goap != null)
                _provider.AgentType = _goap.GetAgentType("DefaultAgent");
        }

        private void Start()
        {
            if (_provider != null && (_provider.AgentType != null || _provider.AgentTypeBehaviour != null))
            {
                _provider.RequestGoal<IdleGoal>();
            }
        }
    }
}