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
            _goap = FindObjectOfType<GoapBehaviour>();
            _agent = GetComponent<AgentBehaviour>();
            _provider = GetComponent<GoapActionProvider>();
            
            // This only applies to the code demo
            if (_provider.AgentTypeBehaviour == null)
                _provider.AgentType = _goap.GetAgentType("DefaultAgent");
        }

        private void Start()
        {
            _provider.RequestGoal<IdleGoal>();
        }
    }
}