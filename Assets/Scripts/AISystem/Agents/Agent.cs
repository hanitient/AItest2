using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


public abstract class Agent
{
    public Dictionary<string,ValueFunction> valueFunctions=new Dictionary<string, ValueFunction>();
    public Dictionary<string, QFunction> qFunctions = new Dictionary<string, QFunction>();
    public Dictionary<string, PolicyFunction> policyFunctions = new Dictionary<string, PolicyFunction>();

    //action的维度
    protected int nActions;

    //agent在一次游戏中游玩的次数
    public int episode=1;
    //总共玩几次游戏
    public int maxEpisode;
    //游戏是否结束
    public bool gameEnd=false;
    //每几回合report一次agent的表现
    public int reportPerEpisode=1000;
    //判断是否report的变量
    protected int reportEpisodeCount;
    //记录在report时totalreward的值
    protected float rewardCount;

    public float totalReward;
    public int currentAction;
    public int nextAction;
    public string currentState;
    public float instantReward;

    public string nextState;
    public bool episodeEnd = false;
    public float stepSize = 0.1f;
    //用于存储AI模型的存储器
    public AgentStorager storage;

    public delegate void AgentHandler();
    //每回合时要调用哪些行为
    public event AgentHandler OnEndRound;
    //每次游戏结束时要调用那些行为
    public event AgentHandler OnEndEpisode;

    public float discount = 1f;

    //初始化
    public virtual void Initiate(int maxEpisode,int nActions, List<string> states)
    {
        this.maxEpisode = maxEpisode;
        this.nActions = nActions;
        gameEnd = false;
        foreach (string state in states)
        {
            for (int k = 0; k < nActions; k++)
            {
                QFunction q = new QFunction();
                q.state = state;
                q.action = k;
                q.value = 0;
                q.counts = 0;
                qFunctions.Add(state + q.action.ToString(), q);
            }
            PolicyFunction p = new PolicyFunction();
            p.state = state;
            p.policy = new float[nActions];
            for (int g = 0; g < nActions; g++)
            {
                p.policy[g] = 1f / nActions;
            }
            policyFunctions.Add(state, p);
        }
        try
        {
           storage.Load();
        }
        catch
        {
            Debug.LogWarning("不存在已经过训练的模型！");
        }
    }

    //根据state返回policy
    public virtual float[] PolicyFromState(string state)
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

    //根据state获得所有对应的qfunction
    protected List<float> GetQS(string state)
    {
        List<float> qS = new List<float>();
        for (int i = 0; i < nActions; i++)
        {
            qS.Add(qFunctions[state + i.ToString()].value);
        }
        return qS;
    }
    //在对应一个state的所有qfunction中选出最大value的那个
    protected List<int> GetMaxQS(List<float> qS)
    {
        List<int> maxQsIndex = new List<int>();
        List<float> temp = new List<float>(qS);
        temp.Sort();
        float maxQ = temp[nActions - 1];
        for (int i = 0; i < nActions; i++)
        {
            if (qS[i] == maxQ)
            {
                maxQsIndex.Add(i);
            }
        }
        return maxQsIndex;
    }
    //greedy policy improvement
    public virtual void PolicyImprovement(string state,float epsilon)
    {
        List<float> qS = GetQS(state);
        List<int> maxQsIndex = GetMaxQS(qS);
        int maxN = maxQsIndex.Count;
        float[] policy = new float[nActions];
        for (int i = 0; i < policy.Length; i++)
        {
            policy[i] = epsilon / nActions;
            if (maxQsIndex.Count > 0)
            {
                if (i == maxQsIndex[0])
                {
                    policy[i] += (1 - epsilon) / maxN;
                    maxQsIndex.RemoveAt(0);
                }
            }
        }
        policyFunctions[state].policy = policy;
    }

    //angent在游戏进行时的行为
    public virtual void Play()
    {
        if (episode > maxEpisode) gameEnd = true;
    }

    //agent learning时调用
    public virtual void Learning()
    {
        if (gameEnd)
        {
            //foreach (PolicyFunction p in policyFunctions.Values)
            //{
            //    if (p.policy[1] != 0.5f) Debug.LogWarning("state:" + p.state + " strick:" + p.policy[1]);
            //}
            storage.Save();
            Debug.LogWarning("model储存完成！");
        }
    }


    //每次游戏结束时调用
    public virtual void EndEpisode()
    {
        episode += 1;
        instantReward = 0;
        currentAction = 0;
        currentState = "";
        nextAction = -1;
        reportEpisodeCount++;
        if (reportEpisodeCount >= reportPerEpisode)
        {
            reportEpisodeCount = 0;
            Debug.LogWarning("这"+reportPerEpisode+"回合中，agent的totalreward是:"+(totalReward-rewardCount).ToString());
            rewardCount = totalReward;
        }
        if (OnEndEpisode != null)
        {
            OnEndEpisode();
        }
    }

    public virtual void EndRound()
    {
        if (OnEndRound != null)
        {
            OnEndRound();
        }
    }

    //agent获得当期reward
    public virtual void GetReward(float reward)
    {
        instantReward = reward;
        totalReward += instantReward;
    }

    public virtual void CountingInstantReward()
    {

    }
}
