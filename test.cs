using System;
using AbilitySystem.Runtime.Abilities;

class Test
{
    public void M()
    {
        Ability ability = null;
        bool b = AbilityManager.HasAuthorityToTerminate(ability, true);
    }
}
