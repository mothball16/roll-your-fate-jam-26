using DangryGames;
using UnityEngine;


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
        _wantToDestroyOnLoad = false;
        base.Awake();
    }



    

}