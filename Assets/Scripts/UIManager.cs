using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager :Singleton<UIManager>
{
    public Button hitButton;
    public Button stickButton;
    public Text playerTotalRewardTxt;
    public Text playerValueSumTxt;
    public Text dealerValueSumTxt;
    public Text playerBaseCardTxt;
    public Text dealerBaseCardTxt;
    public Text episodeTxt;

    public void Hit()
    {
        GameManager.Instance.CardFactory.ReclaimAll();
        Card c = GameManager.Instance.CardFactory.GetRandomCard();
        if (c.Color == CardColor.Red)
        {
            Player.Instance.valueSum -= c.Value;
        }else if (c.Color == CardColor.Black)
        {
            Player.Instance.valueSum += c.Value;
        }
       UpdateUI();
        if (Player.Instance.valueSum > 21)
        {
            GameStateManager.Instance.switchToState(GameState.CountingState);
        }
    }
    public void Stick()
    {
        GameStateManager.Instance.switchToState(GameState.CountingState);
    }
    public void UpdateUI()
    {
        playerValueSumTxt.text = "player本局点数：" + Player.Instance.valueSum.ToString();
        dealerValueSumTxt.text = "dealer本局点数：" + Dealer.Instance.valueSum.ToString();
        playerTotalRewardTxt.text = "总得分：" + Player.Instance.totalReward.ToString();
        episodeTxt.text = "当前回合数：" + GameStateManager.Instance.Episode.ToString();
    }
}
