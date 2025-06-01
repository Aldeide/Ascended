using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Runtime;
using UnityEngine;

namespace AISystem.Runtime.Sensors
{
    public class IdleTargetSensor : LocalTargetSensorBase
    {
        private static readonly Bounds Bounds = new(Vector3.zero, new Vector3(100, 0, 100));
        
        public override void Created()
        {
        }

        public override void Update()
        {
        }

        public override ITarget Sense(IActionReceiver agent, IComponentReference references, ITarget existingTarget)
        {
            var random = this.GetRandomPosition(agent);
            if (existingTarget is PositionTarget positionTarget)
            {
                return positionTarget.SetPosition(random);
            }

            return new PositionTarget(random);
        }
        
        private Vector3 GetRandomPosition(IActionReceiver agent)
        {
            var random = Random.insideUnitCircle * 3f;
            var position = agent.Transform.position + new Vector3(random.x, 0f, random.y);

            // Check if the position is within the bounds of the world.
            return Bounds.Contains(position) ? position : Bounds.ClosestPoint(position);
        }
    }
}