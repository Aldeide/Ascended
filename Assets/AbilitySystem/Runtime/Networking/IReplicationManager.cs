using AbilitySystem.Runtime.Attributes;

namespace AbilitySystem.Runtime.Networking
{
    public interface IReplicationManager
    {
        public void NotifyClientsAttributeBaseValueChanged(Attribute attribute, float oldValue, float newValue);
        public void OnAttributeBaseValueChanged(string attributeName, float newValue);
    }
}