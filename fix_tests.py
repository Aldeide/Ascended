import re

file_path = 'Assets/AISystem/Tests/Editor/AISystemTests.cs'
with open(file_path, 'r', encoding='utf-8') as f:
    content = f.read()

# Add ActiveInstances mock setup for HealTargetSensor allies
content = content.replace('var (allyGo1, _, _, allyAsc1) = CreateMockAgent("Ally1", "Enemy");\n            allyGo1.AddComponent<EnemyDecisionMaker>();',
                          'var (allyGo1, _, _, allyAsc1) = CreateMockAgent("Ally1", "Enemy");\n            AbilitySystemComponent.ActiveInstances.Add(allyAsc1);\n            allyGo1.AddComponent<EnemyDecisionMaker>();')
content = content.replace('var (allyGo2, _, _, allyAsc2) = CreateMockAgent("Ally2", "Enemy");\n            allyGo2.AddComponent<EnemyDecisionMaker>();',
                          'var (allyGo2, _, _, allyAsc2) = CreateMockAgent("Ally2", "Enemy");\n            AbilitySystemComponent.ActiveInstances.Add(allyAsc2);\n            allyGo2.AddComponent<EnemyDecisionMaker>();')

with open(file_path, 'w', encoding='utf-8') as f:
    f.write(content)
