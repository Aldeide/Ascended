using System;
using Unity.Netcode;
using UnityEngine.Serialization;

namespace AbilitySystem.Runtime.Networking
{
    [Serializable]
    public struct PredictionKey : INetworkSerializable
    {
        public int currentKey;
        [NonSerialized]
        public int BaseKey;
        [NonSerialized]
        private static int _counter;

        public PredictionKey(int currentKey, int baseKey = 0)
        {
            this.currentKey = currentKey;
            this.BaseKey = baseKey;
        }

        public static PredictionKey CreatePredictionKey()
        {
            _counter++;
            if (_counter <= 0) _counter = 1;
            return new PredictionKey(_counter);
        }

        public static PredictionKey CreateDependentPredictionKey(PredictionKey baseKey)
        {
            _counter++;
            if (_counter <= 0) _counter = 1;
            return new PredictionKey(_counter, baseKey.currentKey);
        }

        public static PredictionKey CreateInvalidPredictionKey()
        {
            return new PredictionKey(0);
        }

        public bool IsValidKey()
        {
            return currentKey > 0;
        }

        public void Invalidate()
        {
            currentKey = 0;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref currentKey);
        }
    }
}