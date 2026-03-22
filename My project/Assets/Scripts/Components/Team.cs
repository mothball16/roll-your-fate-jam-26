using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Components
{
    public enum TeamType
    {
        Player,
        Enemy,
        None
    }
    public class Team : MonoBehaviour
    {
        public TeamType team;
    }
}
