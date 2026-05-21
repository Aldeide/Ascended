using System.Collections.Generic;
using GameplayTags.Runtime;
using Item.Runtime.Manager;
using Item.Scripts;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace Item.Scripts.UI
{
    public class EquipmentUIController : NetworkBehaviour
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
            
            // Unsubscribe from previous equipment manager to avoid memory leaks/double-subscription
            if (_equipmentManager != null)
            {
                _equipmentManager.OnEquipmentChanged -= RefreshEquipment;
            }

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

        private string GetPlaceholderText(string uxmlId)
        {
            return uxmlId switch
            {
                "slot-head" => "HEAD",
                "slot-chest" => "CHEST",
                "slot-legs" => "LEGS",
                "slot-mainhand" => "WEAPON",
                "slot-offhand" => "OFFHAND",
                _ => ""
            };
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
                    var label = slotElement.Q<Label>();
                    
                    if (item != null)
                    {
                        if (item.Icon != null)
                        {
                            slotElement.style.backgroundImage = new StyleBackground(item.Icon);
                            slotElement.style.opacity = 1.0f;
                            if (label != null)
                            {
                                label.style.display = DisplayStyle.None;
                            }
                        }
                        else
                        {
                            slotElement.style.backgroundImage = null;
                            slotElement.style.opacity = 1.0f;
                            if (label != null)
                            {
                                label.text = item.Name;
                                label.style.display = DisplayStyle.Flex;
                            }
                        }
                    }
                    else
                    {
                        slotElement.style.backgroundImage = null;
                        slotElement.style.opacity = 1.0f; // Keep background slot styling clear
                        if (label != null)
                        {
                            label.text = GetPlaceholderText(uxmlId);
                            label.style.display = DisplayStyle.Flex;
                        }
                    }
                }
            }
        }

        private string TagToUxmlId(Tag tag)
        {
            string tagName = tag.ToString().ToLower();
            if (tagName.Contains("weapon") || tagName.Contains("mainhand")) return "slot-mainhand";
            if (tagName.Contains("armor") || tagName.Contains("chest")) return "slot-chest";
            if (tagName.Contains("core") || tagName.Contains("head")) return "slot-head";
            if (tagName.Contains("legs")) return "slot-legs";
            if (tagName.Contains("offhand")) return "slot-offhand";
            return "";
        }
        
        public void OnEquipment(InputAction.CallbackContext context)
        {
            if (!IsLocalPlayer) return;
            
            Debug.Log($"Inventory Input: {context.action.name}, Phase: {context.phase}, IsLocalPlayer: {IsLocalPlayer}");
            
            if (context.phase == InputActionPhase.Performed)
            {
                var inventoryUI = GetComponent<InventoryUIController>();
                if (inventoryUI != null)
                {
                    inventoryUI.ToggleMenu();
                }
            }
        }

        public void Cleanup()
        {
            if (_equipmentManager != null)
            {
                _equipmentManager.OnEquipmentChanged -= RefreshEquipment;
                _equipmentManager = null;
            }
            _localPlayerInventory = null;
        }
    }
}
