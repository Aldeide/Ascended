using AbilitySystem.Runtime.Core;
using System.Reflection;

namespace AbilityGraph.Tests.Runtime
{
    /// <summary>
    /// Utility methods for testing Ability Graph nodes, providing access to internal/protected members.
    /// </summary>
    public static class AbilityGraphTestUtilities
    {
        /// <summary>
        /// Injects an IAbilitySystem owner into an AbilityNode using reflection.
        /// </summary>
        public static void InjectOwner(object node, IAbilitySystem owner)
        {
            var field = typeof(AbilityGraph.Runtime.Nodes.Base.AbilityNode).GetField("Owner", BindingFlags.NonPublic | BindingFlags.Instance);
            if (field != null)
            {
                field.SetValue(node, owner);
            }
        }

        /// <summary>
        /// Invokes the protected Process method on a node using reflection.
        /// </summary>
        public static void InvokeProcess(object node)
        {
            // Try "Process" first (most common in our implementation)
            var method = node.GetType().GetMethod("Process", BindingFlags.NonPublic | BindingFlags.Instance);
            
            // If not found, try "OnProcess" (sometimes used in BaseNode variations)
            if (method == null)
            {
                method = node.GetType().GetMethod("OnProcess", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            }

            if (method != null)
            {
                method.Invoke(node, null);
            }
        }
    }
}
