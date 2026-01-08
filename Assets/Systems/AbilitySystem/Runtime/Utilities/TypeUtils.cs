using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AbilitySystem.Runtime.Utilities
{
    public static class TypeUtil
    {
        public static Type[] GetAllInheritedTypesOf(Type parentType)
        {
            List<Type> inheritedTypes = new List<Type>();
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var assembly in assemblies)
            {
                try
                {
                    var types = assembly.GetTypes();

                    inheritedTypes.AddRange(types.Where(type => type.IsSubclassOf(parentType) && !type.IsAbstract));
                }
                catch (ReflectionTypeLoadException)
                {
                }
            }

            return inheritedTypes.ToArray();
        }

        public static Type FindTypeInAllAssemblies(string typeName)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    Type type = assembly.GetType(typeName);
                    if (type != null)
                    {
                        return type;
                    }
                }
                catch (ReflectionTypeLoadException)
                {
                }
            }
            return null;
        }
        
        public static string[] GetInheritanceChain(this Type type, bool fullName = true)
        {
            var inheritanceChain = new List<string>();
            var currentType = type;

            while (currentType != null)
            {
                var name = fullName ? currentType.FullName : currentType.Name;
                inheritanceChain.Add(name);
                currentType = currentType.BaseType;
            }

            return inheritanceChain.ToArray();
        }
    }
}