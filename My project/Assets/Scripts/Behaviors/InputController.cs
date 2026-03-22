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
            var mousePos = Mouse.current.position.ReadValue();
            var propX = mousePos.x / Screen.width;
            var propY = mousePos.y / Screen.height;


            var dir = new Vector3(
                (float)(propX - 0.5) * 2,
                (float)(propY - 0.5) * 2,
                0);

            if(dir.magnitude == 0)
            {
                dir = Vector3.one;
            }

            dir.Normalize();
            _char.SteeringForce = dir * _char.MaxSpeed;
        }

        public void OnSprint(InputAction.CallbackContext context)
        {

        }

        public void OnMove(InputAction.CallbackContext context)
        {
            //_char.SteeringForce = context.ReadValue<Vector2>() * _char.MaxSpeed;



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
