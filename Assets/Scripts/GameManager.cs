using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] 
    CardFactory cardFactory;

    FTMCAgent agent=new FTMCAgent();
    Dealer dealer = new Dealer();
    //ÓÎÏ·µÄ½×¶Î
    GameState gameState;

    public FTMCAgent Agent { get => agent;}
    public Dealer Dealer { get => dealer; }
    public GameState GameState { get => gameState; }
    // Start is called before the first frame update
    void Start()
    {
        cardFactory.Initiate();
        agent.Initiate(1000,2);
        gameState = GameState.DealerState;
        EnterIntoState(GameState.DealerState);
    }

    // Update is called once per frame
    void Update()
    { 
        agent.Play();
    }

    private void EnterIntoState(GameState gs)
    {
        cardFactory.ReclaimAll();
        switch (gs)
        {
            case GameState.PlayerState:
                UIManager.Instance.hitButton.gameObject.SetActive(true);
                UIManager.Instance.stickButton.gameObject.SetActive(true);
                break;
            case GameState.DealerState:
                InitiateEpisode();
                switchToState(GameState.PlayerState);
                break;
            case GameState.CountingState:
                CountingEpisode();
                UIManager.Instance.UpdateUI();
                agent.Learning();
                if (!agent.gameEnd)
                {
                    switchToState(GameState.DealerState);
                }
                break;
        }
    }

    private void ExitFromState(GameState gs)
    {
        switch (gs)
        {
            case GameState.PlayerState:
                UIManager.Instance.hitButton.gameObject.SetActive(false);
                UIManager.Instance.stickButton.gameObject.SetActive(false);
                break;
            case GameState.DealerState:
                break;
            case GameState.CountingState:
                agent.EndEpisode();
                break;
        }
    }
    private void switchToState(GameState gs)
    {
        UIManager.Instance.UpdateUI();
        ExitFromState(GameState);
        gameState = gs;
        EnterIntoState(GameState);
    }
    public void Hit()
    {
        cardFactory.ReclaimAll();
        Card c = cardFactory.GetRandomCard();
        if (c.Color == CardColor.Red)
        {
            agent.valueSum += c.Value;
        }
        else if (c.Color == CardColor.Black)
        {
            agent.valueSum += c.Value;
        }
        UIManager.Instance.UpdateUI();

        if (agent.valueSum > 21||agent.valueSum<-20)
        {
            switchToState(GameState.CountingState);
            return;
        }
        agent.EndRound();
    }
    public void Stick()
    {
        switchToState(GameState.CountingState);
    }

    private void InitiateEpisode()
    {
        dealer.valueSum = 0;
        agent.valueSum = 0;
        Card playerBase = cardFactory.Get(Random.Range(1, 10), CardColor.Black);
        UIManager.Instance.playerBaseCardTxt.text = "Íæ¼Òµ×ÅÆ£º" + playerBase.Color.ToString() + " " + playerBase.Value.ToString();
        Card dealerBase = cardFactory.Get(Random.Range(1, 10), CardColor.Black);
        UIManager.Instance.dealerBaseCardTxt.text = "dealerµ×ÅÆ£º" + dealerBase.Color.ToString() + " " + dealerBase.Value.ToString();
        agent.valueSum += playerBase.Value;
        dealer.valueSum += dealerBase.Value;
        while (dealer.valueSum < 10)
        {
            Card c = cardFactory.GetRandomCard();
            if (c.Color == CardColor.Red)
            {
                dealer.valueSum += c.Value;
            }
            else if (c.Color == CardColor.Black)
            {
                dealer.valueSum += c.Value;
            }
        }
    }

    private void CountingEpisode()
    {
        if (agent.valueSum > 21 && dealer.valueSum > 21)
        {
            agent.GetReward(0);
        }
        else
        {
            if (agent.valueSum > 21) agent.GetReward (- 1);
            else if (dealer.valueSum > 21) agent.GetReward(1);
            else
            {
                if (agent.valueSum > dealer.valueSum) agent.GetReward(1);
                if (agent.valueSum < dealer.valueSum) agent.GetReward(-1);
            }
        }
        agent.EndRound();
    }

}
