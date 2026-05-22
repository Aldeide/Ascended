using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Core;
using CrashKonijn.Goap.Runtime;
using AISystem.Runtime.DecisionMakers;

namespace AISystem.Runtime.Sensors
{
    public class RoleSensor : LocalWorldSensorBase
    {
        public EnemyRole TargetRole { get; set; }

        public override void Created() {}

        public override void Update() {}

        public override SenseValue Sense(IActionReceiver agent, IComponentReference references)
        {
            var decisionMaker = agent.Transform.GetComponent<EnemyDecisionMaker>();
            if (decisionMaker == null)
                return false;

            return decisionMaker.Role == TargetRole;
        }
    }
}
