// -- AUTO-GENERATED FILE --
using GameplayTags.Runtime;
using System.Collections.Generic;

namespace GameplayTags.Generated
{
    public static class TagLibrary
    {
        public static readonly Tag AbilityActive = new Tag("Ability.Active");
        public static readonly Tag AbilityPassive = new Tag("Ability.Passive");
        public static readonly Tag CueAnimation = new Tag("Cue.Animation");
        public static readonly Tag CueAnimationParameterGrounded = new Tag("Cue.Animation.Parameter.Grounded");
        public static readonly Tag CueAnimationStateDeath = new Tag("Cue.Animation.State.Death");
        public static readonly Tag CueAnimationStateJumpStart = new Tag("Cue.Animation.State.Jump.Start");
        public static readonly Tag CueAudioGunB = new Tag("Cue.Audio.Gun.B");
        public static readonly Tag CueLightBurstMuzzle = new Tag("Cue.Light.Burst.Muzzle");
        public static readonly Tag CuePrefabImpactA = new Tag("Cue.Prefab.Impact.A");
        public static readonly Tag CueVisualEffectDevLoop = new Tag("Cue.VisualEffect.Dev.Loop");
        public static readonly Tag CueVisualEffectMuzzleA = new Tag("Cue.VisualEffect.Muzzle.A");
        public static readonly Tag EffectCostAbilityDash = new Tag("Effect.Cost.Ability.Dash");
        public static readonly Tag EquipmentSlotCore = new Tag("EquipmentSlot.Core");
        public static readonly Tag EquipmentSlotUtilityOne = new Tag("EquipmentSlot.Utility.One");
        public static readonly Tag EquipmentSlotUtilityTwo = new Tag("EquipmentSlot.Utility.Two");
        public static readonly Tag SlotHipLeft = new Tag("Slot.Hip.Left");
        public static readonly Tag SlotHipRight = new Tag("Slot.Hip.Right");
        public static readonly Tag StatusAiming = new Tag("Status.Aiming");
        public static readonly Tag StatusDead = new Tag("Status.Dead");
        public static readonly Tag StatusImmobilised = new Tag("Status.Immobilised");
        public static readonly Tag UnitPlayer = new Tag("Unit.Player");

        private static readonly List<Tag> AllTags = new List<Tag>
        {
            AbilityActive,
            AbilityPassive,
            CueAnimation,
            CueAnimationParameterGrounded,
            CueAnimationStateDeath,
            CueAnimationStateJumpStart,
            CueAudioGunB,
            CueLightBurstMuzzle,
            CuePrefabImpactA,
            CueVisualEffectDevLoop,
            CueVisualEffectMuzzleA,
            EffectCostAbilityDash,
            EquipmentSlotCore,
            EquipmentSlotUtilityOne,
            EquipmentSlotUtilityTwo,
            SlotHipLeft,
            SlotHipRight,
            StatusAiming,
            StatusDead,
            StatusImmobilised,
            UnitPlayer,
        };

        public static IReadOnlyList<Tag> GetAllTags()
        {
            return AllTags;
        }
    }
}
