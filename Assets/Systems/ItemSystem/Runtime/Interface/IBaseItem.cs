using UnityEngine.Localization;

namespace ItemSystem.Runtime.Interface
{
    public interface IBaseItem
    {
        public string Name { get; set; }
        public LocalizedString DisplayName { get; set; }
        public LocalizedString Description { get; set; }
    }
}
