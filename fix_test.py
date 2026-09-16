import re

with open('Assets/Systems/AbilitySystem/Runtime/Networking/ReplicationManager.cs', 'r') as f:
    content = f.read()

content = content.replace(
'''            if (effect.Source != null && effect.Source.NetworkRole != null)
                data.SourceId = effect.Source.NetworkRole.NetworkObjectId;
            else
                data.SourceId = _owner.NetworkRole.NetworkObjectId;''',
'''            if (effect.Source != null && effect.Source.NetworkRole != null)
                data.SourceId = effect.Source.NetworkRole.NetworkObjectId;
            else if (_owner.NetworkRole != null)
                data.SourceId = _owner.NetworkRole.NetworkObjectId;
            else
                data.SourceId = 0;'''
)

with open('Assets/Systems/AbilitySystem/Runtime/Networking/ReplicationManager.cs', 'w') as f:
    f.write(content)
