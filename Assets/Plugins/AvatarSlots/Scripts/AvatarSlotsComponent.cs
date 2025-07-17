using System.Collections.Generic;
using System.Linq;
using GameplayTags.Generated;
using GameplayTags.Runtime;
using Plugins.AvatarSlots.Runtime;
using Sirenix.OdinInspector;
using Unity.Netcode;
using UnityEngine;

namespace AvatarSlots.Scripts
{
    public class AvatarSlotsComponent : NetworkBehaviour
    {
        public List<AvatarSlot> AvatarSlotConfiguration = new();

        // Should we make a list for each tag or have multiple tags that have the same transform?
        private readonly Dictionary<Tag, Transform> _avatarSlots = new();
        private readonly Dictionary<Tag, GameObject> _slottedObjects = new();

        // Development only.
        public GameObject SlotPrefab;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            Initialise();
            if (IsServer)
            {
                NetworkManager.OnClientConnectedCallback += OnClientConnected;
            }
        }

        private void Initialise()
        {
            foreach (var slot in AvatarSlotConfiguration)
            {
                _avatarSlots.Add(slot.SlotTag, slot.Transform);
                _slottedObjects.Add(slot.SlotTag, null);
            }
            SlotGameObject(Instantiate(SlotPrefab), TagLibrary.SlotHipRight);
        }

        public void OnClientConnected(ulong clientId)
        {
            // Might not be required for NetworkedPrefabs?
        }

        public void SlotGameObject(GameObject go, Tag gameplayTag)
        {
            if (!go.TryGetComponent<SlottableComponent>(out var slottableComponent))
            {
                Debug.LogWarning($"The GameObject '{go.name}' is missing a SlottableComponent and cannot be slotted.");
                return;
            }

            if (!_avatarSlots.ContainsKey(gameplayTag) || !_slottedObjects.ContainsKey(gameplayTag)) return;
            go.transform.SetParent(_avatarSlots[gameplayTag]);
            var config = slottableComponent.SlottableConfigurations.FirstOrDefault(c => c.Slot == gameplayTag);
            if (config.Slot == gameplayTag)
            {
                go.transform.localPosition = config.PositionOffset;
                go.transform.localRotation = config.RotationOffset;
            }

            _slottedObjects[gameplayTag] = go;
        }

        public void SwitchSlot(Tag source, Tag target)
        {
            if (_slottedObjects[source] == null || _slottedObjects[target] != null) return;
            var go = _slottedObjects[source];
            go.transform.SetParent(_avatarSlots[target]);

            if (!go.TryGetComponent<SlottableComponent>(out var slottableComponent))
            {
                Debug.LogWarning($"The GameObject '{go.name}' is missing a SlottableComponent and cannot be slotted.");
                return;
            }

            var config = slottableComponent.SlottableConfigurations.FirstOrDefault(c => c.Slot == target);
            if (config.Slot == target)
            {
                go.transform.localPosition = config.PositionOffset;
                go.transform.localRotation = config.RotationOffset;
            }

            _slottedObjects[target] = _slottedObjects[source];
            _slottedObjects[source] = null;
        }

        public void RemoveSlottedGameObject(Tag gameplayTag)
        {
            if (!_slottedObjects.TryGetValue(gameplayTag, out var go)) return;
            Destroy(go);
            _slottedObjects[gameplayTag].transform.SetParent(null);
            _slottedObjects[gameplayTag] = null;
        }

        public GameObject GetSlottedGameObject(Tag gameplayTag)
        {
            return _slottedObjects[gameplayTag];
        }

        [Button]
        public void SwitchSides()
        {
            // Just to test the functionality.
            if (_slottedObjects[TagLibrary.SlotHipRight])
            {
                SwitchSlot(TagLibrary.SlotHipRight, TagLibrary.SlotHipLeft);
            }
            else
            {
                SwitchSlot(TagLibrary.SlotHipLeft, TagLibrary.SlotHipRight);
            }
        }
    }
}