using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.Effects;
using AbilitySystem.Runtime.Networking;
using AbilitySystem.Runtime.Modifiers;
using AbilitySystem.Test.Utilities;
using NUnit.Framework;
using UnityEngine;
using System.Linq;
using GameplayTags.Runtime;
using UnityEditor.PackageManager;
using static AbilitySystem.Test.Utilities.EffectUtilities;

namespace AbilitySystem.Test.Runtime.Networking
{
    public class TagReplicationTests : AbilitySystemSyncTestBase
    {
        [Test]
        public void TagReplicationTests_ServerAppliesTagViaEffect_ClientReceivesTag()
        {
            var serverEffect = CreateDurationalEffectWithTag(ServerSystem, ServerSystem);
            DataManager.Effects.Add(serverEffect.Definition.name, serverEffect.Definition);
            ServerSystem.EffectManager.AddEffectFromServer(serverEffect);

            Assert.IsTrue(ClientSystem.EffectManager.Effects.Any(e => e.Definition.name == "TestEffectWithTag"), 
                "Client should have received the effect from server via LinkSystems");

            Assert.IsTrue(ClientSystem.TagManager.HasTag(new Tag("Tag.Test.GrantedTag")),
                "Client should have received the tag from server via LinkSystems");
        }
        
        [Test]
        public void TagReplicationTests_ServerRemovesTagViaEffectExpired_ClientRemovesTag()
        {
            var serverEffect = CreateDurationalEffectWithTag(ServerSystem, ServerSystem);
            DataManager.Effects.Add(serverEffect.Definition.name, serverEffect.Definition);
            ServerSystem.EffectManager.AddEffectFromServer(serverEffect);

            Assert.IsTrue(ClientSystem.TagManager.HasTag(new Tag("Tag.Test.GrantedTag")),
                "Client should have received the tag from server via LinkSystems");
            
            ServerSystem.EffectManager.RemoveEffect(serverEffect);
            
            Assert.IsFalse(ClientSystem.TagManager.HasTag(new Tag("Tag.Test.GrantedTag")),
                "Client should have removed the tag.");
        }

        [Test]
        public void TagReplicationTests_ServerAppliesTagViaAbility_ClientReceivesTag()
        {
            var abilityDef = ScriptableObject.CreateInstance<PassiveAbilityDefinition>();
            abilityDef.UniqueName = "TestAbilityWithTag";
            abilityDef.NetworkPolicy = AbilityNetworkPolicy.Server;
            abilityDef.ActivationRequiredTags = new Tag[0];
            abilityDef.ActivationBlockedTags = new Tag[0];
            abilityDef.ActivationOwnedTags = new[] { new Tag("Tag.Test.AbilityGrantedTag") };
            abilityDef.CancelAbilityTags = new Tag[0];
            abilityDef.AssetTags = new Tag[0];
            abilityDef.GrantedEffects = new EffectDefinition[0];

            DataManager.Abilities.Add(abilityDef.UniqueName, abilityDef);
            ServerSystem.AbilityManager.GrantAbility(abilityDef);

            Assert.IsTrue(ClientSystem.TagManager.HasTag(new Tag("Tag.Test.AbilityGrantedTag")),
                "Client should have received the ability-granted tag from server via LinkSystems");
        }

        [Test]
        public void TagReplicationTests_ServerRemovesTagViaAbilityEnded_ClientRemovesTag()
        {
            var abilityDef = ScriptableObject.CreateInstance<PassiveAbilityDefinition>();
            abilityDef.UniqueName = "TestAbilityWithTag";
            abilityDef.NetworkPolicy = AbilityNetworkPolicy.Server;
            abilityDef.ActivationRequiredTags = new Tag[0];
            abilityDef.ActivationBlockedTags = new Tag[0];
            abilityDef.ActivationOwnedTags = new[] { new Tag("Tag.Test.AbilityGrantedTag") };
            abilityDef.CancelAbilityTags = new Tag[0];
            abilityDef.AssetTags = new Tag[0];
            abilityDef.GrantedEffects = new EffectDefinition[0];

            DataManager.Abilities.Add(abilityDef.UniqueName, abilityDef);
            ServerSystem.AbilityManager.GrantAbility(abilityDef);

            Assert.IsTrue(ClientSystem.TagManager.HasTag(new Tag("Tag.Test.AbilityGrantedTag")),
                "Client should have received the ability-granted tag from server via LinkSystems");

            var serverAbility = ServerSystem.AbilityManager.Abilities[abilityDef.UniqueName];
            serverAbility.TryEndAbility();

            Assert.IsFalse(ClientSystem.TagManager.HasTag(new Tag("Tag.Test.AbilityGrantedTag")),
                "Client should have removed the ability-granted tag after the ability ended.");
        }
    }
}