import re

with open('Assets/Systems/AbilitySystem/Test/Utilities/AbilitySystemTestBase.cs', 'r') as f:
    content = f.read()

# Add missing using statement
new_content = re.sub(
    r'(using Moq;)',
    r'using AbilitySystem.Scripts;\n\1',
    content
)

with open('Assets/Systems/AbilitySystem/Test/Utilities/AbilitySystemTestBase.cs', 'w') as f:
    f.write(new_content)
