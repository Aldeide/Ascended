import re

with open("Assets/AISystem/Tests/Editor/AISystemTests.cs", "r") as f:
    content = f.read()

# Add AbilitySystemComponent.ActiveInstances.Clear() to TearDown
content = content.replace(
    "_gameObjectsToCleanup.Clear();",
    "_gameObjectsToCleanup.Clear();\n            AbilitySystemComponent.ActiveInstances.Clear();"
)

# Call OnEnable directly on AbilitySystemComponent instances created in CreateMockAgent
content = content.replace(
    "var asc = go.AddComponent<AbilitySystemComponent>();",
    "var asc = go.AddComponent<AbilitySystemComponent>();\n            asc.OnEnable();"
)

with open("Assets/AISystem/Tests/Editor/AISystemTests.cs", "w") as f:
    f.write(content)
