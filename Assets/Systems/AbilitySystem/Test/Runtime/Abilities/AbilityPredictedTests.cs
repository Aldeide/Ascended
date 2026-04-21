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
            var serverAbilitySystem = CreateMockServerAbilitySystem().Object;
            var abilityDefinition = CreatePredictedAbilityDefinition();
            serverAbilitySystem.AbilityManager.GrantAbility(abilityDefinition);
            clientAbilitySystem.AbilityManager.GrantAbility(abilityDefinition);
            
            //OnServerTryActivateAbilityRequested
            
            clientAbilitySystem.AbilityManager.TryActivateAbility(abilityDefinition.UniqueName);
            
            Assert.IsTrue(clientAbilitySystem.AbilityManager.Abilities[abilityDefinition.UniqueName].IsActive, "Predicted ability isn't active on client but should.");
        }
    }
}