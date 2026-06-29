import re

with open("Assets/AISystem/Tests/Editor/AISystemTests.cs", "r") as f:
    content = f.read()

# Fix case 1: player needs an ASC now
content = content.replace(
    'playerGo.transform.position = new Vector3(1000, 1000, 1010);',
    'playerGo.transform.position = new Vector3(1000, 1000, 1010);\n            var playerAsc = playerGo.AddComponent<AbilitySystemComponent>();\n            playerAsc.OnEnable();'
)

# Fix case 2: otherEnemy needs OnEnable to be tracked
content = content.replace(
    'var otherAsc = otherEnemyGo.AddComponent<AbilitySystemComponent>();',
    'var otherAsc = otherEnemyGo.AddComponent<AbilitySystemComponent>();\n            otherAsc.OnEnable();'
)

with open("Assets/AISystem/Tests/Editor/AISystemTests.cs", "w") as f:
    f.write(content)
