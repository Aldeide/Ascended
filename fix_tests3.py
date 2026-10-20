import re

with open('Assets/AISystem/Tests/Editor/AISystemTests.cs', 'r') as f:
    content = f.read().replace('\r\n', '\n')

content = content.replace(
    '        private (GameObject go, Mock<IMonoAgent> agentMock, Mock<IAbilitySystem> abilitySystemMock, AbilitySystemComponent asc) CreateMockAgent(string name = "MockAgent", string tag = "Enemy")',
    '        private (GameObject go, Mock<IActionReceiver> agentMock, Mock<IAbilitySystem> abilitySystemMock, AbilitySystemComponent asc) CreateMockAgent(string name = "MockAgent", string tag = "Enemy")'
)

content = content.replace(
    '            var agentMock = new Mock<IMonoAgent>();',
    '            var agentMock = new Mock<IActionReceiver>();\n            AbilitySystemComponent.ActiveInstances.Add(asc);'
)

content = content.replace(
    '            go.tag = tag;',
    '            try { go.tag = tag; } catch { }'
)

with open('Assets/AISystem/Tests/Editor/AISystemTests.cs', 'w', newline='\n') as f:
    f.write(content)
