using System;
using NewGraph;
using UnityEngine;

namespace AbilityGraph
{
    [Serializable]
    public abstract class AbilityNode : INode
    {
        public virtual void Execute()
        {
            Debug.Log("Executing node");
        }
    }
    
    [Serializable, Node("#007F00FF", "Special", createInputPort = false)]
    public class ActivateNode : AbilityNode
    {

        [Port, SerializeReference]
        public AbilityNode nextNode;
    }
    
    [Serializable, Node("#007F00FF", "Special", createInputPort = true)]
    public class TestAbilityNode : AbilityNode
    {

        [Port, SerializeReference]
        public AbilityNode nextNode;
    }
}