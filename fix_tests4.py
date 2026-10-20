import re

with open('Assets/AISystem/Tests/Editor/AISystemTests.cs', 'r') as f:
    content = f.read().replace('\r\n', '\n')

content = content.replace(
    'agentMock.As<IActionReceiver>().Object',
    'agentMock.Object'
)

with open('Assets/AISystem/Tests/Editor/AISystemTests.cs', 'w', newline='\n') as f:
    f.write(content)
