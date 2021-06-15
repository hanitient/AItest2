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
        playerValueSumTxt.text = "player���ֵ�����" + GameManager.Instance.Agent.valueSum.ToString();
        dealerValueSumTxt.text = "dealer���ֵ�����" + GameManager.Instance.Dealer.valueSum.ToString();
        playerTotalRewardTxt.text = "�ܵ÷֣�" + GameManager.Instance.Agent.totalReward.ToString();
        episodeTxt.text = "��ǰ�غ�����" + GameManager.Instance.Agent.episode.ToString();
    }
}
