using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] 
    CardFactory cardFactory;
    Agent agent=new Agent();
    Dealer dealer = new Dealer();
    public CardFactory CardFactory { get => cardFactory; private set { } }
    GameState gameState = GameState.DealerState;
    int episode = 1;
    public int Episode { get => episode; set => episode = value; }
    public Agent Agent { get => agent; set => agent = value; }
    public Dealer Dealer { get => dealer; set => dealer = value; }
    public GameState GameState { get => gameState; set => gameState = value; }

    // Start is called before the first frame update
    void Start()
    {
        CardFactory.Initiate();
        Agent.Initiate();
        EnterIntoState(GameState.DealerState);
    }

    // Update is called once per frame
    void Update()
    {
        switch (GameState)
        {
            case GameState.PlayerState:
                agent.AgentPlay();
                break;
        }
    }

    private void EnterIntoState(GameState gs)
    {
        CardFactory.ReclaimAll();
        switch (gs)
        {
            case GameState.PlayerState:
                UIManager.Instance.hitButton.gameObject.SetActive(true);
                UIManager.Instance.stickButton.gameObject.SetActive(true);
                break;
            case GameState.DealerState:
                Dealer.valueSum = 0;
                Agent.valueSum = 0;
                Card playerBase = CardFactory.Get(Random.Range(1, 10), CardColor.Black);
                UIManager.Instance.playerBaseCardTxt.text = "Íæ¼Òµ×ÅÆ£º" + playerBase.Color.ToString() + " " + playerBase.Value.ToString();
                Card dealerBase = CardFactory.Get(Random.Range(1, 10), CardColor.Black);
                UIManager.Instance.dealerBaseCardTxt.text = "dealerµ×ÅÆ£º" + dealerBase.Color.ToString() + " " + dealerBase.Value.ToString();
                Agent.valueSum += playerBase.Value;
                Dealer.valueSum += dealerBase.Value;
                while (Dealer.valueSum < 10)
                {
                    Card c = CardFactory.GetRandomCard();
                    if (c.Color == CardColor.Red)
                    {
                        Dealer.valueSum += c.Value;
                    }
                    else if (c.Color == CardColor.Black)
                    {
                        Dealer.valueSum += c.Value;
                    }
                }

                switchToState(GameState.PlayerState);
                break;
            case GameState.CountingState:
                if (agent.valueSum > 21 && Dealer.valueSum > 21)
                {
                    agent.instantReward = 0;
                }
                else
                {
                    if (agent.valueSum > 21) agent.instantReward = -1;
                    else if (Dealer.valueSum > 21) agent.instantReward = 1;
                    else
                    {
                        if (agent.valueSum > Dealer.valueSum) agent.instantReward = 1;
                        if (agent.valueSum < Dealer.valueSum) agent.instantReward = -1;
                    }
                }
                agent.AgentRecordEndEpisode();
                agent.totalReward += agent.instantReward;
                UIManager.Instance.UpdateUI();
                if (episode < StaticData.EpisodeLength) switchToState(GameState.DealerState);
                else
                {
                    Debug.LogWarning(agent.stateRecord.Count);
                    Debug.LogWarning(agent.stateActionRecord.Count);
                    agent.FVMCLearning();
                    foreach (QFunction q in agent.qFunctions.Values)
                    {
                        if (q.value != 0) Debug.Log("qfunction:" + q.value);
                    }
                    foreach (PolicyFunction q in agent.policyFunctions.Values)
                    {
                        Debug.Log("policyFunction"+ q.state+" stickµÄ¸ÅÂÊ:"  +q.policy[1]);
                    }
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
                episode += 1;
                if (episode > StaticData.EpisodeLength)
                {
                    episode = 1;
                    agent.totalReward = 0;
                }
                agent.StartNewEpisode();
                break;
        }
    }
    public void switchToState(GameState gs)
    {
        UIManager.Instance.UpdateUI();
        ExitFromState(GameState);
        GameState = gs;
        EnterIntoState(GameState);
    }
    public void Hit()
    {
        CardFactory.ReclaimAll();
        Card c = CardFactory.GetRandomCard();
        if (c.Color == CardColor.Red)
        {
            agent.valueSum += c.Value;
        }
        else if (c.Color == CardColor.Black)
        {
            agent.valueSum += c.Value;
        }
        if (agent.valueSum > 21||agent.valueSum<-20)
        {
            switchToState(GameState.CountingState);
        }
        else
        {
            agent.AgentRecordInEpisode();
        }
        UIManager.Instance.UpdateUI();
    }
    public void Stick()
    {
        switchToState(GameState.CountingState);
    }



}
