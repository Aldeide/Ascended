using System.Collections.Generic;
using Item.Runtime.Definition;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Item.Runtime.Database
{
    // Singleton class that loads all items into a database.
    public class ItemLibrary : MonoBehaviour
    {
        private static ItemLibrary _instance;
        public static ItemLibrary Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindAnyObjectByType<ItemLibrary>();
                    if (_instance == null)
                    {
                        var go = new GameObject("ItemLibrary");
                        _instance = go.AddComponent<ItemLibrary>();
                    }
                }
                return _instance;
            }
            private set => _instance = value;
        }

        [ShowInInspector] private Dictionary<string, ItemDefinition> _items = new();
        
        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);
            foreach (var item in Resources.LoadAll<ItemDefinition>(""))
            {
                if (item == null) continue;
                if (!string.IsNullOrEmpty(item.Name))
                {
                    _items.TryAdd(item.Name, item);
                }
                _items.TryAdd(item.name, item);
            }
        }

        public ItemDefinition GetItemByName(string itemName)
        {
            return _items.GetValueOrDefault(itemName);
        }
    }
}