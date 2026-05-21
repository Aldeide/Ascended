using System.Collections.Generic;
using UnityEngine;

namespace Item.Runtime.Definition
{
    [CreateAssetMenu(fileName = "StartingEquipmentDefinition", menuName = "EquipmentSystem/StartingEquipmentDefinition")]
    public class StartingEquipmentDefinition : ScriptableObject
    {
        public List<EquipmentDefinition> StartingEquipment;
    }
}
