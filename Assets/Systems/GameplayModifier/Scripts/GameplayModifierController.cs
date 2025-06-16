using System.Collections.Generic;
using Systems.GameplayModifier.Runtime;
using Unity.Netcode;
using UnityEngine;

namespace Systems.GameplayModifier.Scripts
{
    public class GameplayModifierController : NetworkBehaviour
    {
        public List<GameplayModifierDefinition> AvailableModifiers = new();

        private void Start()
        {
            var modifiers = Resources.LoadAll<GameplayModifierDefinition>("");
            foreach (var modifier in modifiers)
            {
                availableModifiers.Add(modifier);
            }
        }
    }
}