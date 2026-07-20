import re

with open("Assets/AISystem/Tests/Editor/AISystemTests.cs", "r", encoding="utf-8") as f:
    content = f.read()

search = """        [Test]
        public void TargetDeadSensor_ChecksTargetStateCorrectly()
        {
            var (agentGo, agentMock, _, agentAsc) = CreateMockAgent("Agent", "Enemy");

            AbilitySystem.Scripts.AbilitySystemComponent.ActiveInstances.Add(agentAsc);

            var sensor = new TargetDeadSensor();

            // Case 1: No target/action -> true
            Assert.IsTrue(ToBool(sensor.Sense((IActionReceiver)agentMock.Object, null)));

            // Case 2: Target is alive
            var (targetGo, targetMock, _, targetAsc) = CreateMockAgent("Target", "Player");"""

replace = """        [Test]
        public void TargetDeadSensor_ChecksTargetStateCorrectly()
        {
            var (agentGo, agentMock, _, agentAsc) = CreateMockAgent("Agent", "Enemy");

            AbilitySystem.Scripts.AbilitySystemComponent.ActiveInstances.Add(agentAsc);

            var sensor = new TargetDeadSensor();

            // Case 1: No target/action -> true
            Assert.IsTrue(ToBool(sensor.Sense((IActionReceiver)agentMock.Object, null)));

            // Case 2: Target is alive
            var (targetGo, targetMock, _, targetAsc) = CreateMockAgent("Target", "Player");"""

# Wait, there's no targetGo already defined in the lines before Case 2?
# Ah, I replaced:
# var (agentGo, agentMock, _, agentAsc) = CreateMockAgent("Agent", "Enemy");
# var (targetGo, targetMock, _, asc) = CreateMockAgent("Target", "Enemy");
# earlier, but it seems there was no compile error yet in the previous run.
# Actually, the compile error says "error CS0128: A local variable or function named 'targetGo' is already defined in this scope" at line 579.
# Line 579 is: var (targetGo, targetMock, _, targetAsc) = CreateMockAgent("Target", "Player");
# Is there another targetGo defined?
