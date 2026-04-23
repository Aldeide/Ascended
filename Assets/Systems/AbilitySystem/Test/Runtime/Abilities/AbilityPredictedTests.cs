using System.Linq;
using AbilitySystem.Runtime.Networking;
using AbilitySystem.Test.Utilities;
using GameplayTags.Runtime;
using NUnit.Framework;
using UnityEngine;
using static AbilitySystem.Test.Utilities.AbilitySystemUtilities;
using static AbilitySystem.Test.Utilities.AbilityUtilities;

namespace AbilitySystem.Test.Runtime.Abilities
{
    public class AbilityPredictedTests
    {
        [Test]
        public void AbilityPredictedTests_PredictedAbility_TriggersRequestOnServer()
        {
            var clientAbilitySystem = CreateMockClientAbilitySystem().Object;
            var abilityDefinition = CreatePredictedAbilityDefinition();
            clientAbilitySystem.AbilityManager.GrantAbility(abilityDefinition);
            
            var eventDispatched = false;
            clientAbilitySystem.ReplicationManager.OnServerAbilityActivationRequested += (abilityName, key, data) =>
            {
                eventDispatched = true;
            };
            
            clientAbilitySystem.AbilityManager.TryActivateAbility(abilityDefinition.UniqueName);
            
            Assert.IsTrue(eventDispatched, "Predicted ability didn't call server.");
        }
    }
}