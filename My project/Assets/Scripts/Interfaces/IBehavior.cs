using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
namespace Assets.Scripts.Interfaces
{
    public interface IBehavior
    {
        public Vector3 GetVelocity(CharacterController character, float dt);
    }
}
