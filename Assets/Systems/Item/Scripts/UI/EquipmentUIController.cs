using System.Collections.Generic;
using GameplayTags.Runtime;
using Item.Runtime.Manager;
using Item.Scripts;
using UnityEngine;
using UnityEngine.UIElements;

namespace Item.Scripts.UI
{
    public class EquipmentUIController : MonoBehaviour
    {
        [Header("UI Document")]
        [SerializeField] private UIDocument uiDocument;
        
        private VisualElement _root;
        private Dictionary<string, VisualElement> _slotElements = new();
        
        private InventoryComponent _localPlayerInventory;
        private EquipmentManager _equipmentManager;

        private bool InitializeUI()
        {
            if (_slotElements.Count > 0) return true;
            if (uiDocument == null) return false;

            _root = uiDocument.rootVisualElement;
            if (_root == null) return false;
            
            // Map the slots from UXML
            RegisterSlot("slot-head");
            RegisterSlot("slot-chest");
            RegisterSlot("slot-legs");
            RegisterSlot("slot-mainhand");
            RegisterSlot("slot-offhand");

            return _slotElements.Count > 0;
        }

        private void RegisterSlot(string name)
        {
            var element = _root.Q<VisualElement>(name);
            if (element != null)
            {
                _slotElements[name] = element;
            }
        }

        public void Initialize(InventoryComponent inventory, EquipmentManager equipmentManager)
        {
            if (equipmentManager == null)
            {
                Debug.LogWarning("EquipmentUIController: EquipmentManager is null. Cannot initialize UI yet.");
                return;
            }

            if (!InitializeUI()) return;
            
            _localPlayerInventory = inventory;
            _equipmentManager = equipmentManager;
            
            _equipmentManager.OnEquipmentChanged += RefreshEquipment;
            RefreshEquipment();
        }

        private void OnDisable()
        {
            if (_equipmentManager != null)
            {
                _equipmentManager.OnEquipmentChanged -= RefreshEquipment;
            }
        }

        private void RefreshEquipment()
        {
            if (!InitializeUI()) return;
            if (_equipmentManager == null) return;
            
            var equipment = _equipmentManager.GetEquipment();
            
            foreach (var kvp in equipment)
            {
                var slotTag = kvp.Key;
                var item = kvp.Value;
                
                // Convert tag name to UXML ID format (e.g., EquipmentSlot.Head -> slot-head)
                string uxmlId = TagToUxmlId(slotTag);
                
                if (_slotElements.TryGetValue(uxmlId, out var slotElement))
                {
                    if (item != null && item.Icon != null)
                    {
                        slotElement.style.backgroundImage = new StyleBackground(item.Icon);
                        slotElement.style.opacity = 1.0f;
                    }
                    else
                    {
                        slotElement.style.backgroundImage = null;
                        slotElement.style.opacity = 0.5f; // Placeholder look
                    }
                }
            }
        }

        private string TagToUxmlId(Tag tag)
        {
            // Simple mapping: EquipmentSlot.Head -> slot-head
            string tagName = tag.ToString().ToLower();
            if (tagName.Contains("head")) return "slot-head";
            if (tagName.Contains("chest")) return "slot-chest";
            if (tagName.Contains("legs")) return "slot-legs";
            if (tagName.Contains("mainhand")) return "slot-mainhand";
            if (tagName.Contains("offhand")) return "slot-offhand";
            return "";
        }
    }
}
