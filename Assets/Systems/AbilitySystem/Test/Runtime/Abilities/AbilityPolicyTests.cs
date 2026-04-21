using AbilitySystem.Runtime.Abilities;
using GameplayTags.Runtime;
using static AbilitySystem.Test.Utilities.AbilityUtilities;
using static AbilitySystem.Test.Utilities.AbilitySystemUtilities;
using NUnit.Framework;

namespace AbilitySystem.Test.Runtime.Abilities
{
    public class AbilityPolicyTests
    {
        [Test]
        public void AbilityPolicyTests_ClientOnly_ClientOrServer_ActivatesOnClient()
        {
            var mockClientAbilitySystem = CreateMockClientAbilitySystem();
            var mockServerAbilitySystem = CreateMockServerAbilitySystem();
            var clientAbilitySystem = mockClientAbilitySystem.Object;
            var serverAbilitySystem = mockServerAbilitySystem.Object;
            var abilityDefinition = CreateTestAbilityDefinition();
            abilityDefinition.NetworkPolicy = AbilityNetworkPolicy.ClientOnly;
            abilityDefinition.NetworkSecurityPolicy = AbilityNetworkSecurityPolicy.ClientOrServer;
            clientAbilitySystem.AbilityManager.GrantAbility(abilityDefinition);
            serverAbilitySystem.AbilityManager.GrantAbility(abilityDefinition);
            
            var eventDispatched = false;
            clientAbilitySystem.AbilityManager.OnServerTryActivateAbilityRequested += (abilityName, key, data) =>
            {
                eventDispatched = true;
            };
            
            Assert.IsFalse(eventDispatched, "Client tried to request server ability activation but shouldn't have.");
            Assert.IsTrue(clientAbilitySystem.AbilityManager.TryActivateAbility(abilityDefinition.UniqueName));
        }

        [Test]
        public void AbilityPolicyTests_ClientOnly_ClientOrServer_DoesNotActivateOnServer()
        {
            var mockClientAbilitySystem = CreateMockClientAbilitySystem();
            var mockServerAbilitySystem = CreateMockServerAbilitySystem();
            var clientAbilitySystem = mockClientAbilitySystem.Object;
            var serverAbilitySystem = mockServerAbilitySystem.Object;
            var abilityDefinition = CreateTestAbilityDefinition();
            abilityDefinition.NetworkPolicy = AbilityNetworkPolicy.ClientOnly;
            abilityDefinition.NetworkSecurityPolicy = AbilityNetworkSecurityPolicy.ClientOrServer;
            clientAbilitySystem.AbilityManager.GrantAbility(abilityDefinition);
            serverAbilitySystem.AbilityManager.GrantAbility(abilityDefinition);
            
            var eventDispatched = false;
            clientAbilitySystem.AbilityManager.OnServerTryActivateAbilityRequested += (abilityName, key, data) =>
            {
                eventDispatched = true;
            };
            
            Assert.IsFalse(eventDispatched, "Client tried to request server ability activation but shouldn't have.");
            Assert.IsTrue(clientAbilitySystem.AbilityManager.TryActivateAbility(abilityDefinition.UniqueName));
        }
        
        [Test]
        public void AbilityPolicyTests_Server_ActivatesOnServer()
        {
            var mockServerAbilitySystem = CreateMockServerAbilitySystem();
            var serverAbilitySystem = mockServerAbilitySystem.Object;
            var abilityDefinition = CreateTestAbilityDefinition();
            abilityDefinition.NetworkPolicy = AbilityNetworkPolicy.Server;
            serverAbilitySystem.AbilityManager.GrantAbility(abilityDefinition);

            Assert.IsTrue(serverAbilitySystem.AbilityManager.TryActivateAbility(abilityDefinition.UniqueName));
        }
        
        [Test]
        public void AbilityPolicyTests_Server_DoesNotActivatesOnClient()
        {
            var mockClientAbilitySystem = CreateMockClientAbilitySystem();
            var clientAbilitySystem = mockClientAbilitySystem.Object;
            var abilityDefinition = CreateTestAbilityDefinition();
            abilityDefinition.NetworkPolicy = AbilityNetworkPolicy.Server;
            clientAbilitySystem.AbilityManager.GrantAbility(abilityDefinition);

            Assert.IsFalse(clientAbilitySystem.AbilityManager.TryActivateAbility(abilityDefinition.UniqueName));
        }
    }
}