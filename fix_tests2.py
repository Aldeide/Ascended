import re

with open('Assets/AISystem/Tests/Editor/AISystemTests.cs', 'r') as f:
    content = f.read().replace('\r\n', '\n')

content = content.replace(
'''        private (GameObject go, Mock<IMonoAgent> agentMock, Mock<IAbilitySystem> abilitySystemMock, AbilitySystemComponent asc) CreateMockAgent(string name = "MockAgent", string tag = "Enemy")
        {
            var go = CreateGameObject(name);
            go.tag = tag;

            var asc = go.AddComponent<AbilitySystemComponent>();
            var abilitySystemMock = new Mock<IAbilitySystem>();

            // Set the internal AbilitySystem property on AbilitySystemComponent using reflection
            var prop = typeof(AbilitySystemComponent).GetProperty("AbilitySystem", BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
            prop.SetValue(asc, abilitySystemMock.Object);

            var agentMock = new Mock<IMonoAgent>();
            agentMock.SetupGet(a => a.Transform).Returns(go.transform);

            return (go, agentMock, abilitySystemMock, asc);
        }''',
'''        private (GameObject go, Mock<IActionReceiver> agentMock, Mock<IAbilitySystem> abilitySystemMock, AbilitySystemComponent asc) CreateMockAgent(string name = "MockAgent", string tag = "Enemy")
        {
            var go = CreateGameObject(name);
            try { go.tag = tag; } catch { }

            var asc = go.AddComponent<AbilitySystemComponent>();
            var abilitySystemMock = new Mock<IAbilitySystem>();

            // Set the internal AbilitySystem property on AbilitySystemComponent using reflection
            var prop = typeof(AbilitySystemComponent).GetProperty("AbilitySystem", BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
            prop.SetValue(asc, abilitySystemMock.Object);

            var agentMock = new Mock<IActionReceiver>();
            agentMock.SetupGet(a => a.Transform).Returns(go.transform);

            AbilitySystemComponent.ActiveInstances.Add(asc);

            return (go, agentMock, abilitySystemMock, asc);
        }''')

with open('Assets/AISystem/Tests/Editor/AISystemTests.cs', 'w', newline='\n') as f:
    f.write(content)
