
using UnityEngine;
using DangryGames;

public enum GameState
{
    Title,
    Game,
    Results
}

class StateManager : MonoSingleton<StateManager>
{
    
    public override void Awake()
    {
        base.Awake();
    }

}