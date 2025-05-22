using System.Collections.Generic;
using AbilitySystem.Runtime.Tags;

namespace AbilitySystemExtension.Runtime.Tags
{
    public static class CueTagLibrary
    {
        public static GameplayTag CueAnimation { get; } = new("Cue.Animation");
        public static GameplayTag CueAnimationParameterGrounded { get; } = new("Cue.Animation.Parameter.Grounded");
        public static GameplayTag CueAnimationState { get; } = new("Cue.Animation.State");
        public static GameplayTag CueAnimationStatePlayerDeath { get; } = new("Cue.Animation.State.Player.Death");
        public static GameplayTag CueAnimationStateJumpStart { get; } = new("Cue.Animation.State.Jump.Start");
        public static GameplayTag CueAudioGunA { get; } = new("Cue.Audio.Gun.A");
        public static GameplayTag CueVisualEffectMuzzleA { get; } = new("Cue.VisualEffect.Muzzle.A");
        public static GameplayTag CuePrefabImpactA { get; } = new("Cue.Prefab.Impact.A");

        public static Dictionary<string, GameplayTag> TagMap = new()
        {
            ["Cue.Animation"] = CueAnimation,
            ["Cue.Animation.Parameter.Grounded"] = CueAnimationParameterGrounded,
            ["Cue.Animation.State"] = CueAnimationState,
            ["Cue.Animation.State.Player.Death"] = CueAnimationStatePlayerDeath,
            ["Cue.Animation.State.Jump.Start"] = CueAnimationStateJumpStart,
            ["Cue.Audio.Gun.A"] = CueAudioGunA,
            ["Cue.VisualEffect.Muzzle.A"] = CueVisualEffectMuzzleA,
            ["Cue.Prefab.Impact.A"] = CuePrefabImpactA
        };
    }
}