using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.ItemSystem.Runtime.Interface
{
    public interface IRessource : IBaseItem
    {
        public int StackSize { get; }
    }
}
