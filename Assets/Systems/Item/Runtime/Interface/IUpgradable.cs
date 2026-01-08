using System.Collections.Generic;
using Item.Runtime.Definition;

namespace Item.Runtime.Interface
{
    public interface IUpgradable
    {
        public int Level { get; set; }
        public int MaxLevel { get; set; }
        public Dictionary<ItemDefinition, ScalableFloat.Runtime.ScalableFloat> UpgradeCosts { get; set; }
        public Dictionary<ItemDefinition, int> NextUpgradeCosts { get; set; }
        public void Upgrade();
        public bool CanUpgrade();
    }
}