import re

with open('Assets/AISystem/Tests/Editor/AISystemTests.cs', 'r') as f:
    content = f.read().replace('\r\n', '\n')

content = content.replace(
'''        private (GameObject go, Mock<IMonoAgent> agentMock, Mock<IAbilitySystem> asMock, AbilitySystemComponent asc) CreateMockAgent(string name = "Agent", string tag = "Enemy")
        {
            var go = CreateGameObject(name);
            try { go.tag = tag; } catch { }
            var agentMock = new Mock<IMonoAgent>();
            agentMock.Setup(a => a.Transform).Returns(go.transform);

            var asc = go.AddComponent<AbilitySystemComponent>();
            var abilitySystemMock = new Mock<IAbilitySystem>();
            var attributeSetManager = new AttributeSetManager(abilitySystemMock.Object);
            var tagManager = new TagManager(abilitySystemMock.Object);

            abilitySystemMock.Setup(x => x.AttributeSetManager).Returns(attributeSetManager);
            abilitySystemMock.Setup(x => x.TagManager).Returns(tagManager);
            asc.AbilitySystem = abilitySystemMock.Object;

            return (go, agentMock, abilitySystemMock, asc);
        }''',
'''        private (GameObject go, Mock<IActionReceiver> agentMock, Mock<IAbilitySystem> asMock, AbilitySystemComponent asc) CreateMockAgent(string name = "Agent", string tag = "Enemy")
        {
            var go = CreateGameObject(name);
            try { go.tag = tag; } catch { }
            var agentMock = new Mock<IActionReceiver>();
            agentMock.Setup(a => a.Transform).Returns(go.transform);

            var asc = go.AddComponent<AbilitySystemComponent>();
            var abilitySystemMock = new Mock<IAbilitySystem>();
            var attributeSetManager = new AttributeSetManager(abilitySystemMock.Object);
            var tagManager = new TagManager(abilitySystemMock.Object);

            abilitySystemMock.Setup(x => x.AttributeSetManager).Returns(attributeSetManager);
            abilitySystemMock.Setup(x => x.TagManager).Returns(tagManager);
            asc.AbilitySystem = abilitySystemMock.Object;

            AbilitySystemComponent.ActiveInstances.Add(asc);

            return (go, agentMock, abilitySystemMock, asc);
        }''')

with open('Assets/AISystem/Tests/Editor/AISystemTests.cs', 'w', newline='\n') as f:
    f.write(content)
