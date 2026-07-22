import re

filepath = "Assets/AISystem/Tests/Editor/AISystemTests.cs"
with open(filepath, "r", encoding="utf-8") as f:
    content = f.read()

content = content.replace('\r\n', '\n')

teardown_setup = """        [TearDown]
        public void TearDown()
        {
            AbilitySystemComponent.ActiveInstances.Clear();
            foreach (var go in _gameObjectsToCleanup)
            {"""

content = content.replace("""        [TearDown]
        public void TearDown()
        {
            foreach (var go in _gameObjectsToCleanup)
            {""", teardown_setup)

create_mock_agent = """        private (GameObject go, Mock<IMonoAgent> agentMock, Mock<IAbilitySystem> abilitySystemMock, AbilitySystemComponent asc) CreateMockAgent(string name = "MockAgent", string tag = "Enemy")
        {
            var go = CreateGameObject(name);
            go.tag = tag;

            var asc = go.AddComponent<AbilitySystemComponent>();
            var abilitySystemMock = AbilitySystemUtilities.CreateMockAbilitySystem(true);

            // Set the internal AbilitySystem property on AbilitySystemComponent using reflection
            var prop = typeof(AbilitySystemComponent).GetProperty("AbilitySystem", BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
            prop.SetValue(asc, abilitySystemMock.Object);

            var agentMock = new Mock<IMonoAgent>();
            agentMock.SetupGet(a => a.Transform).Returns(go.transform);

            AbilitySystemComponent.ActiveInstances.Add(asc);

            return (go, agentMock, abilitySystemMock, asc);
        }"""

content = re.sub(r'        private \(GameObject go,\s*Mock<IMonoAgent> agentMock,\s*Mock<IAbilitySystem> abilitySystemMock,\s*AbilitySystemComponent asc\)\s*CreateMockAgent\(.*?\{.*?        \}', create_mock_agent, content, flags=re.DOTALL)

with open(filepath, "w", encoding="utf-8") as f:
    f.write(content)
