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

    public void UpdateUI()
    {
        playerValueSumTxt.text = "player本局点数：" + GameManager.Instance.Agent.valueSum.ToString();
        dealerValueSumTxt.text = "dealer本局点数：" + GameManager.Instance.Dealer.valueSum.ToString();
        playerTotalRewardTxt.text = "总得分：" + GameManager.Instance.Agent.totalReward.ToString();
        episodeTxt.text = "当前回合数：" + GameManager.Instance.Agent.episode.ToString();
    }
}
