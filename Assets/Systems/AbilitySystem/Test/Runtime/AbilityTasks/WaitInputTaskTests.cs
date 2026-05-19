using System;
using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.AbilityTasks;
using AbilitySystem.Runtime.Core;
using AbilitySystem.Test.Utilities;
using NUnit.Framework;

namespace AbilitySystem.Test.Runtime.AbilityTasks
{
    /// <summary>
    /// Unit tests verifying WaitInputPressTask and WaitInputReleaseTask behavior under local input events.
    /// </summary>
    public class WaitInputTaskTests : AbilitySystemTestBase
    {
        public class InputTestAbility : Ability
        {
            public int InputPressFired = 0;
            public int InputReleaseFired = 0;

            public InputTestAbility(AbilityDefinition definition, IAbilitySystem owner) 
                : base(definition, owner)
            {
            }

            protected override void ActivateAbility(AbilityData data)
            {
            }

            public void StartInputPressTask()
            {
                var task = WaitInputPressTask.CreateWaitInputPress(this);
                task.OnPressed += () => InputPressFired++;
                task.ReadyForActivation();
            }

            public void StartInputReleaseTask()
            {
                var task = WaitInputReleaseTask.CreateWaitInputRelease(this);
                task.OnReleased += () => InputReleaseFired++;
                task.ReadyForActivation();
            }

            public override void EndAbility()
            {
            }
        }

        public class InputTestAbilityDefinition : AbilityDefinition
        {
            public InputTestAbilityDefinition()
            {
                UniqueName = "InputTestAbility";
                NetworkPolicy = AbilityNetworkPolicy.ClientPredicted;
            }

            public override Type AbilityType() => typeof(InputTestAbility);

            public override Ability ToAbility(IAbilitySystem owner)
            {
                return new InputTestAbility(this, owner);
            }
        }

        protected override bool AddDefaultAttributes => false;

        protected override void InitializeMocks()
        {
            if (SourceMock == null) SourceMock = AbilitySystemUtilities.CreateMockClientAbilitySystem(AddDefaultAttributes);
            if (TargetMock == null) TargetMock = AbilitySystemUtilities.CreateMockServerAbilitySystem(AddDefaultAttributes);
            AbilitySystemUtilities.LinkAbilitySystems(SourceMock, TargetMock);
        }

        [Test]
        public void InputTasks_WaitInputPress_And_WaitInputRelease_FiresCallbackOnEvents()
        {
            var def = new InputTestAbilityDefinition();
            var clientAbility = Source.AbilityManager.GrantAbility(def) as InputTestAbility;
            var serverAbility = Target.AbilityManager.GrantAbility(def) as InputTestAbility;

            Source.AbilityManager.TryActivateAbility(def.UniqueName);

            Assert.IsTrue(clientAbility.IsActive, "Ability must be active for input detection");

            clientAbility.StartInputPressTask();
            clientAbility.StartInputReleaseTask();

            Assert.AreEqual(0, clientAbility.InputPressFired);
            Assert.AreEqual(0, clientAbility.InputReleaseFired);

            // Press input
            Source.AbilityManager.AbilityLocalInputPressed(def.UniqueName);
            Assert.AreEqual(1, clientAbility.InputPressFired);
            Assert.AreEqual(0, clientAbility.InputReleaseFired);

            // Release input
            Source.AbilityManager.AbilityLocalInputReleased(def.UniqueName);
            Assert.AreEqual(1, clientAbility.InputPressFired);
            Assert.AreEqual(1, clientAbility.InputReleaseFired);
        }
    }
}
