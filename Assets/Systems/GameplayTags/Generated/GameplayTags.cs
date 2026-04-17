// -- AUTO-GENERATED FILE --
using GameplayTags.Runtime;
using System.Collections.Generic;

namespace GameplayTags.Generated
{
    public static class TagLibrary
    {
        public static class Ability
        {
            public static readonly Tag Self = new Tag("Ability");
            public static readonly Tag Active = new Tag("Ability.Active");
            public static readonly Tag Passive = new Tag("Ability.Passive");
        }
        public static class Cue
        {
            public static readonly Tag Self = new Tag("Cue");
            public static class Animation
            {
                public static readonly Tag Self = new Tag("Cue.Animation");
                public static class Parameter
                {
                    public static readonly Tag Self = new Tag("Cue.Animation.Parameter");
                    public static readonly Tag Grounded = new Tag("Cue.Animation.Parameter.Grounded");
                }
                public static class State
                {
                    public static readonly Tag Self = new Tag("Cue.Animation.State");
                    public static readonly Tag Death = new Tag("Cue.Animation.State.Death");
                    public static class Jump
                    {
                        public static readonly Tag Self = new Tag("Cue.Animation.State.Jump");
                        public static readonly Tag Start = new Tag("Cue.Animation.State.Jump.Start");
                    }
                }
            }
            public static class Audio
            {
                public static readonly Tag Self = new Tag("Cue.Audio");
                public static class Gun
                {
                    public static readonly Tag Self = new Tag("Cue.Audio.Gun");
                    public static readonly Tag B = new Tag("Cue.Audio.Gun.B");
                }
            }
            public static class Light
            {
                public static readonly Tag Self = new Tag("Cue.Light");
                public static class Burst
                {
                    public static readonly Tag Self = new Tag("Cue.Light.Burst");
                    public static readonly Tag Muzzle = new Tag("Cue.Light.Burst.Muzzle");
                }
            }
            public static class Prefab
            {
                public static readonly Tag Self = new Tag("Cue.Prefab");
                public static class Impact
                {
                    public static readonly Tag Self = new Tag("Cue.Prefab.Impact");
                    public static readonly Tag A = new Tag("Cue.Prefab.Impact.A");
                }
            }
            public static class VisualEffect
            {
                public static readonly Tag Self = new Tag("Cue.VisualEffect");
                public static class Dev
                {
                    public static readonly Tag Self = new Tag("Cue.VisualEffect.Dev");
                    public static readonly Tag Loop = new Tag("Cue.VisualEffect.Dev.Loop");
                }
                public static class Muzzle
                {
                    public static readonly Tag Self = new Tag("Cue.VisualEffect.Muzzle");
                    public static readonly Tag A = new Tag("Cue.VisualEffect.Muzzle.A");
                }
            }
        }
        public static class Effect
        {
            public static readonly Tag Self = new Tag("Effect");
            public static class Cost
            {
                public static readonly Tag Self = new Tag("Effect.Cost");
                public static class Ability
                {
                    public static readonly Tag Self = new Tag("Effect.Cost.Ability");
                    public static readonly Tag Dash = new Tag("Effect.Cost.Ability.Dash");
                }
            }
            public static class Modifier
            {
                public static readonly Tag Self = new Tag("Effect.Modifier");
                public static class Damage
                {
                    public static readonly Tag Self = new Tag("Effect.Modifier.Damage");
                    public static readonly Tag Kinetic = new Tag("Effect.Modifier.Damage.Kinetic");
                }
            }
        }
        public static class EquipmentSlot
        {
            public static readonly Tag Self = new Tag("EquipmentSlot");
            public static readonly Tag Core = new Tag("EquipmentSlot.Core");
            public static class Utility
            {
                public static readonly Tag Self = new Tag("EquipmentSlot.Utility");
                public static readonly Tag One = new Tag("EquipmentSlot.Utility.One");
                public static readonly Tag Two = new Tag("EquipmentSlot.Utility.Two");
            }
        }
        public static class Item
        {
            public static readonly Tag Self = new Tag("Item");
            public static class Equipment
            {
                public static readonly Tag Self = new Tag("Item.Equipment");
                public static readonly Tag EnergyCore = new Tag("Item.Equipment.EnergyCore");
            }
            public static class Modifier
            {
                public static readonly Tag Self = new Tag("Item.Modifier");
                public static readonly Tag Active = new Tag("Item.Modifier.Active");
                public static readonly Tag Passive = new Tag("Item.Modifier.Passive");
            }
        }
        public static class Mod
        {
            public static readonly Tag Self = new Tag("Mod");
            public static class Slot
            {
                public static readonly Tag Self = new Tag("Mod.Slot");
                public static class Active
                {
                    public static readonly Tag Self = new Tag("Mod.Slot.Active");
                    public static readonly Tag _1 = new Tag("Mod.Slot.Active.1");
                    public static readonly Tag _2 = new Tag("Mod.Slot.Active.2");
                }
                public static class Passive
                {
                    public static readonly Tag Self = new Tag("Mod.Slot.Passive");
                    public static readonly Tag _1 = new Tag("Mod.Slot.Passive.1");
                    public static readonly Tag _2 = new Tag("Mod.Slot.Passive.2");
                }
            }
        }
        public static class Slot
        {
            public static readonly Tag Self = new Tag("Slot");
            public static class Hip
            {
                public static readonly Tag Self = new Tag("Slot.Hip");
                public static readonly Tag Left = new Tag("Slot.Hip.Left");
                public static readonly Tag Right = new Tag("Slot.Hip.Right");
            }
        }
        public static class Status
        {
            public static readonly Tag Self = new Tag("Status");
            public static readonly Tag Aiming = new Tag("Status.Aiming");
            public static readonly Tag Dead = new Tag("Status.Dead");
            public static readonly Tag Immobilised = new Tag("Status.Immobilised");
        }
        public static class Unit
        {
            public static readonly Tag Self = new Tag("Unit");
            public static readonly Tag Player = new Tag("Unit.Player");
        }

        private static readonly List<Tag> AllTags = new List<Tag>
        {
            new Tag("Ability.Active"),
            new Tag("Ability.Passive"),
            new Tag("Cue.Animation"),
            new Tag("Cue.Animation.Parameter.Grounded"),
            new Tag("Cue.Animation.State.Death"),
            new Tag("Cue.Animation.State.Jump.Start"),
            new Tag("Cue.Audio.Gun.B"),
            new Tag("Cue.Light.Burst.Muzzle"),
            new Tag("Cue.Prefab.Impact.A"),
            new Tag("Cue.VisualEffect.Dev.Loop"),
            new Tag("Cue.VisualEffect.Muzzle.A"),
            new Tag("Effect.Cost.Ability.Dash"),
            new Tag("Effect.Modifier.Damage"),
            new Tag("Effect.Modifier.Damage.Kinetic"),
            new Tag("EquipmentSlot.Core"),
            new Tag("EquipmentSlot.Utility.One"),
            new Tag("EquipmentSlot.Utility.Two"),
            new Tag("Item.Equipment.EnergyCore"),
            new Tag("Item.Modifier.Active"),
            new Tag("Item.Modifier.Passive"),
            new Tag("Mod.Slot.Active.1"),
            new Tag("Mod.Slot.Active.2"),
            new Tag("Mod.Slot.Passive.1"),
            new Tag("Mod.Slot.Passive.2"),
            new Tag("Slot.Hip.Left"),
            new Tag("Slot.Hip.Right"),
            new Tag("Status.Aiming"),
            new Tag("Status.Dead"),
            new Tag("Status.Immobilised"),
            new Tag("Unit.Player"),
        };

        public static IReadOnlyList<Tag> GetAllTags() => AllTags;
    }
}
