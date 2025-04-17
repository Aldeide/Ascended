using AbilitySystem.Runtime.Core;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace AbilitySystemExtension.Scripts
{
    public class AbilitySystemDebugComponent : MonoBehaviour
    {
        private AbilitySystemManager _asc;
        public TMP_Text text;
        
        public void Initialise(AbilitySystemManager asc)
        {
            _asc = asc;
        }
        
        public void Update()
        {
            if (_asc == null) return;
            DisplayData();
        }

        private void DisplayData()
        {
            var output = _asc.AttributeSetManager.DebugString() + "\n\n";
            output += _asc.EffectManager.DebugString() + "\n\n";
            output += _asc.AbilityManager.DebugString() + "\n\n";
            if (!text) return;
            text.text = output;
        }
    }
}