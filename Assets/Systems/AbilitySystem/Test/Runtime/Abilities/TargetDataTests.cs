using System.Collections.Generic;
using AbilitySystem.Runtime.Abilities.Targeting;
using NUnit.Framework;
using Unity.Netcode;
using UnityEngine;

namespace AbilitySystem.Test.Runtime.Abilities
{
    public class TargetDataTests
    {
        [Test]
        public void TargetDataHandle_AddAndRetrieve_ReturnsCorrectData()
        {
            var handle = new TargetDataHandle();
            var location = new TargetDataLocation { Position = new Vector3(1, 2, 3) };
            var actor = new TargetDataActor { NetworkObjectId = 12345 };

            handle.Add(location);
            handle.Add(actor);

            Assert.AreEqual(2, handle.Data.Count);
            Assert.IsInstanceOf<TargetDataLocation>(handle.Data[0]);
            Assert.AreEqual(new Vector3(1, 2, 3), ((TargetDataLocation)handle.Data[0]).Position);
            Assert.IsInstanceOf<TargetDataActor>(handle.Data[1]);
            Assert.AreEqual(12345, ((TargetDataActor)handle.Data[1]).NetworkObjectId);
        }

        [Test]
        public void TargetDataHandle_NetworkSerialize_SerializesAndDeserializesCorrectly()
        {
            var originalHandle = new TargetDataHandle();
            originalHandle.Add(new TargetDataLocation { Position = new Vector3(1, 2, 3) });
            originalHandle.Add(new TargetDataActor { NetworkObjectId = 12345 });
            originalHandle.Add(new TargetDataHitResult { Position = new Vector3(4, 5, 6), Normal = new Vector3(0, 1, 0), NetworkObjectId = 67890, ColliderIndex = 1 });

            using var writer = new FastBufferWriter(1024, Unity.Collections.Allocator.Temp);
            writer.WriteNetworkSerializable(in originalHandle);

            using var reader = new FastBufferReader(writer, Unity.Collections.Allocator.Temp);
            reader.ReadNetworkSerializable(out TargetDataHandle deserializedHandle);

            Assert.AreEqual(3, deserializedHandle.Data.Count);

            Assert.IsInstanceOf<TargetDataLocation>(deserializedHandle.Data[0]);
            Assert.AreEqual(new Vector3(1, 2, 3), ((TargetDataLocation)deserializedHandle.Data[0]).Position);

            Assert.IsInstanceOf<TargetDataActor>(deserializedHandle.Data[1]);
            Assert.AreEqual(12345, ((TargetDataActor)deserializedHandle.Data[1]).NetworkObjectId);

            Assert.IsInstanceOf<TargetDataHitResult>(deserializedHandle.Data[2]);
            var hitResult = (TargetDataHitResult)deserializedHandle.Data[2];
            Assert.AreEqual(new Vector3(4, 5, 6), hitResult.Position);
            Assert.AreEqual(new Vector3(0, 1, 0), hitResult.Normal);
            Assert.AreEqual(67890, hitResult.NetworkObjectId);
            Assert.AreEqual(1, hitResult.ColliderIndex);
        }
    }
}
