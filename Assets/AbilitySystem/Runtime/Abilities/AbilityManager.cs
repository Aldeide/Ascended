using System.Collections.Generic;
using AbilitySystem.Runtime.Core;
using AbilitySystem.Runtime.Networking;

namespace AbilitySystem.Runtime.Abilities
{
    public class AbilityManager
    {
        private IAbilitySystem _owner;
        private Dictionary<string, Ability> _abilities;
        
        public AbilityManager(IAbilitySystem owner)
        {
            _owner = owner;
            _abilities = new Dictionary<string, Ability>();
        }

        public void TryActivateAbility(string name, params object[] args)
        {
            if (_abilities.TryGetValue(name, out Ability ability))
            {
                if (_owner.IsLocalClient() && ability.Definition.Asset.networkPolicy == AbilityNetworkPolicy.Server)
                {
                    return;
                }

                if (_owner.IsLocalClient() &&
                    ability.Definition.Asset.networkPolicy == AbilityNetworkPolicy.ClientPredicted)
                {
                    
                }
                ability.TryActivateAbility(args);
            }
        }

        public void NotifyAbilityActivationFailed(PredictionKey key)
        {
            
        }
    }
}