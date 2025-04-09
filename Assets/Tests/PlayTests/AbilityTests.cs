using System.Collections;
using System.Collections.Generic;
using Authoring.AttributeSets;
using NUnit.Framework;
using Systems.AbilitySystem.Components;
using Systems.Development;
using UnityEngine;
using UnityEngine.TestTools;

public class AbilityTests
{
    // A Test behaves as an ordinary method
    [Test]
    public void AbilityTestsSimplePasses()
    {
        // Use the Assert class to test conditions
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator AbilityTestsWithEnumeratorPasses()
    {
        GameObject gameObject = new GameObject("TestGameObject");
        AbilitySystemComponent abilitySystemComponent = gameObject.AddComponent<AbilitySystemComponent>();
        gameObject.AddComponent<DevelopmentComponent>();
        gameObject.SetActive(true);
        
        yield return new WaitForFixedUpdate();
        abilitySystemComponent.AttributesSystem.AddAttributeSet<CharacteristicsAttributeSet>();
        yield return new WaitForFixedUpdate();
        Assert.AreEqual(
            abilitySystemComponent.AttributesSystem.GetAttributeCurrentValue("CharacteristicsAttributeSet", "Health"),
            100);
    }
}
