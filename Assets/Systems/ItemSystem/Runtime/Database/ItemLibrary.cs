using System.Collections.Generic;
using ItemSystem.Runtime.Definition;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ItemSystem.Runtime.Database
{
    // Singleton class that loads all items into a database.
    public class ItemLibrary : MonoBehaviour
    {
        public static ItemLibrary Instance { get; private set; }
        [ShowInInspector] private Dictionary<string, ItemDefinition> _items = new();
        
        private void Awake()
        {
            if (Instance && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            foreach (var item in Resources.LoadAll<ItemDefinition>(""))
            {
                _items.Add(item.name, item);
            }
            
        }

        public ItemDefinition GetItemByName(string itemName)
        {
            return _items.GetValueOrDefault(itemName);
        }
    }
}