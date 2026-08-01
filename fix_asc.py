import re

with open("Assets/Systems/AbilitySystem/Scripts/AbilitySystemComponent.cs", "r", encoding="utf-8") as f:
    content = f.read()

search = """    public class AbilitySystemComponent : NetworkBehaviour, INetworkRole
    {
        [FormerlySerializedAs("definition")] public AbilitySystemDefinition Definition;"""

replace = """    public class AbilitySystemComponent : NetworkBehaviour, INetworkRole
    {
        public static readonly System.Collections.Generic.HashSet<AbilitySystemComponent> ActiveInstances = new System.Collections.Generic.HashSet<AbilitySystemComponent>();

        [FormerlySerializedAs("definition")] public AbilitySystemDefinition Definition;"""

content = content.replace(search.replace('\r\n', '\n'), replace.replace('\r\n', '\n'))

if "ActiveInstances.Add(this);" not in content:
    # Need to add OnEnable and OnDisable
    # Let's insert them right after OnNetworkSpawn or RequestUpdateFromServer
    search3 = """        public void RequestUpdateFromServer()
        {"""
    replace3 = """        protected virtual void OnEnable()
        {
            ActiveInstances.Add(this);
        }

        protected virtual void OnDisable()
        {
            ActiveInstances.Remove(this);
        }

        public void RequestUpdateFromServer()
        {"""
    content = content.replace(search3.replace('\r\n', '\n'), replace3.replace('\r\n', '\n'))

with open("Assets/Systems/AbilitySystem/Scripts/AbilitySystemComponent.cs", "w", encoding="utf-8") as f:
    f.write(content)
