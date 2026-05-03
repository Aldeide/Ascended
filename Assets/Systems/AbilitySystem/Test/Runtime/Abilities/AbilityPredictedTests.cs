using AbilitySystem.Test.Utilities;
using NUnit.Framework;

namespace AbilitySystem.Test.Runtime.Abilities
{
    /// <summary>
    /// Unit tests for client-side predicted abilities, ensuring they correctly initiate network synchronization.
    /// </summary>
    public class AbilityPredictedTests : AbilitySystemTestBase
    {
        /// <summary>
        /// Verifies that a client-predicted ability correctly triggers a server activation request when initiated by the client.
        /// </summary>
        [Test]
        public void AbilityPredictedTests_ClientActivation_DispatchesServerRequest()
        {
            var clientSystemMock = AbilitySystemUtilities.CreateMockClientAbilitySystem();
            var clientSystem = clientSystemMock.Object;
            
            var abilityDefinition = AbilityUtilities.CreatePredictedAbilityDefinition();
            clientSystem.AbilityManager.GrantAbility(abilityDefinition);
            
            bool requestDispatched = false;
            clientSystem.ReplicationManager.OnServerAbilityActivationRequested += (name, key, data) => requestDispatched = true;
            
            clientSystem.AbilityManager.TryActivateAbility(abilityDefinition.UniqueName);
            
            Assert.IsTrue(requestDispatched, "Predicted ability should have dispatched a server activation request");
        }
    }
}