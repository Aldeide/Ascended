using AISystem.Runtime.Capabilities;
using CrashKonijn.Goap.Core;
using CrashKonijn.Goap.Runtime;

namespace AISystem.Runtime.AgentTypes
{
    public class DefaultAgentTypeFactory : AgentTypeFactoryBase
    {
        public override IAgentTypeConfig Create()
        {
            var factory = new AgentTypeBuilder("DefaultAgent");
            
            factory.AddCapability<IdleCapabilityFactory>();

            return factory.Build();
        }
    }
}