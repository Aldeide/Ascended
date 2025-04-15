using System;
using UnityEngine.Serialization;

namespace AbilitySystem.Runtime.Networking
{
    [Serializable]
    public struct PredictionKey
    {
        public int currentKey;
        public int baseKey;

        public PredictionKey(int currentKey, int baseKey = 0)
        {
            this.currentKey = currentKey;
            this.baseKey = baseKey;
        }
    }


public class PredictionKeyGenerator
{
    private int _counter;

    public PredictionKeyGenerator()
    {
        _counter = 0;
    }
    
    public PredictionKey CreatePredictionKey()
    {
        _counter++;
        if (_counter <= 0) _counter = 1;
        var predictionKey = new PredictionKey(_counter);
        return predictionKey;
    }

    public PredictionKey CreateDependentPredictionKey(PredictionKey key)
    {
        _counter++;
        var dependentPredictionKey = new PredictionKey(_counter, key.currentKey);
        return dependentPredictionKey;
    }
}

    
    
}