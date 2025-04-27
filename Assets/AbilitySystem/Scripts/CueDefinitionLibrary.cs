using System.Collections.Generic;
using System.Linq;
using AbilitySystem.Runtime.Cues;
using AbilitySystem.Runtime.Effects;
using AbilitySystem.Runtime.Tags;
using Sirenix.OdinInspector;
using UnityEngine;

namespace AbilitySystem.Scripts
{
    public class CueDefinitionLibrary : MonoBehaviour
    {
        [ShowInInspector]
        private Dictionary<GameplayTag, CueDefinition> _cues = new();

        private void Awake()
        {
            foreach (var cue in Resources.LoadAll<CueDefinition>(""))
            {
                _cues.Add(cue.cueTag, cue);
            }
        }

        public CueDefinition GetCueByTag(GameplayTag cueTag)
        {
            return _cues.TryGetValue(cueTag, out var cue) ? cue : null;
        }

        public CueDefinition GetCueByTag(string cueTag)
        {
            return _cues.FirstOrDefault(c => c.Key.GetName() == cueTag).Value;
        }
    }
}