using System;
using UnityEngine;

namespace AbilitySystem.Runtime.Modifiers
{
    [Serializable]
    public class Test : AbstractTest
    {
        [SerializeField]
        public string testName;

        public Test()
        {
            
        }
        
        public override float Calculate()
        {
            return 5f;
        }
        
        public override string SaySomething()
        {
            return testName;
        }
    }
    [Serializable]
    public class TestTwo : AbstractTest
    {
        [SerializeField]
        public string anotherTestName;
        
        public TestTwo()
        {
            
        }
        
        public override float Calculate()
        {
            return 1000f;
        }
        
        public override string SaySomething()
        {
            return anotherTestName;
        }
    }
}