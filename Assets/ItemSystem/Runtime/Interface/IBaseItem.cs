using UnityEngine.Localization;

namespace ItemSystem.Runtime.Interface
{
    public interface IBaseItem
    {
        public LocalizedString DisplayName { get; set; }
        public LocalizedString Description { get; set; }
    }
}
