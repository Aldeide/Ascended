using AbilitySystem.Test.Utilities;
using GameplayTags.Runtime;
using NUnit.Framework;
using static AbilitySystem.Test.Utilities.AbilitySystemUtilities;
using static AbilitySystem.Test.Utilities.AbilityUtilities;

namespace AbilitySystem.Test.Runtime.Abilities
{
    public class AbilityServerTests
    {
        [Test]
        public void AbilityServerTests_ServerOnlyAbility_NotUsableByClient()
        {
            var clientAbilitySystem = CreateMockClientAbilitySystem().Object;
            var abilityDefinition = CreateServerAbilityDefinition();
            clientAbilitySystem.AbilityManager.GrantAbility(abilityDefinition);
            
            clientAbilitySystem.AbilityManager.TryActivateAbility(abilityDefinition.UniqueName);
            
            Assert.IsFalse(clientAbilitySystem.AbilityManager.Abilities[abilityDefinition.UniqueName].IsActive);
        }
        
        [Test]
        public void AbilityServerTests_ServerOnlyAbility_UsableByServer()
        {
            var serverAbilitySystem = CreateMockServerAbilitySystem().Object;
            var abilityDefinition = CreateServerAbilityDefinition();
            serverAbilitySystem.AbilityManager.GrantAbility(abilityDefinition);
            
            serverAbilitySystem.AbilityManager.TryActivateAbility(abilityDefinition.UniqueName);
            
            Assert.IsTrue(serverAbilitySystem.AbilityManager.Abilities[abilityDefinition.UniqueName].IsActive);
        }
        
        [Test]
        public void AbilityServerTests_ServerOnlyAbility_GrantsTagToServer()
        {
            var serverAbilitySystem = CreateMockServerAbilitySystem().Object;
            var abilityDefinition = CreateServerAbilityDefinition();
            serverAbilitySystem.AbilityManager.GrantAbility(abilityDefinition);
            
            serverAbilitySystem.AbilityManager.TryActivateAbility(abilityDefinition.UniqueName);
            
            Assert.IsTrue(serverAbilitySystem.TagManager.HasTag(new Tag("Tag.Test")));
            Assert.IsTrue(serverAbilitySystem.TagManager.AbilityTags.Count == 1);
        }
        
        
        [Test]
        public void AbilityServerTests_ServerOnlyAbility_GrantsTagToClient()
        {
            var serverAbilitySystem = CreateMockServerAbilitySystem().Object;
            var clientAbilitySystem = CreateMockClientAbilitySystem().Object;
            var abilityDefinition = CreateServerAbilityDefinition();
            serverAbilitySystem.AbilityManager.GrantAbility(abilityDefinition);
            serverAbilitySystem.ReplicationManager.OnNotifyClientsAbilityTagsAdded += (data) =>
            {
                clientAbilitySystem.TagManager.AddAbilityTags(data);
            };
            
            serverAbilitySystem.AbilityManager.TryActivateAbility(abilityDefinition.UniqueName);
            
            Assert.IsTrue(clientAbilitySystem.TagManager.HasTag(new Tag("Tag.Test")));
            Assert.IsTrue(clientAbilitySystem.TagManager.AbilityTags.Count == 1);
        }
        
    }
}