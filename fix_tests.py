import re

filepath = "Assets/AISystem/Tests/Editor/AISystemTests.cs"
with open(filepath, "r", encoding="utf-8") as f:
    content = f.read()

content = content.replace('\r\n', '\n')

# Check what using directives are missing for TestAttributeSetDefinition and EffectDefinition
# The error says: "The type or namespace name 'TestAttributeSetDefinition' could not be found"
# And "The type or namespace name 'EffectDefinition' could not be found"
# Let's revert the CreateMockAgent function back to its original state and just add the ActiveInstances.Add(asc) part, because the original state compiled.
