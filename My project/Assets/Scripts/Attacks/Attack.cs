#nullable enable
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Attacks
{
    public enum AimType
    {
        RandomInRange,

        Closest,
        Direction,
        Mouse,
        North
    }

    public abstract class Attack : ScriptableObject 
    {
        public string Name = "n/a";
        public AimType AimType = AimType.Direction;
        public float Cooldown = 1;
        public abstract void Execute(GameObject owner, Vector3 direction, GameObject? target = null);
    }
}
