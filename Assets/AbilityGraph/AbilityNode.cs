using System;
using NewGraph;
using UnityEngine;

namespace AbilityGraph
{
    [Serializable]
    public abstract class AbilityNodeBase : INode
    {
        [Port, SerializeReference]
        public AbilityNodeBase executeNext;
    }

    public abstract class InstantAbilityNode : AbilityNodeBase
    {
        public abstract void Execute();
    }
    
    public abstract class DurationalAbilityNode : AbilityNodeBase
    {
        public abstract void Start();

        public abstract void Tick();

        public abstract void End();
    }
    
    [Serializable, Node("#007F00FF", "Special", createInputPort = false)]
    public class ActivateAbilityNode : InstantAbilityNode
    {
        public override void Execute()
        {
            
        }
    }
    
    [Serializable, Node("#007F00FF", "Special", createInputPort = true)]
    public class TestAbilityNode : InstantAbilityNode
    {
        public override void Execute()
        {
            Debug.Log("Executing Test node");
        }
    }
}