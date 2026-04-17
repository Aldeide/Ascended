using UnityEngine;
using UnityEngine.Localization;

namespace Item.Runtime.Interface
{
    public interface IBaseItem
    {
        public string Name { get; set; }
        public LocalizedString DisplayName { get; set; }
        public LocalizedString Description { get; set; }
        public Sprite Icon { get; set; }
    }
}
