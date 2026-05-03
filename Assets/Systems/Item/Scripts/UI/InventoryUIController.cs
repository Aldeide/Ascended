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
using Cursor = UnityEngine.Cursor;

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

        private void Start()
        {
            // Always initialize and hide on start
            InitializeUI();
        }

        private bool InitializeUI()
        {
            if (_screenOverlay != null) return true;
            
            Debug.Log("InventoryUIController: Initializing UI...");
            
            if (uiDocument == null)
            {
                Debug.LogError("InventoryUIController: UIDocument is not assigned on " + gameObject.name);
                return false;
            }

            _root = uiDocument.rootVisualElement;
            if (_root == null)
            {
                Debug.LogError("InventoryUIController: rootVisualElement is null! Is the UIDocument active and has a panel assigned?");
                return false;
            }

            _inventoryGrid = _root.Q<VisualElement>("inventory-grid");
            _screenOverlay = _root.Q<VisualElement>("inventory-screen");

            if (_screenOverlay == null)
            {
                Debug.LogError("InventoryUIController: Could not find 'inventory-screen' in the assigned UXML asset. Checked name: inventory-screen");
                // Log all children to help debug
                _root.Query<VisualElement>().ForEach(ve => Debug.Log($"Found VE: {ve.name}"));
                return false;
            }

            Debug.Log("InventoryUIController: Successfully found 'inventory-screen'.");

            // Hide by default if not open
            if (!_isMenuOpen)
            {
                _screenOverlay.AddToClassList("hidden");
                _screenOverlay.style.display = DisplayStyle.None;
                Debug.Log("InventoryUIController: UI is closed by default. Added 'hidden' and set display to None.");
            }
            
            return true;
        }

        public void OnInventory(InputAction.CallbackContext context)
        {
            if (!IsLocalPlayer) return;
            
            Debug.Log($"Inventory Input: {context.action.name}, Phase: {context.phase}, IsLocalPlayer: {IsLocalPlayer}");
            
            if (context.phase == InputActionPhase.Performed)
            {
                ToggleMenu();
            }
        }

        private void ToggleMenu()
        {
            Debug.Log("Toggling Inventory Menu...");
            if (!InitializeUI()) 
            {
                Debug.LogError("ToggleMenu: InitializeUI failed!");
                return;
            }
            
            _isMenuOpen = !_isMenuOpen;
            Debug.Log($"Menu Open State: {_isMenuOpen}");
            
            if (_isMenuOpen)
            {
                _screenOverlay.RemoveFromClassList("hidden");
                _screenOverlay.style.display = DisplayStyle.Flex;
                
                // Show cursor
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                
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
                _screenOverlay.style.display = DisplayStyle.None;

                // Hide cursor (assuming gameplay defaults to locked)
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;

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
            Debug.Log("InventoryUIController: Refreshing Inventory UI...");
            if (!GetLocalInventory())
            {
                Debug.LogWarning("RefreshInventory: No local inventory found!");
                return;
            }
            
            if (_inventoryGrid == null)
            {
                Debug.LogWarning("RefreshInventory: inventory-grid is null!");
                return;
            }
            
            _inventoryGrid.Clear();
            
            var items = _localPlayerInventory.InventoryManager.Items;
            if (items == null) 
            {
                Debug.Log("RefreshInventory: Items list is null.");
                return;
            }

            Debug.Log($"RefreshInventory: Found {items.Count} items.");

            if (itemTemplate == null)
            {
                Debug.LogError("RefreshInventory: itemTemplate is null!");
                return;
            }

            foreach (var item in items)
            {
                var itemElement = itemTemplate.Instantiate();
                var icon = itemElement.Q<VisualElement>("item-icon");
                var quantity = itemElement.Q<Label>("item-quantity");
                var border = itemElement.Q<VisualElement>("item-rarity-border");

                if (item.Icon != null && icon != null)
                {
                    icon.style.backgroundImage = new StyleBackground(item.Icon);
                }

                if (border != null)
                {
                    border.ClearClassList();
                    border.AddToClassList("rarity-border-common");
                }

                _inventoryGrid.Add(itemElement);
            }
            Debug.Log("RefreshInventory: Completed grid population.");
        }
    }
}
