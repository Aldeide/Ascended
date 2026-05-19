// -- AUTO-GENERATED FILE --
using GameplayTags.Runtime;
using System.Collections.Generic;
using System.Linq;

namespace GameplayTags.Generated
{
    public static class TagLibrary
    {
        public class __Ability_Group
        {
            public static implicit operator Tag(__Ability_Group _) => new Tag("Ability");
            public class __Ability_Active_Group
            {
                public static implicit operator Tag(__Ability_Active_Group _) => new Tag("Ability.Active");
                public readonly Tag Dash = new Tag("Ability.Active.Dash");
                public readonly Tag Test = new Tag("Ability.Active.Test");
            }
            public readonly __Ability_Active_Group Active = new __Ability_Active_Group();
            public class __Ability_Passive_Group
            {
                public static implicit operator Tag(__Ability_Passive_Group _) => new Tag("Ability.Passive");
                public readonly Tag Shield = new Tag("Ability.Passive.Shield");
            }
            public readonly __Ability_Passive_Group Passive = new __Ability_Passive_Group();
        }
        public static readonly __Ability_Group Ability = new __Ability_Group();
        public class __Cooldown_Group
        {
            public static implicit operator Tag(__Cooldown_Group _) => new Tag("Cooldown");
            public class __Cooldown_Ability_Group
            {
                public static implicit operator Tag(__Cooldown_Ability_Group _) => new Tag("Cooldown.Ability");
                public readonly Tag Dash = new Tag("Cooldown.Ability.Dash");
            }
            public readonly __Cooldown_Ability_Group Ability = new __Cooldown_Ability_Group();
        }
        public static readonly __Cooldown_Group Cooldown = new __Cooldown_Group();
        public class __Cue_Group
        {
            public static implicit operator Tag(__Cue_Group _) => new Tag("Cue");
            public class __Cue_Animation_Group
            {
                public static implicit operator Tag(__Cue_Animation_Group _) => new Tag("Cue.Animation");
                public class __Cue_Animation_Parameter_Group
                {
                    public static implicit operator Tag(__Cue_Animation_Parameter_Group _) => new Tag("Cue.Animation.Parameter");
                    public readonly Tag Grounded = new Tag("Cue.Animation.Parameter.Grounded");
                }
                public readonly __Cue_Animation_Parameter_Group Parameter = new __Cue_Animation_Parameter_Group();
                public class __Cue_Animation_State_Group
                {
                    public static implicit operator Tag(__Cue_Animation_State_Group _) => new Tag("Cue.Animation.State");
                    public readonly Tag Death = new Tag("Cue.Animation.State.Death");
                    public readonly Tag Idle = new Tag("Cue.Animation.State.Idle");
                    public class __Cue_Animation_State_Jump_Group
                    {
                        public static implicit operator Tag(__Cue_Animation_State_Jump_Group _) => new Tag("Cue.Animation.State.Jump");
                        public readonly Tag Start = new Tag("Cue.Animation.State.Jump.Start");
                    }
                    public readonly __Cue_Animation_State_Jump_Group Jump = new __Cue_Animation_State_Jump_Group();
                    public readonly Tag Stunned = new Tag("Cue.Animation.State.Stunned");
                }
                public readonly __Cue_Animation_State_Group State = new __Cue_Animation_State_Group();
            }
            public readonly __Cue_Animation_Group Animation = new __Cue_Animation_Group();
            public class __Cue_Audio_Group
            {
                public static implicit operator Tag(__Cue_Audio_Group _) => new Tag("Cue.Audio");
                public class __Cue_Audio_Gun_Group
                {
                    public static implicit operator Tag(__Cue_Audio_Gun_Group _) => new Tag("Cue.Audio.Gun");
                    public readonly Tag B = new Tag("Cue.Audio.Gun.B");
                }
                public readonly __Cue_Audio_Gun_Group Gun = new __Cue_Audio_Gun_Group();
            }
            public readonly __Cue_Audio_Group Audio = new __Cue_Audio_Group();
            public class __Cue_IK_Group
            {
                public static implicit operator Tag(__Cue_IK_Group _) => new Tag("Cue.IK");
                public class __Cue_IK_Arms_Group
                {
                    public static implicit operator Tag(__Cue_IK_Arms_Group _) => new Tag("Cue.IK.Arms");
                    public readonly Tag Disable = new Tag("Cue.IK.Arms.Disable");
                    public readonly Tag Enable = new Tag("Cue.IK.Arms.Enable");
                }
                public readonly __Cue_IK_Arms_Group Arms = new __Cue_IK_Arms_Group();
                public class __Cue_IK_Feet_Group
                {
                    public static implicit operator Tag(__Cue_IK_Feet_Group _) => new Tag("Cue.IK.Feet");
                    public readonly Tag Disable = new Tag("Cue.IK.Feet.Disable");
                    public readonly Tag Enable = new Tag("Cue.IK.Feet.Enable");
                }
                public readonly __Cue_IK_Feet_Group Feet = new __Cue_IK_Feet_Group();
            }
            public readonly __Cue_IK_Group IK = new __Cue_IK_Group();
            public class __Cue_Light_Group
            {
                public static implicit operator Tag(__Cue_Light_Group _) => new Tag("Cue.Light");
                public class __Cue_Light_Burst_Group
                {
                    public static implicit operator Tag(__Cue_Light_Burst_Group _) => new Tag("Cue.Light.Burst");
                    public readonly Tag Muzzle = new Tag("Cue.Light.Burst.Muzzle");
                }
                public readonly __Cue_Light_Burst_Group Burst = new __Cue_Light_Burst_Group();
            }
            public readonly __Cue_Light_Group Light = new __Cue_Light_Group();
            public class __Cue_Prefab_Group
            {
                public static implicit operator Tag(__Cue_Prefab_Group _) => new Tag("Cue.Prefab");
                public class __Cue_Prefab_Impact_Group
                {
                    public static implicit operator Tag(__Cue_Prefab_Impact_Group _) => new Tag("Cue.Prefab.Impact");
                    public readonly Tag A = new Tag("Cue.Prefab.Impact.A");
                }
                public readonly __Cue_Prefab_Impact_Group Impact = new __Cue_Prefab_Impact_Group();
                public class __Cue_Prefab_Trail_Group
                {
                    public static implicit operator Tag(__Cue_Prefab_Trail_Group _) => new Tag("Cue.Prefab.Trail");
                    public readonly Tag A = new Tag("Cue.Prefab.Trail.A");
                }
                public readonly __Cue_Prefab_Trail_Group Trail = new __Cue_Prefab_Trail_Group();
            }
            public readonly __Cue_Prefab_Group Prefab = new __Cue_Prefab_Group();
            public class __Cue_VisualEffect_Group
            {
                public static implicit operator Tag(__Cue_VisualEffect_Group _) => new Tag("Cue.VisualEffect");
                public class __Cue_VisualEffect_Dev_Group
                {
                    public static implicit operator Tag(__Cue_VisualEffect_Dev_Group _) => new Tag("Cue.VisualEffect.Dev");
                    public readonly Tag Loop = new Tag("Cue.VisualEffect.Dev.Loop");
                }
                public readonly __Cue_VisualEffect_Dev_Group Dev = new __Cue_VisualEffect_Dev_Group();
                public class __Cue_VisualEffect_Muzzle_Group
                {
                    public static implicit operator Tag(__Cue_VisualEffect_Muzzle_Group _) => new Tag("Cue.VisualEffect.Muzzle");
                    public readonly Tag A = new Tag("Cue.VisualEffect.Muzzle.A");
                }
                public readonly __Cue_VisualEffect_Muzzle_Group Muzzle = new __Cue_VisualEffect_Muzzle_Group();
            }
            public readonly __Cue_VisualEffect_Group VisualEffect = new __Cue_VisualEffect_Group();
        }
        public static readonly __Cue_Group Cue = new __Cue_Group();
        public class __Data_Group
        {
            public static implicit operator Tag(__Data_Group _) => new Tag("Data");
            public class __Data_Effect_Group
            {
                public static implicit operator Tag(__Data_Effect_Group _) => new Tag("Data.Effect");
                public readonly Tag Damage = new Tag("Data.Effect.Damage");
            }
            public readonly __Data_Effect_Group Effect = new __Data_Effect_Group();
        }
        public static readonly __Data_Group Data = new __Data_Group();
        public class __Effect_Group
        {
            public static implicit operator Tag(__Effect_Group _) => new Tag("Effect");
            public class __Effect_Cost_Group
            {
                public static implicit operator Tag(__Effect_Cost_Group _) => new Tag("Effect.Cost");
                public class __Effect_Cost_Ability_Group
                {
                    public static implicit operator Tag(__Effect_Cost_Ability_Group _) => new Tag("Effect.Cost.Ability");
                    public readonly Tag Dash = new Tag("Effect.Cost.Ability.Dash");
                }
                public readonly __Effect_Cost_Ability_Group Ability = new __Effect_Cost_Ability_Group();
            }
            public readonly __Effect_Cost_Group Cost = new __Effect_Cost_Group();
            public class __Effect_Modifier_Group
            {
                public static implicit operator Tag(__Effect_Modifier_Group _) => new Tag("Effect.Modifier");
                public class __Effect_Modifier_Damage_Group
                {
                    public static implicit operator Tag(__Effect_Modifier_Damage_Group _) => new Tag("Effect.Modifier.Damage");
                    public readonly Tag Kinetic = new Tag("Effect.Modifier.Damage.Kinetic");
                }
                public readonly __Effect_Modifier_Damage_Group Damage = new __Effect_Modifier_Damage_Group();
            }
            public readonly __Effect_Modifier_Group Modifier = new __Effect_Modifier_Group();
        }
        public static readonly __Effect_Group Effect = new __Effect_Group();
        public class __EquipmentSlot_Group
        {
            public static implicit operator Tag(__EquipmentSlot_Group _) => new Tag("EquipmentSlot");
            public readonly Tag Armor = new Tag("EquipmentSlot.Armor");
            public readonly Tag Core = new Tag("EquipmentSlot.Core");
            public readonly Tag Weapon = new Tag("EquipmentSlot.Weapon");
        }
        public static readonly __EquipmentSlot_Group EquipmentSlot = new __EquipmentSlot_Group();
        public class __Item_Group
        {
            public static implicit operator Tag(__Item_Group _) => new Tag("Item");
            public class __Item_Equipment_Group
            {
                public static implicit operator Tag(__Item_Equipment_Group _) => new Tag("Item.Equipment");
                public readonly Tag Armor = new Tag("Item.Equipment.Armor");
                public readonly Tag EnergyCore = new Tag("Item.Equipment.EnergyCore");
                public readonly Tag Weapon = new Tag("Item.Equipment.Weapon");
            }
            public readonly __Item_Equipment_Group Equipment = new __Item_Equipment_Group();
            public class __Item_Modifier_Group
            {
                public static implicit operator Tag(__Item_Modifier_Group _) => new Tag("Item.Modifier");
                public readonly Tag Active = new Tag("Item.Modifier.Active");
                public readonly Tag Passive = new Tag("Item.Modifier.Passive");
            }
            public readonly __Item_Modifier_Group Modifier = new __Item_Modifier_Group();
        }
        public static readonly __Item_Group Item = new __Item_Group();
        public class __Mod_Group
        {
            public static implicit operator Tag(__Mod_Group _) => new Tag("Mod");
            public class __Mod_Slot_Group
            {
                public static implicit operator Tag(__Mod_Slot_Group _) => new Tag("Mod.Slot");
                public class __Mod_Slot_Active_Group
                {
                    public static implicit operator Tag(__Mod_Slot_Active_Group _) => new Tag("Mod.Slot.Active");
                    public readonly Tag _1 = new Tag("Mod.Slot.Active.1");
                    public readonly Tag _2 = new Tag("Mod.Slot.Active.2");
                }
                public readonly __Mod_Slot_Active_Group Active = new __Mod_Slot_Active_Group();
                public class __Mod_Slot_Passive_Group
                {
                    public static implicit operator Tag(__Mod_Slot_Passive_Group _) => new Tag("Mod.Slot.Passive");
                    public readonly Tag _1 = new Tag("Mod.Slot.Passive.1");
                    public readonly Tag _2 = new Tag("Mod.Slot.Passive.2");
                }
                public readonly __Mod_Slot_Passive_Group Passive = new __Mod_Slot_Passive_Group();
            }
            public readonly __Mod_Slot_Group Slot = new __Mod_Slot_Group();
        }
        public static readonly __Mod_Group Mod = new __Mod_Group();
        public class __Slot_Group
        {
            public static implicit operator Tag(__Slot_Group _) => new Tag("Slot");
            public class __Slot_Hip_Group
            {
                public static implicit operator Tag(__Slot_Hip_Group _) => new Tag("Slot.Hip");
                public readonly Tag Left = new Tag("Slot.Hip.Left");
                public readonly Tag Right = new Tag("Slot.Hip.Right");
            }
            public readonly __Slot_Hip_Group Hip = new __Slot_Hip_Group();
        }
        public static readonly __Slot_Group Slot = new __Slot_Group();
        public class __Status_Group
        {
            public static implicit operator Tag(__Status_Group _) => new Tag("Status");
            public readonly Tag Aiming = new Tag("Status.Aiming");
            public readonly Tag Dead = new Tag("Status.Dead");
            public class __Status_Debuff_Group
            {
                public static implicit operator Tag(__Status_Debuff_Group _) => new Tag("Status.Debuff");
                public readonly Tag Stun = new Tag("Status.Debuff.Stun");
            }
            public readonly __Status_Debuff_Group Debuff = new __Status_Debuff_Group();
            public readonly Tag Immobilised = new Tag("Status.Immobilised");
        }
        public static readonly __Status_Group Status = new __Status_Group();
        public class __Test_Group
        {
            public static implicit operator Tag(__Test_Group _) => new Tag("Test");
            public readonly Tag Test = new Tag("Test.Test");
        }
        public static readonly __Test_Group Test = new __Test_Group();
        public class __Unit_Group
        {
            public static implicit operator Tag(__Unit_Group _) => new Tag("Unit");
            public readonly Tag Player = new Tag("Unit.Player");
        }
        public static readonly __Unit_Group Unit = new __Unit_Group();

        private static readonly List<Tag> AllTags = new List<Tag>
        {
            new Tag("Ability.Active"),
            new Tag("Ability.Active.Dash"),
            new Tag("Ability.Active.Test"),
            new Tag("Ability.Passive"),
            new Tag("Ability.Passive.Shield"),
            new Tag("Cooldown.Ability.Dash"),
            new Tag("Cue.Animation"),
            new Tag("Cue.Animation.Parameter.Grounded"),
            new Tag("Cue.Animation.State.Death"),
            new Tag("Cue.Animation.State.Idle"),
            new Tag("Cue.Animation.State.Jump.Start"),
            new Tag("Cue.Animation.State.Stunned"),
            new Tag("Cue.Audio.Gun.B"),
            new Tag("Cue.IK"),
            new Tag("Cue.IK.Arms.Disable"),
            new Tag("Cue.IK.Arms.Enable"),
            new Tag("Cue.IK.Feet.Disable"),
            new Tag("Cue.IK.Feet.Enable"),
            new Tag("Cue.Light.Burst.Muzzle"),
            new Tag("Cue.Prefab"),
            new Tag("Cue.Prefab.Impact.A"),
            new Tag("Cue.Prefab.Trail.A"),
            new Tag("Cue.VisualEffect.Dev.Loop"),
            new Tag("Cue.VisualEffect.Muzzle.A"),
            new Tag("Data.Effect.Damage"),
            new Tag("Effect.Cost.Ability.Dash"),
            new Tag("Effect.Modifier.Damage"),
            new Tag("Effect.Modifier.Damage.Kinetic"),
            new Tag("EquipmentSlot.Armor"),
            new Tag("EquipmentSlot.Core"),
            new Tag("EquipmentSlot.Weapon"),
            new Tag("Item.Equipment.Armor"),
            new Tag("Item.Equipment.EnergyCore"),
            new Tag("Item.Equipment.Weapon"),
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
            new Tag("Status.Debuff.Stun"),
            new Tag("Status.Immobilised"),
            new Tag("Test.Test"),
            new Tag("Unit.Player"),
        };

        public static IReadOnlyList<Tag> GetAllTags() => AllTags;
    }
}
