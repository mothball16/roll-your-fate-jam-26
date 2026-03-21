using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Components
{
    enum TeamType
    {
        Player,
        Enemy,
        None
    }
    internal class Team : MonoBehaviour
    {
        public TeamType team;
    }
}
