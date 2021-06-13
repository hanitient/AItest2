using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] 
    CardFactory cardFactory;
    Agent states;

    public CardFactory CardFactory { get => cardFactory; private set { } }
    public Agent States { get => states; set => states = value; }

    // Start is called before the first frame update
    void Start()
    {
        CardFactory.Initiate();
        GameStateManager.Instance.EnterIntoState(GameState.DealerState);
    }

    // Update is called once per frame
    void Update()
    {

    }



}
