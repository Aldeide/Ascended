with open("Assets/AISystem/Tests/Editor/AISystemTests.cs", "r", encoding="utf-8") as f:
    lines = f.readlines()
for i, line in enumerate(lines):
    if i >= 550 and i <= 590:
        print(f"{i}: {line}", end="")
