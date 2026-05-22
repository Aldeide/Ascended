using AISystem.Runtime.Actions;
using AISystem.Runtime.Goals;
using AISystem.Runtime.Sensors;
using AISystem.Runtime.TargetKeys;
using AISystem.Runtime.WorldKeys;
using AISystem.Runtime.DecisionMakers;
using CrashKonijn.Goap.Core;
using CrashKonijn.Goap.Runtime;

namespace AISystem.Runtime.Capabilities
{
    public class CombatTacticsCapabilityFactory : CapabilityFactoryBase
    {
        public override ICapabilityConfig Create()
        {
            var builder = new CapabilityBuilder("CombatTacticsCapability");

            // Goals
            builder.AddGoal<KillEnemyGoal>()
                .AddCondition<TargetDead>(Comparison.GreaterThanOrEqual, 1)
                .SetBaseCost(1);

            builder.AddGoal<SurvivalGoal>()
                .AddCondition<HealthLow>(Comparison.SmallerThanOrEqual, 0)
                .SetBaseCost(5);

            builder.AddGoal<HealAllyGoal>()
                .AddCondition<AllyNeedsHealing>(Comparison.SmallerThanOrEqual, 0)
                .SetBaseCost(3);

            // Actions

            // Melee Attack Action
            builder.AddAction<GoapAbilityAction>()
                .SetProperties(new GoapAbilityAction.PropertiesClass { AbilityName = "MeleeAttack" })
                .SetTarget<EnemyTarget>()
                .AddCondition<TargetInMeleeRange>(Comparison.GreaterThanOrEqual, 1)
                .AddEffect<TargetDead>(EffectType.Increase)
                .SetStoppingDistance(1.5f);

            // Ranged Attack Action
            builder.AddAction<GoapAbilityAction>()
                .SetProperties(new GoapAbilityAction.PropertiesClass { AbilityName = "RangedAttack" })
                .SetTarget<EnemyTarget>()
                .AddCondition<TargetInRangedRange>(Comparison.GreaterThanOrEqual, 1)
                .AddEffect<TargetDead>(EffectType.Increase)
                .SetStoppingDistance(15f);

            // Ranged Flank Attack Action (requires being in flanking position)
            builder.AddAction<GoapAbilityAction>()
                .SetProperties(new GoapAbilityAction.PropertiesClass { AbilityName = "RangedAttack" })
                .SetTarget<EnemyTarget>()
                .AddCondition<IsRoleFlanker>(Comparison.GreaterThanOrEqual, 1)
                .AddCondition<IsFlanking>(Comparison.GreaterThanOrEqual, 1)
                .AddCondition<TargetInRangedRange>(Comparison.GreaterThanOrEqual, 1)
                .AddEffect<TargetDead>(EffectType.Increase)
                .SetStoppingDistance(15f);

            // Take Cover Action (to satisfy survival / avoid low health)
            builder.AddAction<TakeCoverAction>()
                .SetTarget<CoverTarget>()
                .AddEffect<HasCover>(EffectType.Increase)
                .SetStoppingDistance(0.5f);

            // Move to Flank Action
            builder.AddAction<MoveToFlankAction>()
                .SetTarget<FlankTarget>()
                .AddCondition<IsRoleFlanker>(Comparison.GreaterThanOrEqual, 1)
                .AddEffect<IsFlanking>(EffectType.Increase)
                .SetStoppingDistance(0.5f);

            // Heal Self Action (requires being in cover to heal)
            builder.AddAction<GoapAbilityAction>()
                .SetProperties(new GoapAbilityAction.PropertiesClass { AbilityName = "HealSelf" })
                .SetTarget<EnemyTarget>()
                .AddCondition<HasCover>(Comparison.GreaterThanOrEqual, 1)
                .AddCondition<HealthLow>(Comparison.GreaterThanOrEqual, 1)
                .AddEffect<HealthLow>(EffectType.Decrease)
                .SetStoppingDistance(0.5f)
                .SetRequiresTarget(false);

            // Heal Ally Action
            builder.AddAction<GoapAbilityAction>()
                .SetProperties(new GoapAbilityAction.PropertiesClass { AbilityName = "HealAlly" })
                .SetTarget<HealTarget>()
                .AddCondition<AllyNeedsHealing>(Comparison.GreaterThanOrEqual, 1)
                .AddEffect<AllyNeedsHealing>(EffectType.Decrease)
                .SetStoppingDistance(10f);

            // Target Sensors
            builder.AddTargetSensor<EnemyTargetSensor>()
                .SetTarget<EnemyTarget>();

            builder.AddTargetSensor<TacticalPositionSensor>()
                .SetTarget<CoverTarget>()
                .SetCallback(s => s.PreferFlanking = false);

            builder.AddTargetSensor<TacticalPositionSensor>()
                .SetTarget<FlankTarget>()
                .SetCallback(s => s.PreferFlanking = true);

            builder.AddTargetSensor<HealTargetSensor>()
                .SetTarget<HealTarget>();

            // World Sensors
            builder.AddWorldSensor<TargetDeadSensor>()
                .SetKey<TargetDead>();

            builder.AddWorldSensor<HealthLowSensor>()
                .SetKey<HealthLow>();

            builder.AddWorldSensor<AllyNeedsHealingSensor>()
                .SetKey<AllyNeedsHealing>();

            builder.AddWorldSensor<RangeSensor>()
                .SetKey<TargetInMeleeRange>()
                .SetCallback(s => { s.MinRange = 0f; s.MaxRange = 2f; });

            builder.AddWorldSensor<RangeSensor>()
                .SetKey<TargetInRangedRange>()
                .SetCallback(s => { s.MinRange = 0f; s.MaxRange = 15f; });

            builder.AddWorldSensor<RoleSensor>()
                .SetKey<IsRoleVanguard>()
                .SetCallback(s => s.TargetRole = EnemyRole.Vanguard);

            builder.AddWorldSensor<RoleSensor>()
                .SetKey<IsRoleFlanker>()
                .SetCallback(s => s.TargetRole = EnemyRole.Flanker);

            builder.AddWorldSensor<RoleSensor>()
                .SetKey<IsRoleSupport>()
                .SetCallback(s => s.TargetRole = EnemyRole.Support);

            return builder.Build();
        }
    }
}
