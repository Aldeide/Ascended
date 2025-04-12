using UnityEngine;

namespace Systems.AbilitySystem.Replication
{
    public class PredictionKey
    {
        [SerializeField]
        private int _key = 0;

        public int GetNextPredictionKey()
        {
            _key += 1;
            return _key;
        }
    }
}