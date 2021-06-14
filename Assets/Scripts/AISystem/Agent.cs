using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Agent
{
    public Dictionary<string,ValueFunction> valueFunctions=new Dictionary<string, ValueFunction>();
    public Dictionary<string, QFunction> qFunctions = new Dictionary<string, QFunction>();
    public Dictionary<string, PolicyFunction> policyFunctions = new Dictionary<string, PolicyFunction>();
    public int valueSum = 0;
    public float totalReward = 0;

    int currentAction;
    string currentState;
    public float instantReward;

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

    public void Initiate()
    {
        for (int i = 1; i <= 30; i++)
        {
            for (int j = 10; j <= 20; j++)
            {
                string state= i.ToString() + j.ToString();
                for (int k = 0; k < 2; k++)
                {
                    QFunction q = new QFunction();
                    q.state = state;
                    q.action = k;
                    q.value = 0;
                    q.counts = 0;
                    qFunctions.Add(state+q.action.ToString(), q);
                }
                PolicyFunction p = new PolicyFunction();
                p.state = state;
                p.policy = new float[] { 0.5f, 0.5f };
                policyFunctions.Add(state, p);
            }
        }
    }
    public void StartNewEpisode()
    {
        instantReward = 0;
        currentAction = 0;
        currentState = "";
    }
    //public void UpdateValue(int state,float value)
    //{
    //    values[state] = value;
    //}
    //public void UpdatePolicy(int state, float[] policy)
    //{
    //    policies[state] = policy;
    //}

    public float[] PolicyFunction(string state)
    {
        if (policyFunctions.ContainsKey(state))
        {
            return (policyFunctions[state].policy);
        }
        else
        {
            Debug.LogAssertion("找不到对应的policy！");
            return null;
        }
    }
    public void AgentPlay()
    {
        if (GameManager.Instance.GameState==GameState.PlayerState)
        {
            currentState = valueSum.ToString() + GameManager.Instance.Dealer.valueSum.ToString();
            currentAction = StaticData.GetRandomElement(PolicyFunction(currentState));
            if (currentAction == 0)
            {
                GameManager.Instance.Hit();
            }
            else
            {
                GameManager.Instance.Stick();
            }
        }
    }
    public void AgentRecordInEpisode()
    {
        eStateRecord.Add(currentState);
        eActionRecord.Add(currentAction);
        eRewardRecord.Add(instantReward);
        eStateActionRecord.Add(currentState+currentAction.ToString());
    }
    public void AgentRecordEndEpisode()
    {
        AgentRecordInEpisode();
        rewardRecord.Add(eRewardRecord);
        stateRecord.Add(eStateRecord);
        actionRecord.Add(eActionRecord);
        stateActionRecord.Add(eStateActionRecord);
        eRewardRecord = new List<float>();
        eStateRecord = new List<string>();
        eActionRecord = new List<int>();
        eStateActionRecord = new List<string>();
    }
    public void PolicyImprovement(string state)
    {
        float q1 = qFunctions[state+0.ToString()].value;
        float q2 = qFunctions[state+1.ToString()].value;
        if (q1 > q2)
        {
            policyFunctions[state].policy = new float[] { 0.95f, 0.05f };
        }else if (q1 < q2)
        {
            policyFunctions[state].policy = new float[] { 0.05f, 0.95f };
        }
        else
        {
            policyFunctions[state].policy = new float[] { 0.5f, 0.5f };
        }
    }
    //first-visit Monte Carlo Q-function evaluation
    public void FVMCLearning()
    {
        for(int index=0;index<rewardRecord.Count;index++)
        {
            int lastIndex = rewardRecord[index].Count - 1;
            float G = 0;
            if (lastIndex > 0)
            {
                for (int i = lastIndex; i >= 0; i--)
                {
                    //Debug.LogWarning(i+" "+l.Count);
                    G += rewardRecord[index][i];
                    if (stateActionRecord[index][i] == stateActionRecord[index][lastIndex])
                    {
                        qFunctions[stateActionRecord[index][i]].counts += 1;
                        qFunctions[stateActionRecord[index][i]].value += G / qFunctions[stateActionRecord[index][i]].counts;
                        PolicyImprovement(stateRecord[index][i]);
                        G = 0;
                    }
                }
            }

        }
    }
}
