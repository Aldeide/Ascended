using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.Attributes;
using AbilitySystem.Runtime.Core;
using AbilitySystemExtension.Runtime.Abilities;
using AbilitySystemExtension.Runtime.AttributeSets;
using System.Collections.Generic;
using System.Linq;
using SystemsExtensions.AbilitySystemExtension.Runtime.Controllers;
using UnityEngine;
using UnityEngine.UI;

namespace Systems.Controllers
{
    public class InterfaceController : MonoBehaviour
    {
        [SerializeField] private Slider healthSlider;
        [SerializeField] private Slider _energySlider;
        [SerializeField] private Text _ammoCount;
        [SerializeField] private GameObject _dash;
        [SerializeField] private GameObject _chargePrefab;
        [SerializeField] private Image[] _dashCharges;
        [SerializeField] private GameObject _reload;

        private AbilitySystemManager _asc;
        private List<UIChargeController> _chargeControllers = new List<UIChargeController>();
        private int _currentDashCharges;
        private int _maxDashCharges;
        
        private Ability _reloadAbility;
        private UIChargeController _reloadController;
        private float _reloadStartTime;
        private float _reloadDuration;
        private bool _isReloading;
        private float _reloadFadeTime;

        public void Initialise(AbilitySystemManager owner)
        {
            _asc = owner;
            _asc.AttributeSetManager.RegisterOnAttributeChanged("Health", OnHealthChanged);
            _asc.AttributeSetManager.RegisterOnAttributeChanged("MaxHealth", OnHealthChanged);
            _asc.AttributeSetManager.RegisterOnAttributeChanged("Energy", OnEnergyChanged);
            _asc.AttributeSetManager.RegisterOnAttributeChanged("MaxEnergy", OnEnergyChanged);
            _asc.AttributeSetManager.RegisterOnAttributeChanged("ClipSize", OnAmmoChanged);
            _asc.AttributeSetManager.RegisterOnAttributeChanged("CurrentClip", OnAmmoChanged);
            healthSlider.value = 1;
            UpdateHealth();

            SetupDashUI();
            SetupReloadUI();
        }

        private void SetupReloadUI()
        {
            _reloadController = _reload.GetComponent<UIChargeController>();
            _reload.SetActive(false);

            if (_asc.AbilityManager.Abilities.TryGetValue("ReloadWeaponAbility", out _reloadAbility))
            {
                _reloadAbility.OnActivateResult += OnReloadActivateResult;
                _reloadAbility.OnEndAbility += OnReloadEnded;
                _reloadAbility.OnCancelAbility += OnReloadEnded;
            }
        }

        private void OnReloadActivateResult(AbilityActivationResult result)
        {
            if (result == AbilityActivationResult.Success)
            {
                _isReloading = true;
                _reload.SetActive(true);
                _reloadStartTime = Time.time;
                _reloadFadeTime = 0;
                
                var weaponSet = _asc.AttributeSetManager.GetAttributeSet<WeaponAttributeSet>();
                _reloadDuration = weaponSet != null ? weaponSet.ReloadTime.CurrentValue : 1.0f;
                
                if (_reloadController) _reloadController.SetFill(0f);
            }
        }

        private void OnReloadEnded()
        {
            _isReloading = false;
            // Start fading out after a short delay or immediately
            _reloadFadeTime = Time.time + 0.2f;
        }

        private void Update()
        {
            if (_isReloading && _reloadController != null)
            {
                float elapsed = Time.time - _reloadStartTime;
                float progress = Mathf.Clamp01(elapsed / _reloadDuration);
                _reloadController.SetFill(progress);
            }
            else if (_reloadFadeTime > 0 && Time.time > _reloadFadeTime)
            {
                _reload.SetActive(false);
                _reloadFadeTime = 0;
            }
        }

        private void SetupDashUI()
        {
            if (_asc == null || _asc.AbilityManager == null) return;

            if (!_asc.AbilityManager.Abilities.TryGetValue("Dash", out var ability))
            {
                // Fallback: try to find any ChargesAbility if "Dash" specifically isn't found
                ability = _asc.AbilityManager.Abilities.Values.FirstOrDefault(a => a is DashAbility || a.Definition.UniqueName.Contains("Dash"));
                if (ability == null) return;
            }

            if (ability is not ChargesAbility dashAbility) return;

            _currentDashCharges = dashAbility.GetCurrentCharges();
            _maxDashCharges = dashAbility.GetMaxCharges();

            // Clear existing charges
            foreach (Transform child in _dash.transform)
            {
                Destroy(child.gameObject);
            }
            _chargeControllers.Clear();

            // Spawn charges
            for (int i = 0; i < _maxDashCharges; i++)
            {
                var go = Instantiate(_chargePrefab, _dash.transform);
                go.name = $"Charge_{i}";
                go.SetActive(true);

                var rect = go.GetComponent<RectTransform>();
                if (rect != null)
                {
                    rect.anchorMin = new Vector2(0, 0);
                    rect.anchorMax = new Vector2(0, 0);
                    rect.pivot = new Vector2(0, 0);
                    rect.anchoredPosition = new Vector2(i * 18, 0);
                    rect.localScale = Vector3.one;
                }
                
                var controller = go.GetComponent<UIChargeController>();
                if (controller != null)
                {
                    _chargeControllers.Add(controller);
                    controller.SetFill(i < _currentDashCharges ? 1f : 0f);
                }
            }

            dashAbility.OnChargesChanged += OnDashChargesChanged;
            dashAbility.OnCooldownProgressChanged += OnDashCooldownProgressChanged;
        }

        private void OnDashChargesChanged(int current, int max)
        {
            _currentDashCharges = current;
            _maxDashCharges = max;

            // Re-spawn if max charges changed
            if (max != _chargeControllers.Count)
            {
                SetupDashUI();
                return;
            }

            for (int i = 0; i < _chargeControllers.Count; i++)
            {
                _chargeControllers[i].SetFill(i < current ? 1f : 0f);
            }
        }

        private void OnDashCooldownProgressChanged(float progress)
        {
            if (_currentDashCharges < _maxDashCharges && _currentDashCharges < _chargeControllers.Count)
            {
                _chargeControllers[_currentDashCharges].SetFill(progress);
            }
        }

        public void OnHealthChanged(Attribute attribute, float oldValue, float newValue)
        {
            UpdateHealth();
        }
        
        public void OnEnergyChanged(Attribute attribute, float oldValue, float newValue)
        {
            UpdateEnergy();
        }

        public void OnAmmoChanged(Attribute attribute, float oldValue, float newValue)
        {
            var current = _asc.AttributeSetManager.GetAttributeValue<WeaponAttributeSet>("CurrentClip").CurrentValue;
            var max = _asc.AttributeSetManager.GetAttributeValue<WeaponAttributeSet>("ClipSize").CurrentValue;
            _ammoCount.text = $"{current}/{max}";
        }
        
        private void UpdateHealth()
        {
            var maxHealth = _asc.AttributeSetManager.GetAttributeValue<CharacteristicsAttributeSet>("MaxHealth").CurrentValue;
            var health = _asc.AttributeSetManager.GetAttributeValue<CharacteristicsAttributeSet>("Health").CurrentValue;
            healthSlider.value = health / maxHealth;
        }
        
        private void UpdateEnergy()
        {
            var maxEnergy = _asc.AttributeSetManager.GetAttributeValue<CharacteristicsAttributeSet>("MaxEnergy").CurrentValue;
            var energy = _asc.AttributeSetManager.GetAttributeValue<CharacteristicsAttributeSet>("Energy").CurrentValue;
            _energySlider.value = energy / maxEnergy;
        }

    }
}