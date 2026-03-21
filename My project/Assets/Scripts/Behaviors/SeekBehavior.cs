using Assets.Scripts.Interfaces;
using UnityEngine;

namespace Assets.Scripts.Behaviors
{
    internal class SeekBehavior : IBehavior
    {
        public Transform Target { get; set; }
        public Vector3 GetVelocity(CharacterController agent, float _)
        {
            if (!Target) return Vector3.zero;
            Vector3 desiredVelocity = Target.position - agent.Position;
            desiredVelocity = desiredVelocity.normalized * agent.MaxSpeed;
            Vector3 seekingForce = desiredVelocity - agent.Velocity;
            return seekingForce;
        }
    }
}
