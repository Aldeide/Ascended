using System.IO;
using AbilitySystem.Runtime.Cues;
using AbilitySystem.Runtime.Networking;
using NUnit.Framework;
using Unity.Netcode;
using UnityEngine;

namespace AbilitySystem.Test.Runtime.Networking
{
    public class CueDataTests
    {
        [Test]
        public void NetworkSerialize_SerializesAllFieldsCorrectly()
        {
            var writer = new FastBufferWriter(1024, Unity.Collections.Allocator.Temp);
            var cueDataOut = new CueData
            {
                VectorData = new[] { new Vector3(1, 2, 3) },
                Magnitude = 100f,
                Normal = new Vector3(0, 1, 0),
                SourceId = 42,
                TargetId = 43,
                PredictionKey = new PredictionKey { currentKey = 10 }
            };

            writer.WriteValueSafe(cueDataOut);

            var reader = new FastBufferReader(writer, Unity.Collections.Allocator.Temp);
            var cueDataIn = new CueData();
            reader.ReadValueSafe(out cueDataIn);

            Assert.AreEqual(cueDataOut.VectorData[0], cueDataIn.VectorData[0]);
            Assert.AreEqual(cueDataOut.Magnitude, cueDataIn.Magnitude);
            Assert.AreEqual(cueDataOut.Normal, cueDataIn.Normal);
            Assert.AreEqual(cueDataOut.SourceId, cueDataIn.SourceId);
            Assert.AreEqual(cueDataOut.TargetId, cueDataIn.TargetId);
            Assert.AreEqual(cueDataOut.PredictionKey.currentKey, cueDataIn.PredictionKey.currentKey);

            writer.Dispose();
            reader.Dispose();
        }
    }
}
