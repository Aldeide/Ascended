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
    }
}
