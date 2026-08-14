import re

file_path = 'Assets/AISystem/Tests/Editor/AISystemTests.cs'
with open(file_path, 'r', encoding='utf-8') as f:
    content = f.read()

match = re.search(r'public void EnemyDecisionMaker_GoalSelectionLogic\(\)[\s\S]*?\}', content)
if match:
    print(match.group(0))

match2 = re.search(r'public void GoapAbilityAction_PerformsCorrectly\(\)[\s\S]*?\}', content)
if match2:
    print(match2.group(0))
