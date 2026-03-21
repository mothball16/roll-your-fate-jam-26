using Assets.Scripts.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts.Behaviors
{
    [RequireComponent(typeof(CharacterController))]
    class InputController : MonoBehaviour, InputSystem_Actions.IPlayerActions
    {
        private InputSystem_Actions _actions;
        private CharacterController _char;
        public void Awake()
        {
            _actions = new();
            _char = gameObject.GetComponent<CharacterController>();
        }


        public void OnEnable()
        {
            _actions.Player.Enable();
            _actions.Player.SetCallbacks(this);
        }
        public void OnDisable()
        {
            _actions.Player.Disable();
            _actions.Player.RemoveCallbacks(this);
        }

        public void Update()
        {
            // Vector2 mouse = InputSystem.
            float angle = 45; //Mathf.Atan2(_velocity.y, _velocity.x) * Mathf.Rad2Deg;
            _char.Rotation = Quaternion.Euler(0, 0, angle - 90);
        }




        public void OnSprint(InputAction.CallbackContext context)
        {

        }

        public void OnMove(InputAction.CallbackContext context)
        {
            _char.SteeringForce = context.ReadValue<Vector2>();
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            
        }

        public void OnAttack(InputAction.CallbackContext context)
        {
            
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
            
        }

        public void OnCrouch(InputAction.CallbackContext context)
        {
            
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            
        }

        public void OnPrevious(InputAction.CallbackContext context)
        {
            
        }

        public void OnNext(InputAction.CallbackContext context)
        {
            
        }
    }
}
