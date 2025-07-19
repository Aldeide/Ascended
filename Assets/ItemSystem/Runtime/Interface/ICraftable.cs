using System.Collections.Generic;

namespace ItemSystem.Runtime.Interface
{
    internal interface ICraftable
    {
        public List<RecipeItem> Recipe { get; set; }
        
        public bool HasRecipeItems { get; }
    }

    public struct RecipeItem
    {
        public ScalableFloat.Runtime.ScalableFloat Amount;
        public IBaseItem Item;
    }
}
