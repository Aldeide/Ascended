using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.Core;
using AbilitySystem.Runtime.Networking;
using AbilitySystem.Test.Utilities;
using Moq;
using NUnit.Framework;

namespace AbilitySystem.Test.Runtime.Networking
{
    public class AbilityBatchingTests : AbilitySystemTestBase
    {
        [Test]
        public void ScopedAbilityRPCBatcher_BatchesActivationAndTermination()
        {
            var repl = new MockReplicationManager(Source);

            bool batchRequested = false;
            AbilityBatchData receivedBatch = default;
            bool activationRequested = false;
            bool terminationRequested = false;

            repl.OnServerAbilityBatchRequested += (batch) => { batchRequested = true; receivedBatch = batch; };
            repl.OnServerAbilityActivationRequested += (name, key, data) => activationRequested = true;
            repl.OnServerAbilityTerminationRequested += (name) => terminationRequested = true;

            using (var batcher = new ScopedAbilityRPCBatcher(repl))
            {
                repl.RequestAbilityActivation("TestAbility", new PredictionKey { currentKey = 1 }, new AbilityData());
                repl.RequestAbilityTermination("TestAbility");
            }

            Assert.IsFalse(activationRequested, "Activation should have been intercepted by batcher.");
            Assert.IsFalse(terminationRequested, "Termination should have been intercepted by batcher.");
            Assert.IsTrue(batchRequested, "A batch should have been emitted upon disposal.");
            Assert.AreEqual("TestAbility", receivedBatch.AbilityName);
            Assert.AreEqual(1, receivedBatch.PredictionKey.currentKey);
            Assert.IsTrue(receivedBatch.EndAbilityImmediately);
        }
    }
}
