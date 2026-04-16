using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.Core;
using AbilitySystem.Runtime.Cues;
using UnityEngine;

namespace AbilitySystemExtension.Runtime.Abilities
{
    public class DeathAbility : Ability
    {
        private float _respawnTimer = 5f;
        private float _currentTimer = 0;
        public DeathAbility(AbilityDefinition ability, IAbilitySystem owner) : base(ability, owner)
        {
        }

        protected override void ActivateAbility(AbilityData data)
        {
            _currentTimer = 0;
            Debug.Log("Death ability activated");
            if (Owner.IsServer())
            {
                var test = new CueData();
                test.VectorData = new[] {Vector3.one, Vector3.one, Vector3.one};
                Owner.PlayCue("Cue.Animation.State.Player.Death", test, false);
            }
        }

        protected override void AbilityTick()
        {
            _currentTimer += Time.deltaTime;
            if (_currentTimer < _respawnTimer)
            {
                return;
            }

            if (Owner.NetworkRole != null) ((UnityEngine.Component)Owner.NetworkRole).transform.position = new Vector3(0, 0, 0);
            Owner.Reset();
            TryEndAbility();
        }

        public override void EndAbility()
        {
            RemoveTags();
            Debug.Log("Death ability ended");
        }
    }
}