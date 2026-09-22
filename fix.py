with open("Assets/Systems/AbilitySystem/Scripts/AbilitySystemComponent.cs", "r") as f:
    content = f.read()

import re

# We need to add "using System.Collections.Generic;"
if "using System.Collections.Generic;" not in content:
    content = content.replace("using System.Linq;", "using System.Linq;\nusing System.Collections.Generic;")
    with open("Assets/Systems/AbilitySystem/Scripts/AbilitySystemComponent.cs", "w") as f:
        f.write(content)
