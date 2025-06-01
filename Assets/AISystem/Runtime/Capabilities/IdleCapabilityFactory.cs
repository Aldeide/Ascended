using AISystem.Runtime.Actions;
using AISystem.Runtime.Goals;
using AISystem.Runtime.Sensors;
using AISystem.Runtime.TargetKeys;
using AISystem.Runtime.WorldKeys;
using CrashKonijn.Goap.Core;
using CrashKonijn.Goap.Runtime;

namespace AISystem.Runtime.Capabilities
{
    public class IdleCapabilityFactory : CapabilityFactoryBase
    {
        public override ICapabilityConfig Create()
        {
            var builder = new CapabilityBuilder("IdleCapability");

            builder.AddGoal<IdleGoal>()
                .AddCondition<IsIdle>(Comparison.GreaterThanOrEqual, 1)
                .SetBaseCost(2);

            builder.AddAction<IdleAction>()
                .AddEffect<IsIdle>(EffectType.Increase)
                .SetTarget<IdleTarget>();

            builder.AddTargetSensor<IdleTargetSensor>()
                .SetTarget<IdleTarget>();
            
            return builder.Build();
        }
    }
}