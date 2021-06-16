using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FTMCAgent : Agent
{
    /** 用于Monte Carlo模拟的数据记录 **/
    public List<List<int>> actionRecord = new List<List<int>>();
    public List<List<float>> rewardRecord = new List<List<float>>();
    public List<List<string>> stateRecord = new List<List<string>>();
    public List<List<string>> stateActionRecord = new List<List<string>>();

    public List<int> eActionRecord = new List<int>();
    public List<float> eRewardRecord = new List<float>();
    public List<string> eStateRecord = new List<string>();
    public List<string> eStateActionRecord = new List<string>();
    /** end **/
    public int valueSum = 0;

    public override void Initiate(int maxEpisode, int nActions,List<string> states)
    {
        OnEndRound += new AgentHandler(Record);
        storage = new AgentStorager(this, "FTMC");
        base.Initiate(maxEpisode,nActions,states);
    }

    public override void Play()
    {
        if (GameManager.Instance.GameState == GameState.PlayerState)
        {
            currentState = valueSum.ToString() + GameManager.Instance.Dealer.valueSum.ToString();
            currentAction = StaticData.GetRandomElement(PolicyFromState(currentState));
            if (currentAction == 0)
            {
                GameManager.Instance.Hit();
            }
            else
            {
                GameManager.Instance.Stick();
            }
        }
        base.Play();
    }

    public void Record()
    {
        eStateRecord.Add(currentState);
        eActionRecord.Add(currentAction);
        eRewardRecord.Add(instantReward);
        eStateActionRecord.Add(currentState + currentAction.ToString());
        if (GameManager.Instance.GameState == GameState.CountingState)
        {
            rewardRecord.Add(eRewardRecord);
            stateRecord.Add(eStateRecord);
            actionRecord.Add(eActionRecord);
            stateActionRecord.Add(eStateActionRecord);
            eRewardRecord = new List<float>();
            eStateRecord = new List<string>();
            eActionRecord = new List<int>();
            eStateActionRecord = new List<string>();
        }
    }

    //first-visit Monte Carlo control algorithm
    public override void Learning()
    {
        for (int index = 0; index < rewardRecord.Count; index++)
        {
            int lastIndex = rewardRecord[index].Count - 1;
            float G = 0;
            if (lastIndex > 0)
            {
                for (int i = lastIndex; i >= 0; i--)
                {
                    //Debug.LogWarning(i+" "+l.Count);
                    G =discount * G+ rewardRecord[index][i];
                    if (stateActionRecord[index][i] == stateActionRecord[index][lastIndex])
                    {
                        qFunctions[stateActionRecord[index][i]].counts += 1;
                        qFunctions[stateActionRecord[index][i]].value += G / qFunctions[stateActionRecord[index][i]].counts;
                        PolicyImprovement(stateRecord[index][i],0.05f);
                        G = 0;
                    }
                }
            }
        }
        base.Learning();
    }

    public override void CountingInstantReward()
    {
        if (valueSum > 21 && GameManager.Instance.Dealer.valueSum > 21)
        {
            GetReward(0);
        }
        else
        {
            if (valueSum > 21) GetReward(-1);
            else if (GameManager.Instance.Dealer.valueSum > 21) GetReward(1);
            else
            {
                if (valueSum > GameManager.Instance.Dealer.valueSum) GetReward(1);
                if (valueSum < GameManager.Instance.Dealer.valueSum) GetReward(-1);
            }
        }
    }
}
