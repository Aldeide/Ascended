using System;
using UnityEngine.Serialization;

namespace AbilitySystem.Runtime.Networking
{
    [Serializable]
    public struct PredictionKey
    {
        public int currentKey;
        [NonSerialized]
        public int BaseKey;

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

        public bool IsValidKey()
        {
            return currentKey > 0;
        }

        public void Invalidate()
        {
            currentKey = 0;
        }
    }
}