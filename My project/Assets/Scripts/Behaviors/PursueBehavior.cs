using UnityEngine;
using Assets.Scripts.Interfaces;

namespace Assets.Scripts.Behaviors
{
    public class PursueBehavior : IBehavior
    {
        public Transform Target;
        public float MaxPredictionTime = 2f;

        public Vector3 GetVelocity(CharacterController agent, float deltaTime)
        {
            if (Target == null) return Vector3.zero;

            Vector3 targetPos = Target.position;
            Vector3 targetVelocity = Vector3.zero;

            if (Target.TryGetComponent<CharacterController>(out var targetChar))
            {
                targetVelocity = targetChar.Velocity;
            }
            else if (Target.TryGetComponent<Rigidbody2D>(out var rb))
            {
                targetVelocity = rb.linearVelocity;
            }

            Vector3 directionToTarget = targetPos - agent.Position;
            float distance = directionToTarget.magnitude;
            float speed = agent.MaxSpeed;

            // Calculate how far ahead to predict based on distance and speed
            float predictionTime = speed > 0 ? distance / speed : 0;
            predictionTime = Mathf.Min(predictionTime, MaxPredictionTime);

            Vector3 predictedPosition = targetPos + targetVelocity * predictionTime;

            // Seek towards the predicted position
            Vector3 desiredVelocity = (predictedPosition - agent.Position).normalized * speed;
            return desiredVelocity - agent.Velocity;
        }
    }
}