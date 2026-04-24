using System;
using System.Collections.Generic;
using Unity.Netcode;

namespace AbilitySystem.Runtime.Abilities.Targeting
{
    public enum TargetDataType : byte
    {
        Empty = 0,
        Location = 1,
        Actor = 2,
        HitResult = 3
    }

    /// <summary>
    /// A handle to a collection of TargetData.
    /// In GAS, this is usually passed into ActivateAbility to provide context on WHAT was hit/targeted.
    /// </summary>
    public struct TargetDataHandle : INetworkSerializable
    {
        private List<ITargetData> _data;

        public List<ITargetData> Data
        {
            get => _data ??= new List<ITargetData>();
            set => _data = value;
        }

        public void Add(ITargetData data)
        {
            Data.Add(data);
        }

        public void Clear()
        {
            Data.Clear();
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            int count = Data.Count;
            serializer.SerializeValue(ref count);

            if (serializer.IsReader)
            {
                Data.Clear();
                for (int i = 0; i < count; i++)
                {
                    TargetDataType type = TargetDataType.Empty;
                    serializer.SerializeValue(ref type);

                    ITargetData item = type switch
                    {
                        TargetDataType.Location => new TargetDataLocation(),
                        TargetDataType.Actor => new TargetDataActor(),
                        TargetDataType.HitResult => new TargetDataHitResult(),
                        _ => null
                    };

                    if (item != null)
                    {
                        item.NetworkSerialize(serializer);
                        Data.Add(item);
                    }
                }
            }
            else
            {
                foreach (var item in Data)
                {
                    TargetDataType type = item switch
                    {
                        TargetDataLocation => TargetDataType.Location,
                        TargetDataActor => TargetDataType.Actor,
                        TargetDataHitResult => TargetDataType.HitResult,
                        _ => TargetDataType.Empty
                    };
                    serializer.SerializeValue(ref type);
                    item.NetworkSerialize(serializer);
                }
            }
        }
    }
}
