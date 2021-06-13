using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager:Singleton<GameStateManager>
{
    GameState gameState = GameState.DealerState;
    public GameState GameState { get => gameState; set => gameState = value; }
    int episode=1;
    public int Episode { get => episode; set => episode = value; }
    public void EnterIntoState(GameState gs)
    {
        GameManager.Instance.CardFactory.ReclaimAll();
        switch (gs)
        {
            case GameState.PlayerState:
                Debug.Log("½øÈëplayerstate");
                UIManager.Instance.hitButton.gameObject.SetActive(true);
                UIManager.Instance.stickButton.gameObject.SetActive(true);
                break;
            case GameState.DealerState:
                Dealer.Instance.valueSum = 0;
                Player.Instance.valueSum = 0;
                Card playerBase = GameManager.Instance.CardFactory.Get(Random.Range(1, 10), CardColor.Black);
                UIManager.Instance.playerBaseCardTxt.text = "Íæ¼Òµ×ÅÆ£º" + playerBase.Color.ToString() + " " + playerBase.Value.ToString();
                Card dealerBase = GameManager.Instance.CardFactory.Get(Random.Range(1, 10), CardColor.Black);
                UIManager.Instance.dealerBaseCardTxt.text = "dealerµ×ÅÆ£º" + dealerBase.Color.ToString() + " " + dealerBase.Value.ToString();
                while (Dealer.Instance.valueSum < 17)
                {
                    Card c = GameManager.Instance.CardFactory.GetRandomCard();
                    if (c.Color == CardColor.Red)
                    {
                        Dealer.Instance.valueSum -= c.Value;
                    }
                    else if (c.Color == CardColor.Black)
                    {
                        Dealer.Instance.valueSum += c.Value;
                    }
                }

                switchToState(GameState.PlayerState);
                break;
            case GameState.CountingState:
                if (Player.Instance.valueSum > 21 && Dealer.Instance.valueSum > 21)
                {

                }
                else
                {
                    if (Player.Instance.valueSum > 21) Player.Instance.totalReward -= 1;
                    else if(Dealer.Instance.valueSum>21) Player.Instance.totalReward += 1;
                    else
                    {
                        if(Player.Instance.valueSum>Dealer.Instance.valueSum) Player.Instance.totalReward += 1;
                        if(Player.Instance.valueSum < Dealer.Instance.valueSum) Player.Instance.totalReward -= 1;
                    }
                }
                UIManager.Instance.UpdateUI();
                switchToState(GameState.DealerState);
                break;
        }
    }

    public void ExitFromState(GameState gs)
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
                    Player.Instance.totalReward = 0;
                }
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
}
