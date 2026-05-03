using AbilitySystem.Runtime.Abilities.Targeting;
using NUnit.Framework;
using Unity.Netcode;
using UnityEngine;

namespace AbilitySystem.Test.Runtime.Abilities
{
    /// <summary>
    /// Unit tests for TargetData containers, verifying data persistence and network serialization integrity.
    /// </summary>
    public class TargetDataTests
    {
        /// <summary>
        /// Verifies that TargetDataHandle correctly stores and retrieves multiple types of targeting information.
        /// </summary>
        [Test]
        public void TargetDataTests_AddAndRetrieve_MaintainsDataIntegrity()
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

        /// <summary>
        /// Verifies that TargetDataHandle correctly serializes and deserializes its contents for network transmission.
        /// </summary>
        [Test]
        public void TargetDataTests_NetworkSerialization_MatchesOriginalData()
        {
            var originalHandle = new TargetDataHandle();
            originalHandle.Add(new TargetDataLocation { Position = new Vector3(1, 2, 3) });
            originalHandle.Add(new TargetDataActor { NetworkObjectId = 12345 });
            originalHandle.Add(new TargetDataHitResult { 
                Position = new Vector3(4, 5, 6), 
                Normal = new Vector3(0, 1, 0), 
                NetworkObjectId = 67890, 
                ColliderIndex = 1 
            });

            // Simulate network transmission
            using var writer = new FastBufferWriter(1024, Unity.Collections.Allocator.Temp);
            writer.WriteNetworkSerializable(in originalHandle);

            using var reader = new FastBufferReader(writer, Unity.Collections.Allocator.Temp);
            reader.ReadNetworkSerializable(out TargetDataHandle deserializedHandle);

            Assert.AreEqual(3, deserializedHandle.Data.Count);

            // Verify Location Data
            Assert.IsInstanceOf<TargetDataLocation>(deserializedHandle.Data[0]);
            Assert.AreEqual(new Vector3(1, 2, 3), ((TargetDataLocation)deserializedHandle.Data[0]).Position);

            // Verify Actor Data
            Assert.IsInstanceOf<TargetDataActor>(deserializedHandle.Data[1]);
            Assert.AreEqual(12345, ((TargetDataActor)deserializedHandle.Data[1]).NetworkObjectId);

            // Verify Hit Result Data
            Assert.IsInstanceOf<TargetDataHitResult>(deserializedHandle.Data[2]);
            var hitResult = (TargetDataHitResult)deserializedHandle.Data[2];
            Assert.AreEqual(new Vector3(4, 5, 6), hitResult.Position);
            Assert.AreEqual(new Vector3(0, 1, 0), hitResult.Normal);
            Assert.AreEqual(67890, hitResult.NetworkObjectId);
            Assert.AreEqual(1, hitResult.ColliderIndex);
        }
    }
}
