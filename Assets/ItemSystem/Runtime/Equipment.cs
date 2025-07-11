using ItemSystem.Runtime.Definition;
using ItemSystem.Runtime.Manager;

namespace ItemSystem.Runtime
{
    public class Equipment
    {
        private EquipmentDefinition _definition;
        private EquipmentManager _manager;
        
        public Equipment(EquipmentDefinition definition, EquipmentManager manager)
        {
            _definition = definition;
            _manager = manager;
        }
        
        public void Equip()
        {
            foreach (var ability in _definition.GrantedAbilities)
            {
                _manager.GetOwner().AbilityManager.GrantAbility(ability);
            }
        }
        
        public void Unequip()
        {
            foreach (var ability in _definition.GrantedAbilities)
            {
                _manager.GetOwner().AbilityManager.RemoveAbility(ability);
            }
        }
    }
}