using System;
using NewGraph;
using UnityEngine;

namespace AbilityGraph
{
    [Serializable]
    public abstract class AbilityNode : INode
    {
        [Port, SerializeReference]
        public AbilityNode nextNode;
        public virtual void Execute()
        {
            Debug.Log("Executing node");
        }
    }
    
    [Serializable, Node("#007F00FF", "Special", createInputPort = false)]
    public class ActivateAbilityNode : AbilityNode
    {
        
    }
    
    [Serializable, Node("#007F00FF", "Special", createInputPort = true)]
    public class TestAbilityNode : AbilityNode
    {
        

        public override void Execute()
        {
            Debug.Log("Executing Test node");
        }
    }
}