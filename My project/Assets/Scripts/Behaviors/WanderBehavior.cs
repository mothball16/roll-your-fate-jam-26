using Assets.Scripts.Interfaces;
using UnityEngine;
namespace Assets.Scripts.Behaviors
{
    internal class WanderBehavior : IBehavior
    {
        // whether or not wander has its own steering direction or whether it updates according to the current direction
        public bool UseIndependentDirection { get; set; } = false;
        public float Frequency { get; set; } = 0.3f;
        public float Strength { get; set; } = 1f;

        private Vector3 _direction, _desiredVelocity;
        private float _step;
        public Vector3 GetVelocity(CharacterController agent, float dt)
        {
            _step += dt;
            if (_step >= Frequency)
            {
                _step -= Frequency;
                var dirChange = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
                _direction = ((UseIndependentDirection ? _direction : agent.Velocity.normalized) + (dirChange.normalized * Strength)).normalized;
            }

            _desiredVelocity = _direction * agent.MaxSpeed;
            Vector3 seekingForce = _desiredVelocity - agent.Velocity;
            return seekingForce;
        }
    }
}
