using System;

namespace Systems.GameplayModifier.Runtime
{
    [Serializable]
    public class GameplayModifier
    {
        public bool IsActive { get; private set; }
        
        private GameplayModifierDefinition _definition;
        
        public GameplayModifier(GameplayModifierDefinition definition)
        {
            _definition = definition;
        }
        
        public void Enable()
        {
            IsActive = true;
        }
        
        public void Disable()
        {
            IsActive = false;
        }

        public string GetName()
        {
            return _definition.NameKey.ToString();
        }
        
        public string GetDescription()
        {
            return _definition.DescriptionKey.ToString();
        }
    }
}