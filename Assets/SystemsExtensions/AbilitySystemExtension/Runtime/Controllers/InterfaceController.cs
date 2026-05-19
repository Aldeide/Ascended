using System.Collections.Generic;
using System.Linq;
using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.Attributes;
using AbilitySystem.Runtime.Core;
using AbilitySystemExtension.Runtime.Abilities;
using AbilitySystemExtension.Runtime.AttributeSets;
using SystemsExtensions.AbilitySystemExtension.Runtime.Controllers;
using UnityEngine;
using UnityEngine.UI;

namespace Systems.Controllers
{
    public class InterfaceController : MonoBehaviour
    {
        [SerializeField] private Slider _healthSlider;
        [SerializeField] private Slider _healthGhostSlider;
        [SerializeField] private float _ghostDelay = 1.0f;
        [SerializeField] private float _ghostSpeed = 2.0f;
        [SerializeField] private Slider _energySlider;
        [SerializeField] private GameObject _energyContainer;
        [SerializeField] private Text _ammoCount;
        [SerializeField] private GameObject _dash;
        [SerializeField] private GameObject _chargePrefab;
        [SerializeField] private Image[] _dashCharges;
        [SerializeField] private GameObject _reload;
        [SerializeField] private Slider _shieldSlider;
        [SerializeField] private Slider _shieldGhostSlider;
        [SerializeField] private GameObject _shieldContainer;

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
        private float _ghostTimer;
        private float _shieldGhostTimer;
        private Vector2 _energyOriginalAnchoredPosition;
        private Vector2 _ammoOriginalAnchoredPosition;
        private bool _positionsCached;

        public void Initialise(AbilitySystemManager owner)
        {
            _asc = owner;
            _asc.AttributeSetManager.RegisterOnAttributeChanged("Health", OnHealthChanged);
            _asc.AttributeSetManager.RegisterOnAttributeChanged("MaxHealth", OnHealthChanged);
            _asc.AttributeSetManager.RegisterOnAttributeChanged("Energy", OnEnergyChanged);
            _asc.AttributeSetManager.RegisterOnAttributeChanged("MaxEnergy", OnEnergyChanged);
            _asc.AttributeSetManager.RegisterOnAttributeChanged("ClipSize", OnAmmoChanged);
            _asc.AttributeSetManager.RegisterOnAttributeChanged("CurrentClip", OnAmmoChanged);
            _asc.AttributeSetManager.RegisterOnAttributeChanged("Shield", OnShieldChanged);
            _asc.AttributeSetManager.RegisterOnAttributeChanged("MaxShield", OnShieldChanged);

            if (!_positionsCached)
            {
                if (_energySlider != null)
                {
                    _energyOriginalAnchoredPosition = _energyContainer.GetComponent<RectTransform>().anchoredPosition;
                }
                if (_ammoCount != null)
                {
                    _ammoOriginalAnchoredPosition = _ammoCount.GetComponent<RectTransform>().anchoredPosition;
                }
                _positionsCached = true;
            }

            _healthSlider.value = 1;
            if (_healthGhostSlider) _healthGhostSlider.value = 1;
            if (_shieldSlider) _shieldSlider.value = 0;
            if (_shieldGhostSlider) _shieldGhostSlider.value = 0;

            UpdateHealth();
            UpdateShield();
            SetupDashUI();
            SetupReloadUI();
        }

        private void SetupReloadUI()
        {
            _reloadController = _reload.GetComponent<UIChargeController>();
            _reload.SetActive(false);
            if (!_asc.AbilityManager.Abilities.TryGetValue("ReloadWeaponAbility", out _reloadAbility)) return;
            _reloadAbility.OnActivateResult += OnReloadActivateResult;
            _reloadAbility.OnEndAbility += OnReloadEnded;
            _reloadAbility.OnCancelAbility += OnReloadEnded;
        }

        private void OnReloadActivateResult(AbilityActivationResult result)
        {
            if (result != AbilityActivationResult.Success) return;
            _isReloading = true;
            _reload.SetActive(true);
            _reloadStartTime = Time.time;
            _reloadFadeTime = 0;
            var weaponSet = _asc.AttributeSetManager.GetAttributeSet<WeaponAttributeSet>();
            _reloadDuration = weaponSet != null ? weaponSet.ReloadTime.CurrentValue : 1.0f;
            if (_reloadController) _reloadController.SetFill(0f);
        }

        private void OnReloadEnded()
        {
            _isReloading = false;
            _reloadFadeTime = Time.time + 0.2f;
        }

        private void Update()
        {
            if (_isReloading && _reloadController)
            {
                var elapsed = Time.time - _reloadStartTime;
                var progress = Mathf.Clamp01(elapsed / _reloadDuration);
                _reloadController.SetFill(progress);
            }
            else if (_reloadFadeTime > 0 && Time.time > _reloadFadeTime)
            {
                _reload.SetActive(false);
                _reloadFadeTime = 0;
            }

            UpdateGhostBar();
        }

        private void UpdateGhostBar()
        {
            if (_healthGhostSlider && Time.time > _ghostTimer)
            {
                _healthGhostSlider.value = _healthGhostSlider.value > _healthSlider.value
                    ? Mathf.MoveTowards(_healthGhostSlider.value, _healthSlider.value, _ghostSpeed * Time.deltaTime)
                    : _healthSlider.value;
            }

            if (_shieldGhostSlider && _shieldSlider && Time.time > _shieldGhostTimer)
            {
                _shieldGhostSlider.value = _shieldGhostSlider.value > _shieldSlider.value
                    ? Mathf.MoveTowards(_shieldGhostSlider.value, _shieldSlider.value, _ghostSpeed * Time.deltaTime)
                    : _shieldSlider.value;
            }
        }

        private void SetupDashUI()
        {
            if (_asc?.AbilityManager == null) return;

            if (!_asc.AbilityManager.Abilities.TryGetValue("Dash", out var ability))
            {
                // Fallback: try to find any ChargesAbility if "Dash" specifically isn't found
                ability = _asc.AbilityManager.Abilities.Values.FirstOrDefault(a =>
                    a is DashAbility || a.Definition.UniqueName.Contains("Dash"));
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
            for (var i = 0; i < _maxDashCharges; i++)
            {
                var go = Instantiate(_chargePrefab, _dash.transform);
                go.name = $"Charge_{i}";
                go.SetActive(true);

                var rect = go.GetComponent<RectTransform>();
                if (rect)
                {
                    rect.anchorMin = new Vector2(0, 0);
                    rect.anchorMax = new Vector2(0, 0);
                    rect.pivot = new Vector2(0, 0);
                    rect.anchoredPosition = new Vector2(i * 18, 0);
                    rect.localScale = Vector3.one;
                }

                var controller = go.GetComponent<UIChargeController>();
                if (!controller) continue;
                _chargeControllers.Add(controller);
                controller.SetFill(i < _currentDashCharges ? 1f : 0f);
            }

            dashAbility.OnChargesChanged += OnDashChargesChanged;
            dashAbility.OnCooldownProgressChanged += OnDashCooldownProgressChanged;
        }

        private void OnDashChargesChanged(int current, int max)
        {
            _currentDashCharges = current;
            _maxDashCharges = max;

            if (max != _chargeControllers.Count)
            {
                SetupDashUI();
                return;
            }

            for (var i = 0; i < _chargeControllers.Count; i++)
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
            var maxHealth = _asc.AttributeSetManager.GetAttributeValue<CharacteristicsAttributeSet>("MaxHealth")
                .CurrentValue;
            var health = _asc.AttributeSetManager.GetAttributeValue<CharacteristicsAttributeSet>("Health").CurrentValue;

            var targetValue = health / maxHealth;

            if (targetValue < _healthSlider.value)
            {
                // Damage taken: Start/Reset ghost timer
                _ghostTimer = Time.time + _ghostDelay;
            }
            else if (targetValue > _healthSlider.value)
            {
                // Healed: Snap ghost bar immediately
                if (_healthGhostSlider) _healthGhostSlider.value = targetValue;
            }

            _healthSlider.value = targetValue;
        }

        private void UpdateEnergy()
        {
            var maxEnergy = _asc.AttributeSetManager.GetAttributeValue<CharacteristicsAttributeSet>("MaxEnergy")
                .CurrentValue;
            var energy = _asc.AttributeSetManager.GetAttributeValue<CharacteristicsAttributeSet>("Energy").CurrentValue;
            _energySlider.value = energy / maxEnergy;
        }

        public void OnShieldChanged(Attribute attribute, float oldValue, float newValue)
        {
            UpdateShield();
        }

        private void UpdateShield()
        {
            var maxShield = _asc.AttributeSetManager.GetAttributeValue<CharacteristicsAttributeSet>("MaxShield").CurrentValue;
            var shield = _asc.AttributeSetManager.GetAttributeValue<CharacteristicsAttributeSet>("Shield").CurrentValue;
 
            var showShield = maxShield > 0f;

            if (_shieldContainer)
            {
                _shieldContainer.SetActive(showShield);
            }
            else
            {
                if (_shieldSlider) _shieldSlider.gameObject.SetActive(showShield);
                if (_shieldGhostSlider) _shieldGhostSlider.gameObject.SetActive(showShield);
            }

            UpdateElementPositions(showShield);

            if (!showShield) return;

            var targetValue = maxShield > 0f ? shield / maxShield : 0f;

            if (!_shieldSlider) return;
            if (targetValue < _shieldSlider.value)
            {
                _shieldGhostTimer = Time.time + _ghostDelay;
            }
            else if (targetValue > _shieldSlider.value)
            {
                if (_shieldGhostSlider) _shieldGhostSlider.value = targetValue;
            }
            _shieldSlider.value = targetValue;
        }

        private void UpdateElementPositions(bool showShield)
        {
            if (!_positionsCached) return;

            var offset = showShield ? 0f : -10f;

            if (_energyContainer)
            {
                var rect = _energyContainer.GetComponent<RectTransform>();
                rect.anchoredPosition = new Vector2(_energyOriginalAnchoredPosition.x, _energyOriginalAnchoredPosition.y + offset);
            }

            if (!_ammoCount) return;
            {
                var rect = _ammoCount.GetComponent<RectTransform>();
                rect.anchoredPosition = new Vector2(_ammoOriginalAnchoredPosition.x, _ammoOriginalAnchoredPosition.y + offset);
            }
        }
    }
}