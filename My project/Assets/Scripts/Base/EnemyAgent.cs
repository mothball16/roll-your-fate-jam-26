using UnityEngine;
using Assets.Scripts.Components;

namespace Assets.Scripts.Agents
{
    [RequireComponent(typeof(CharacterController))]
    public class EnemyAgent : MonoBehaviour
    {
        public float TargetRefreshRate = 0.5f;
        public TeamType TargetTeam = TeamType.Player;
        protected float _targetTimer;
        protected CharacterController _char;
        public virtual void Awake()
        {
            _char = GetComponent<CharacterController>();
        }


        public virtual Vector3 CalculateSteering()
        {
            Vector3 steering = Vector3.zero;
            return steering;
        }

        public virtual void RunLogic()
        {
            
        }

        public virtual void Update()
        {
            _targetTimer -= Time.deltaTime;
            if (_targetTimer <= 0f)
            {
                RunLogic();
                _targetTimer = TargetRefreshRate;
            }

            _char.SteeringForce = CalculateSteering();

            // Rotate to face the moving direction
            if (_char.Velocity.sqrMagnitude > 0.001f)
            {
                _char.Rotation = Quaternion.LookRotation(Vector3.forward, _char.Velocity.normalized);
            }
        }

        protected Transform FindClosestTarget()
        {
            Transform target = null;
            var allTeams = FindObjectsByType<Team>();
            float closestDistance = float.MaxValue;

            foreach (var teamComp in allTeams)
            {
                if (teamComp.team == TargetTeam)
                {
                    float distance = Vector3.Distance(transform.position, teamComp.transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        target = teamComp.transform;
                    }
                }
            }
            return target;
        }
    }
}