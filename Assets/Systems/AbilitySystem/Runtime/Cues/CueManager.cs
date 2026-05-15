using System;
using System.Collections.Generic;
using AbilitySystem.Runtime.Core;
using AbilitySystem.Runtime.Networking;
using GameplayTags.Runtime;
using UnityEngine;

namespace AbilitySystem.Runtime.Cues
{
    /// <summary>
    /// Manages the lifecycle of gameplay cues, handling their addition, removal, and execution.
    /// </summary>
    public class CueManager
    {
        private readonly IAbilitySystem _owner;
        private readonly IDataManager _dataManager;

        public Action<CueDefinition, CueData> OnCueAdd;
        public Action<CueDefinition, CueData> OnCueRemove;
        public Action<CueDefinition, CueData> OnCueExecute;

        private readonly Dictionary<Tag, CueData> _activeCues = new();
        private readonly List<(string Tag, PredictionKey Key, float Time)> _predictedCues = new();
        private const float PredictedCueTimeout = 5.0f; // Clear predicted cues after 5 seconds
        
        public CueManager(IAbilitySystem owner, IDataManager dataManager = null)
        {
            _owner = owner;
            _dataManager = dataManager ?? owner.DataManager;
        }

        public void MarkCueAsPredicted(string cueTag, PredictionKey key)
        {
            if (!key.IsValidKey()) return;
            _predictedCues.Add((cueTag, key, Time.time));
            CleanOldPredictedCues();
        }

        private bool IsCuePredicted(string cueTag, PredictionKey key)
        {
            if (!key.IsValidKey()) return false;
            return _predictedCues.Exists(c => c.Tag == cueTag && c.Key.currentKey == key.currentKey);
        }

        private void CleanOldPredictedCues()
        {
            _predictedCues.RemoveAll(c => Time.time - c.Time > PredictedCueTimeout);
        }

        /// <summary>
        /// Processes a gameplay cue based on its tag, action, and data, and invokes the appropriate operation for the cue.
        /// </summary>
        /// <param name="cueTag">The tag that identifies the specific cue.</param>
        /// <param name="cueAction">The action to perform on the cue (Add, Remove, or Execute).</param>
        /// <param name="cueData">The data associated with the cue, including positional and normal information.</param>
        public void OnCueReceived(Tag cueTag, CueAction cueAction, CueData cueData)
        {
            // Don't play cues on the server.
            if (_owner.IsServer() && !_owner.IsHost()) return;

            // Prediction Culling: If this was a predicted cue and it's coming from the server, cull it.
            if (!_owner.IsServer() && IsCuePredicted(cueTag.Name, cueData.PredictionKey))
            {
                // Debug.Log($"[CueManager] Culling predicted cue: {cueTag.Name}");
                return;
            }

            Debug.Log("Processing Received Cue: " + cueTag.Name + " / " + cueAction.ToString());

            var cueDefinition = _dataManager.GetCueByTag(cueTag);
            if (!cueDefinition)
            {
#if UNITY_EDITOR
                Debug.LogWarning("Cue not found in data manager: " + cueTag);
#endif
                return;
            }

            switch (cueAction)
            {
                case CueAction.Add:
                    AddCue(cueDefinition, cueData);
                    break;
                case CueAction.Remove:
                    RemoveCue(cueDefinition, cueData);
                    break;
                case CueAction.Execute:
                    ExecuteCue(cueDefinition, cueData);
                    break;
                default:
                    break;
            }
        }

        public void AddCue(CueDefinition cue, CueData data)
        {
            if (!_activeCues.TryAdd(cue.CueTag, data)) return;
            OnCueAdd?.Invoke(cue, data);
        }

        public void RemoveCue(CueDefinition cue, CueData data)
        {
            if (_activeCues.Remove(cue.CueTag, out _))
            {
                OnCueRemove?.Invoke(cue, data);
            }
        }

        public void ExecuteCue(CueDefinition cue, CueData data)
        {
            OnCueExecute?.Invoke(cue, data);
        }

        public Dictionary<Tag, CueData> GetActiveCues()
        {
            return _activeCues;
        }
    }
}