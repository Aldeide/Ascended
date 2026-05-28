using System.Diagnostics;
using System.Linq;
using UnityEngine;
using AbilitySystem.Scripts;
using AISystem.Runtime.Sensors;
using CrashKonijn.Agent.Core;
using Moq;
using NUnit.Framework;
using CrashKonijn.Goap.Runtime;

[TestFixture]
public class TargetSensorBenchmark
{
    private GameObject[] _dummies;

    [SetUp]
    public void Setup()
    {
        _dummies = new GameObject[1000];
        for (int i = 0; i < 1000; i++)
        {
            _dummies[i] = new GameObject("Dummy" + i);
            _dummies[i].AddComponent<AbilitySystemComponent>();
            _dummies[i].transform.position = new Vector3(i, 0, 0);
        }
    }

    [TearDown]
    public void TearDown()
    {
        foreach (var go in _dummies)
        {
            Object.DestroyImmediate(go);
        }
    }

    [Test]
    public void BenchmarkSense()
    {
        var sensor = new EnemyTargetSensor();
        var agentObj = new GameObject("Agent");
        var mockAgent = new Mock<IActionReceiver>();
        mockAgent.Setup(a => a.Transform).Returns(agentObj.transform);

        // Warmup
        sensor.Sense(mockAgent.Object, null, null);

        var sw = Stopwatch.StartNew();
        for (int i = 0; i < 100; i++)
        {
            sensor.Sense(mockAgent.Object, null, null);
        }
        sw.Stop();

        UnityEngine.Debug.Log($"BenchmarkSense took: {sw.ElapsedMilliseconds} ms");

        Object.DestroyImmediate(agentObj);
    }
}
