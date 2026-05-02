using System.Collections.Generic;
using AbilitySystem.Scripts;
using Item.Runtime.Interface;
using Item.Runtime.Manager;
using Item.Scripts;
using Sirenix.OdinInspector;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;

namespace Item.Scripts.UI
{
    public class InventoryUIController : NetworkBehaviour
    {
        [Header("UI Document")]
        [SerializeField] private UIDocument uiDocument;
        
        [Header("Templates")]
        [SerializeField] private VisualTreeAsset itemTemplate;
        
        private VisualElement _root;
        private VisualElement _inventoryGrid;
        private VisualElement _screenOverlay;
        
        [ShowInInspector]
        private InventoryComponent _localPlayerInventory;
        private AbilitySystemComponent _asc;
        private bool _isMenuOpen = false;

        private void Awake()
        {
            _isMenuOpen = false;
        }

        private bool InitializeUI()
        {
            if (_screenOverlay != null) return true;
            if (uiDocument == null)
            {
                Debug.LogError("InventoryUIController: UIDocument is not assigned on " + gameObject.name);
                return false;
            }

            _root = uiDocument.rootVisualElement;
            if (_root == null) return false;

            _inventoryGrid = _root.Q<VisualElement>("inventory-grid");
            _screenOverlay = _root.Q<VisualElement>("inventory-screen");

            if (_screenOverlay == null)
            {
                Debug.LogError("InventoryUIController: Could not find 'inventory-screen' in the assigned UXML asset.");
                return false;
            }

            // Hide by default if not open
            if (!_isMenuOpen)
            {
                _screenOverlay.AddToClassList("hidden");
            }
            
            return true;
        }

        public void OnInventory(InputAction.CallbackContext context)
        {
            Debug.Log(context.action.name + " was performed!");
            Debug.Log("IsClient" + IsClient);
            if (!IsLocalPlayer) return;
            if (context.phase == InputActionPhase.Performed)
            {
                ToggleMenu();
            }
        }

        private void ToggleMenu()
        {
            if (!InitializeUI()) return;

            _isMenuOpen = !_isMenuOpen;
            
            if (_isMenuOpen)
            {
                _screenOverlay.RemoveFromClassList("hidden");
                
                // Subscribe to changes
                if (GetLocalInventory())
                {
                    _localPlayerInventory.InventoryManager.OnInventoryChanged += RefreshInventory;
                    RefreshInventory();

                    // Initialize Equipment UI
                    var equipComp = _localPlayerInventory.GetComponent<EquipmentComponent>();
                    var equipUI = GetComponent<EquipmentUIController>();
                    if (equipComp != null && equipUI != null)
                    {
                        if (equipComp.EquipmentManager != null)
                        {
                            equipUI.Initialize(_localPlayerInventory, equipComp.EquipmentManager);
                        }
                        else
                        {
                            Debug.LogWarning("InventoryUIController: EquipmentManager is null. Equipment UI will not be initialized.");
                        }
                    }
                }
            }
            else
            {
                _screenOverlay.AddToClassList("hidden");
                if (_localPlayerInventory != null)
                {
                    _localPlayerInventory.InventoryManager.OnInventoryChanged -= RefreshInventory;
                }
            }
        }

        private bool GetLocalInventory()
        {
            if (_localPlayerInventory != null) return true;
            
            _localPlayerInventory = GetComponentInParent<InventoryComponent>();
            return _localPlayerInventory != null;
        }

        private void RefreshInventory()
        {
            if (!GetLocalInventory() || _inventoryGrid == null) return;
            
            _inventoryGrid.Clear();
            
            var items = _localPlayerInventory.InventoryManager.Items;
            if (items == null) return;

            foreach (var item in items)
            {
                var itemElement = itemTemplate.Instantiate();
                var icon = itemElement.Q<VisualElement>("item-icon");
                var quantity = itemElement.Q<Label>("item-quantity");
                var border = itemElement.Q<VisualElement>("item-rarity-border");

                if (item.Icon != null)
                {
                    icon.style.backgroundImage = new StyleBackground(item.Icon);
                }

                // Simple rarity logic - check if it's Equipment to get definition and rarity
                border.ClearClassList();
                border.AddToClassList("rarity-border-common");

                _inventoryGrid.Add(itemElement);
            }
        }
    }
}
