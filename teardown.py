import re

with open('Assets/AISystem/Tests/Editor/AISystemTests.cs', 'r') as f:
    content = f.read().replace('\r\n', '\n')

content = content.replace(
'''        [TearDown]
        public void Teardown()
        {
            foreach (var go in _createdObjects)
            {
                if (go != null)
                {
                    Object.DestroyImmediate(go);
                }
            }
            _createdObjects.Clear();
            TacticalGroupCoordinator.Instance = null;
            TacticalPointManager.Instance = null;
        }''',
'''        [TearDown]
        public void Teardown()
        {
            foreach (var go in _createdObjects)
            {
                if (go != null)
                {
                    Object.DestroyImmediate(go);
                }
            }
            _createdObjects.Clear();
            TacticalGroupCoordinator.Instance = null;
            TacticalPointManager.Instance = null;
            AbilitySystemComponent.ActiveInstances.Clear();
        }''')

with open('Assets/AISystem/Tests/Editor/AISystemTests.cs', 'w', newline='\n') as f:
    f.write(content)
