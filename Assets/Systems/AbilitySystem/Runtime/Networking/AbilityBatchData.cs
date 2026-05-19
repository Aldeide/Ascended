using AbilitySystem.Runtime.Abilities;
using Unity.Netcode;

namespace AbilitySystem.Runtime.Networking
{
    public struct AbilityBatchData : INetworkSerializable
    {
        public string AbilityName;
        public PredictionKey PredictionKey;
        public AbilityData ActivationData;
        public bool EndAbilityImmediately;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref AbilityName);
            PredictionKey.NetworkSerialize(serializer);
            ActivationData.NetworkSerialize(serializer);
            serializer.SerializeValue(ref EndAbilityImmediately);
        }
    }
}
