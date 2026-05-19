using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.Core;
using AbilitySystem.Runtime.Effects;
using AbilitySystem.Scripts;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Unity.Netcode;

public class AbilitySystemDebugWindow : EditorWindow
{
    private AbilitySystemComponent _inspectedComponent;
    
    private ScrollView _localScroll;
    private Label _localServerDebugText;
    private Label _objectNameLabel;
    private float _lastRebuildTime;
    
    // Details panel
    private VisualElement _detailsContainer;
    private Effect _selectedEffect;
    private Ability _selectedAbility;

    [MenuItem("Window/Ability System/Debug Window")]
    public static void ShowWindow()
    {
        AbilitySystemDebugWindow wnd = GetWindow<AbilitySystemDebugWindow>();
        wnd.titleContent = new GUIContent("Ability System Debug");
        wnd.minSize = new Vector2(700, 500);
    }

    public void CreateGUI()
    {
        VisualElement root = rootVisualElement;
        
        // --- Header ---
        var header = new VisualElement();
        header.style.flexDirection = FlexDirection.Row;
        header.style.paddingLeft = 5;
        header.style.paddingTop = 5;
        header.style.paddingBottom = 5;
        header.style.backgroundColor = new Color(0.15f, 0.15f, 0.15f);
        header.style.borderBottomWidth = 1;
        header.style.borderBottomColor = Color.black;
        
        _objectNameLabel = new Label("Select a GameObject with an AbilitySystemComponent");
        _objectNameLabel.style.fontSize = 14;
        _objectNameLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
        header.Add(_objectNameLabel);

        var refreshBtn = new Button(() => { if(_inspectedComponent != null) _inspectedComponent.RequestUpdateFromServer(); });
        refreshBtn.text = "Force Server Sync";
        refreshBtn.style.marginLeft = 20;
        header.Add(refreshBtn);
        
        root.Add(header);

        // --- Main Content (Split View) ---
        var mainSplit = new TwoPaneSplitView(0, 350, TwoPaneSplitViewOrientation.Horizontal);
        root.Add(mainSplit);

        // Left Pane: Local Info
        var leftPane = new VisualElement();
        leftPane.style.flexGrow = 1;
        leftPane.Add(CreatePanelHeader("LOCAL STATE (Active Client)"));
        _localScroll = new ScrollView();
        leftPane.Add(_localScroll);
        mainSplit.Add(leftPane);

        // Right Pane: Vertically Split for Details and Server State
        var rightSplit = new TwoPaneSplitView(0, 250, TwoPaneSplitViewOrientation.Vertical);
        mainSplit.Add(rightSplit);

        // Details Panel
        _detailsContainer = new VisualElement();
        _detailsContainer.style.paddingLeft = 5;
        _detailsContainer.Add(CreatePanelHeader("SELECTION DETAILS"));
        rightSplit.Add(_detailsContainer);

        // Server Comparison
        var serverPane = new VisualElement();
        serverPane.Add(CreatePanelHeader("SERVER STATE (REMOTE SNAPSHOT)", new Color(0.4f, 0.2f, 0.2f)));
        var serverScroll = new ScrollView();
        _localServerDebugText = new Label("No data received from server.");
        _localServerDebugText.style.fontSize = 11;
        _localServerDebugText.style.color = new Color(0.8f, 0.8f, 0.8f);
        _localServerDebugText.style.paddingLeft = 5;
        serverScroll.Add(_localServerDebugText);
        serverPane.Add(serverScroll);
        rightSplit.Add(serverPane);
    }

    private VisualElement CreatePanelHeader(string title, Color? bgColor = null)
    {
        var label = new Label($" {title}");
        label.style.unityFontStyleAndWeight = FontStyle.Bold;
        label.style.backgroundColor = bgColor ?? new Color(0.25f, 0.25f, 0.25f);
        label.style.color = Color.white;
        label.style.height = 20;
        return label;
    }

    private void OnSelectionChange()
    {
        var active = Selection.activeGameObject;
        if (active != null)
        {
            var component = active.GetComponent<AbilitySystemComponent>();
            if (component != null)
            {
                _inspectedComponent = component;
                _objectNameLabel.text = $"Inspecting: {active.name}";
                _lastRebuildTime = 0; // Force rebuild
                Repaint();
            }
        }
    }

    private void OnInspectorUpdate()
    {
        if (_inspectedComponent == null) return;
        
        if (EditorApplication.isPlaying)
        {
            _inspectedComponent.RequestUpdateFromServer();
        }
        
        RefreshGUI();
    }

    private void RefreshGUI()
    {
        if (_inspectedComponent == null || _inspectedComponent.AbilitySystem == null) return;

        // Update Server Text
        _localServerDebugText.text = string.IsNullOrEmpty(_inspectedComponent.ServerDebugString) 
            ? "Waiting for server response..." 
            : _inspectedComponent.ServerDebugString;

        // We rebuild the list only periodically or if explicitly needed
        // to avoid destroying buttons while they are being clicked.
        if (Time.realtimeSinceStartup - _lastRebuildTime > 0.5f)
        {
            RebuildLocalList();
            _lastRebuildTime = Time.realtimeSinceStartup;
        }

        UpdateDetails();
    }

    private void RebuildLocalList()
    {
        _localScroll.Clear();
        
        // Network Section
        AddHeader(_localScroll, "Network Information");
        var system = _inspectedComponent.AbilitySystem;
        var role = system.NetworkRole;
        
        AddRow(_localScroll, "Network ID", role?.NetworkObjectId.ToString() ?? "N/A");
        AddRow(_localScroll, "Is Server", system.IsServer().ToString());
        AddRow(_localScroll, "Is Host", system.IsHost().ToString());
        AddRow(_localScroll, "Is Local Client", system.IsLocalClient().ToString());
        
        if (role != null)
        {
            AddRow(_localScroll, "Is Owner", role.IsOwner.ToString());
            AddRow(_localScroll, "Is Local Player", role.IsLocalPlayer.ToString());
            AddRow(_localScroll, "Has Authority", role.HasAuthority.ToString());
        }

        // Attributes Section
        AddHeader(_localScroll, "Attributes");
        foreach (var set in _inspectedComponent.AbilitySystem.AttributeSetManager.AttributeSets.Values)
        {
            foreach (var attr in set.GetAllAttributes())
            {
                AddRow(_localScroll, attr.GetName(), $"{attr.CurrentValue:F2} / {attr.BaseValue:F2}");
            }
        }

        // Effects Section
        AddHeader(_localScroll, "Active Effects");
        var activeEffects = _inspectedComponent.AbilitySystem.EffectManager.Effects;
        if (activeEffects.Count == 0)
        {
            _localScroll.Add(new Label("  (None)") { style = { color = Color.gray, marginLeft = 10 } });
        }
        foreach (var effect in activeEffects)
        {
            var btn = new Button(() => SelectEffect(effect));
            btn.text = $"{effect.Definition.name} (x{effect.NumStacks})";
            btn.style.unityTextAlign = TextAnchor.MiddleLeft;
            btn.style.marginLeft = 5;
            btn.style.marginRight = 5;
            
            if (effect == _selectedEffect)
            {
                btn.style.backgroundColor = new Color(0.2f, 0.4f, 0.7f);
                btn.style.color = Color.white;
            }
            
            _localScroll.Add(btn);
        }
        
        // Predicted Effects Section
        var predicted = _inspectedComponent.AbilitySystem.EffectManager.PredictedEffects;
        if (predicted.Count > 0)
        {
            AddHeader(_localScroll, "Predicted Effects");
            foreach (var group in predicted)
            {
                foreach (var effect in group.Value)
                {
                    var btn = new Button(() => SelectEffect(effect));
                    btn.text = $"[P] {effect.Definition.name} (Key: {group.Key})";
                    btn.style.unityTextAlign = TextAnchor.MiddleLeft;
                    btn.style.color = new Color(1f, 0.8f, 0.4f);
                    btn.style.marginLeft = 5;
                    _localScroll.Add(btn);
                }
            }
        }

        // Abilities Section
        AddHeader(_localScroll, "Abilities");
        var abilities = _inspectedComponent.AbilitySystem.AbilityManager.Abilities;
        if (abilities.Count == 0)
        {
            _localScroll.Add(new Label("  (None)") { style = { color = Color.gray, marginLeft = 10 } });
        }
        foreach (var kvp in abilities)
        {
            var btn = new Button(() => SelectAbility(kvp.Value));
            btn.text = $"{kvp.Key}: {kvp.Value.DebugString()}";
            btn.style.unityTextAlign = TextAnchor.MiddleLeft;
            btn.style.marginLeft = 5;
            btn.style.marginRight = 5;
            
            if (kvp.Value == _selectedAbility)
            {
                btn.style.backgroundColor = new Color(0.2f, 0.4f, 0.7f);
                btn.style.color = Color.white;
            }
            
            _localScroll.Add(btn);
        }

        // Tags Section
        AddHeader(_localScroll, "Gameplay Tags");
        var tagLabel = new Label(_inspectedComponent.AbilitySystem.TagManager.DebugString());
        tagLabel.style.fontSize = 10;
        tagLabel.style.paddingLeft = 10;
        _localScroll.Add(tagLabel);
    }

    private void AddRow(VisualElement container, string label, string value)
    {
        var row = new VisualElement();
        row.style.flexDirection = FlexDirection.Row;
        row.style.justifyContent = Justify.SpaceBetween;
        row.style.paddingLeft = 10;
        row.style.paddingRight = 10;
        row.style.borderBottomWidth = 1;
        row.style.borderBottomColor = new Color(0.2f, 0.2f, 0.2f, 0.5f);

        row.Add(new Label(label));
        row.Add(new Label(value) { style = { unityFontStyleAndWeight = FontStyle.Bold } });
        container.Add(row);
    }

    private void AddHeader(VisualElement container, string text)
    {
        var h = new Label(text.ToUpper());
        h.style.unityFontStyleAndWeight = FontStyle.Bold;
        h.style.fontSize = 11;
        h.style.marginTop = 15;
        h.style.marginBottom = 5;
        h.style.color = new Color(0.7f, 0.7f, 0.7f);
        h.style.borderBottomWidth = 1;
        h.style.borderBottomColor = new Color(0.4f, 0.4f, 0.4f);
        container.Add(h);
    }

    private void SelectEffect(Effect effect)
    {
        _selectedEffect = effect;
        _selectedAbility = null;
        UpdateDetails();
    }

    private void SelectAbility(Ability ability)
    {
        _selectedAbility = ability;
        _selectedEffect = null;
        UpdateDetails();
    }

    private void UpdateDetails()
    {
        _detailsContainer.Clear();
        _detailsContainer.Add(CreatePanelHeader("SELECTION DETAILS"));

        if (_selectedEffect == null && _selectedAbility == null)
        {
            var noSel = new Label("Select an effect or ability from the list to see details.");
            noSel.style.paddingTop = 20;
            noSel.style.unityTextAlign = TextAnchor.MiddleCenter;
            noSel.style.color = Color.gray;
            _detailsContainer.Add(noSel);
            return;
        }

        if (_selectedEffect != null)
        {
            UpdateEffectDetails(_selectedEffect);
        }
        else if (_selectedAbility != null)
        {
            UpdateAbilityDetails(_selectedAbility);
        }
    }

    private void UpdateEffectDetails(Effect effect)
    {
        var box = new VisualElement();
        box.style.paddingLeft = 10;
        box.style.paddingTop = 10;

        box.Add(new Label($"Effect Name: {effect.Definition.name}") { style = { unityFontStyleAndWeight = FontStyle.Bold, fontSize = 13 } });
        box.Add(new Label($"Guid: {effect.Guid}"));
        box.Add(new Label($"Level: {effect.Level}"));
        box.Add(new Label($"Stacks: {effect.NumStacks}"));
        
        string durationStr = effect.Definition.IsInfinite() ? "Infinite" : $"{effect.RemainingDuration():F2}s / {effect.Duration:F2}s";
        box.Add(new Label($"Duration: {durationStr}"));
        box.Add(new Label($"Is Predicted: {effect.IsPredicted()}") { style = { color = effect.IsPredicted() ? Color.yellow : Color.white } });
        
        if (effect.Context != null)
        {
            box.Add(new Label($"Source ID: {effect.Source?.NetworkRole?.NetworkObjectId ?? 0}"));
        }

        if (effect.SetByCallerTagMagnitudes.Count > 0)
        {
            var sbcHeader = new Label("\nSET BY CALLER DATA:");
            sbcHeader.style.unityFontStyleAndWeight = FontStyle.Bold;
            box.Add(sbcHeader);
            foreach (var kvp in effect.SetByCallerTagMagnitudes)
            {
                box.Add(new Label($"  - {kvp.Key.Name}: {kvp.Value:F3}"));
            }
        }
        
        var tagsHeader = new Label("\nTAGS:");
        tagsHeader.style.unityFontStyleAndWeight = FontStyle.Bold;
        box.Add(tagsHeader);
        if (effect.Definition.AssetTags != null)
        {
            foreach(var t in effect.Definition.AssetTags) box.Add(new Label($"  - {t.Name} (Asset)"));
        }
        if (effect.Definition.GrantedTags != null)
        {
            foreach(var t in effect.Definition.GrantedTags) box.Add(new Label($"  - {t.Name} (Granted)"));
        }

        _detailsContainer.Add(box);
    }

    private void UpdateAbilityDetails(Ability ability)
    {
        var box = new VisualElement();
        box.style.paddingLeft = 10;
        box.style.paddingTop = 10;

        box.Add(new Label($"Ability: {ability.Definition.UniqueName}") { style = { unityFontStyleAndWeight = FontStyle.Bold, fontSize = 13 } });
        box.Add(new Label($"State: {(ability.IsActive ? "Active" : "Inactive")}"));
        box.Add(new Label($"Level: {ability.Level}"));

        if (ability is ChargesAbility chargesAbility)
        {
            var chargesHeader = new Label("\nCHARGES INFO:");
            chargesHeader.style.unityFontStyleAndWeight = FontStyle.Bold;
            box.Add(chargesHeader);
            
            box.Add(new Label($"  - Current Charges: {chargesAbility.GetCurrentCharges()}"));
            box.Add(new Label($"  - Max Charges: {chargesAbility.GetMaxCharges()}"));
            
            var def = (ChargesAbilityDefinition)ability.Definition;
            box.Add(new Label($"  - Base Max: {def.MaxCharges}"));
            box.Add(new Label($"  - Meta Attr: {def.MaxChargesMetaAttribute ?? "None"}"));
            
            // Show contributing effects
            var activeEffects = _inspectedComponent.AbilitySystem.EffectManager.GetActiveEffects();
            var metaName = def.MaxChargesMetaAttribute;
            if (!string.IsNullOrEmpty(metaName))
            {
                var modHeader = new Label("\nACTIVE MODIFIERS:");
                modHeader.style.unityFontStyleAndWeight = FontStyle.Bold;
                box.Add(modHeader);
                
                bool found = false;
                foreach (var effect in activeEffects)
                {
                    if (effect.Definition.Modifiers == null) continue;
                    if (!def.MaxChargesModifiersTagQuery.MatchesTags(effect.Definition.AssetTags)) continue;
                    
                    foreach (var mod in effect.Definition.Modifiers)
                    {
                        if (mod.AttributeName == metaName)
                        {
                            box.Add(new Label($"  - {effect.Definition.name}: {mod.Operation} {mod.Calculate(effect)} (x{effect.NumStacks})"));
                            found = true;
                        }
                    }
                }
                if (!found) box.Add(new Label("  - (None)"));
            }
        }

        _detailsContainer.Add(box);
    }
}
