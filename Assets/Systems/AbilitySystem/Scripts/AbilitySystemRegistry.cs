using System.Collections.Generic;
using UnityEngine;

namespace AbilitySystem.Scripts
{
    public static class AbilitySystemRegistry
    {
        public static readonly HashSet<AbilitySystemComponent> AllComponents = new HashSet<AbilitySystemComponent>();

        public static void Register(AbilitySystemComponent component)
        {
            if (component != null)
                AllComponents.Add(component);
        }

        public static void Unregister(AbilitySystemComponent component)
        {
            if (component != null)
                AllComponents.Remove(component);
        }
    }
}
