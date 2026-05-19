using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.AttributeSets;
using AbilitySystem.Runtime.Core;
using AbilitySystem.Runtime.Effects;
using AbilitySystem.Runtime.Events;
using AbilitySystem.Runtime.Modifiers;
using AbilitySystem.Runtime.Networking;
using AbilitySystem.Runtime.Tags;
using Moq;

namespace AbilitySystem.Test.Utilities
{
    public static class AbilitySystemUtilities
    {
        public static Mock<IAbilitySystem> CreateMockAbilitySystem(bool addDefaultAttributes = true)
        {
            var owner = new Mock<IAbilitySystem>();
            SetupDefaultMocks(owner, addDefaultAttributes);
            return owner;
        }
        
        public static Mock<IAbilitySystem> CreateMockServerAbilitySystem(bool addDefaultAttributes = true)
        {
            var owner = new Mock<IAbilitySystem>();
            owner.Setup(x => x.IsServer()).Returns(true);
            owner.Setup(x => x.IsLocalClient()).Returns(false);
            SetupDefaultMocks(owner, addDefaultAttributes);
            return owner;
        }
        
        public static Mock<IAbilitySystem> CreateMockClientAbilitySystem(bool addDefaultAttributes = true)
        {
            var owner = new Mock<IAbilitySystem>();
            owner.Setup(x => x.IsServer()).Returns(false);
            owner.Setup(x=>x.IsHost()).Returns(false);
            owner.Setup(x => x.IsLocalClient()).Returns(true);
            SetupDefaultMocks(owner, addDefaultAttributes);
            return owner;
        }

        private static void SetupDefaultMocks(Mock<IAbilitySystem> owner, bool addDefaultAttributes = true)
        {
            var networkRole = new Mock<INetworkRole>();
            networkRole.SetupGet(nr => nr.NetworkObjectId).Returns(1);
            owner.SetupGet(o => o.NetworkRole).Returns(networkRole.Object);

            var effectManager = new EffectManager(owner.Object);
            owner.Setup(x => x.EffectManager).Returns(effectManager);
            var eventManager = new EventManager();
            owner.Setup(x => x.EventManager).Returns(eventManager);
            var tagManager = new GameplayTagManager(owner.Object);
            owner.Setup(x => x.TagManager).Returns(tagManager);
            var attributeSetManager = new AttributeSetManager(owner.Object);
            if (addDefaultAttributes)
            {
                attributeSetManager.AddAttributeSet(typeof(TestAttributeSet), new TestAttributeSet(owner.Object));
            }
            owner.SetupGet(x => x.AttributeSetManager).Returns(attributeSetManager);
            var replicationManager = new MockReplicationManager(owner.Object);
            owner.Setup(x => x.ReplicationManager).Returns(replicationManager);
            var abilityManager = new AbilityManager(owner.Object);
            owner.Setup(x => x.AbilityManager).Returns(abilityManager);
            var dataManager = new Mock<IDataManager>();
            owner.Setup(x => x.DataManager).Returns(dataManager.Object);
            replicationManager.DataManager = dataManager.Object;

            owner.Setup(x => x.MakeEffectContext()).Returns(() => new EffectContext(owner.Object, owner.Object));
            owner.Setup(x => x.MakeOutgoingEffect(It.IsAny<EffectDefinition>(), It.IsAny<int>(), It.IsAny<EffectContext>()))
                .Returns((EffectDefinition def, int level, EffectContext context) => def.ToEffect(owner.Object, owner.Object, context));
            owner.Setup(x => x.ApplyEffectToSelf(It.IsAny<Effect>()))
                .Returns((Effect eff) =>
                {
                    eff.Initialise(eff.Source, owner.Object, eff.Context, eff.Level);
                    eff.Activate();
                    return owner.Object.EffectManager.AddEffect(eff);
                });

            owner.Setup(x => x.GetGameObjectFromNetworkId(It.IsAny<ulong>()))
                .Returns((ulong id) => null);
        }
        public static void LinkAbilitySystems(Mock<IAbilitySystem> client, Mock<IAbilitySystem> server)
        {
            var clientRep = (MockReplicationManager)client.Object.ReplicationManager;
            var serverRep = (MockReplicationManager)server.Object.ReplicationManager;

            // Client -> Server
            clientRep.OnServerAbilityActivationRequested = (name, key, data) => 
                serverRep.ProcessServerAbilityActivation(name, key, data);
            
            clientRep.OnServerAbilityUnpredictedActivationRequested = (name, data) =>
                serverRep.ProcessServerAbilityUnpredictedActivation(name, data);

            clientRep.OnServerAbilityTerminationRequested = (name) =>
                serverRep.ProcessServerAbilityTermination(name);

            // Server -> Client
            serverRep.OnAbilityActivationResponded = (key, success) => 
                client.Object.AbilityManager.NotifyServerResponse(key, success);

            serverRep.OnClientActivateAbility = (name, data) =>
                clientRep.ProcessClientActivateAbility(name, data);

            serverRep.OnClientEndAbility = (name) =>
                clientRep.ProcessClientEndAbility(name);

            // Client -> Server Sync Key
            clientRep.OnServerSyncKeyReceived = (name, key) =>
                serverRep.ProcessServerSyncKey(name, key);

            // Server -> Client Sync Key
            serverRep.OnClientSyncKeyConfirmed = (name, key) =>
                clientRep.ProcessClientSyncKeyConfirmed(name, key);
        }

        /// <summary>
        /// Creates a simple EffectDefinition for testing.
        /// </summary>
        public static EffectDefinition CreateEffectDefinition(EffectDurationType durationType, float duration = 0f, Modifier[] modifiers = null)
        {
            var def = UnityEngine.ScriptableObject.CreateInstance<EffectDefinition>();
            def.DurationType = durationType;
            def.DurationSeconds = duration;
            def.Modifiers = modifiers ?? System.Array.Empty<Modifier>();
            return def;
        }

        /// <summary>
        /// Creates an AttributeBasedModifier for testing.
        /// </summary>
        public static AttributeBasedModifier CreateAttributeBasedModifier(
            string attributeName, 
            AttributeBasedModifier.AttributeFrom fromType, 
            string fromAttributeName, 
            AttributeBasedModifier.AttributeCaptureType captureType,
            EffectOperation operation = EffectOperation.Additive,
            float k = 1f, 
            float b = 0f)
        {
            return new AttributeBasedModifier
            {
                AttributeName = attributeName,
                Operation = operation,
                attributeFromType = fromType,
                attributeFromName = fromAttributeName,
                captureType = captureType,
                k = k,
                b = b
            };
        }
    }
}