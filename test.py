import re

with open('Assets/AISystem/Tests/Editor/AISystemTests.cs', 'r') as f:
    content = f.read().replace('\r\n', '\n')

content = content.replace(
'''            // Setup a player threat
            var (playerGo, _, _, _) = CreateMockAgent("Player", "Player");
            playerGo.transform.position = new Vector3(1000, 1000, 1010);''',
'''            // Setup a player threat
            var (playerGo, _, _, playerAsc) = CreateMockAgent("Player", "Player");
            playerGo.transform.position = new Vector3(1000, 1000, 1010);
            AbilitySystemComponent.ActiveInstances.Add(playerAsc);''')

with open('Assets/AISystem/Tests/Editor/AISystemTests.cs', 'w', newline='\n') as f:
    f.write(content)
