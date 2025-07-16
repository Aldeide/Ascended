using System.Collections.Generic;
using AvatarSlots.Runtime;
using UnityEngine;

namespace AvatarSlots.Scripts
{
    // This component needs to be added to any gameObject that can be slotted into an Avatar.
    public class SlottableComponent : MonoBehaviour
    {
        public List<SlottableConfiguration> SlottableConfigurations;
    }
}