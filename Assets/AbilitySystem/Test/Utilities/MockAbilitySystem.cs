using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.AttributeSets;
using AbilitySystem.Runtime.Core;
using AbilitySystem.Runtime.Cues;
using AbilitySystem.Runtime.Effects;
using AbilitySystem.Runtime.Networking;
using AbilitySystem.Runtime.Tags;
using AbilitySystem.Scripts;

namespace AbilitySystem.Test.Utilities
{
    public class MockAbilitySystem : IAbilitySystem
    {
        public AbilitySystemComponent Component { get; set; }
        GameplayTagManager IAbilitySystem.TagManager { get; set; }
        EffectManager IAbilitySystem.EffectManager { get; set; }
        AbilityManager IAbilitySystem.AbilityManager { get; set; }
        AttributeSetManager IAbilitySystem.AttributeSetManager { get; set; }
        public IReplicationManager ReplicationManager { get; set; }
        public void Initialise(AbilitySystemComponent component)
        {
            throw new System.NotImplementedException();
        }

        public void Tick()
        {
            throw new System.NotImplementedException();
        }

        public float GetTime()
        {
            return 0;
        }

        public bool IsLocalClient()
        {
            return true;
        }

        public bool IsServer()
        {
            return true;
        }

        public bool IsHost()
        {
            return true;
        }

        public bool HasAuthority()
        {
            return true;
        }

        public void PlayCue(CueDefinition cue)
        {
            throw new System.NotImplementedException();
        }

        public void PlayCue(CueDefinition cue, CueData data)
        {
            throw new System.NotImplementedException();
        }

        public void Reset()
        {
            throw new System.NotImplementedException();
        }
    }
}