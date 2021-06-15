using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


public abstract class Agent
{
    public Dictionary<string,ValueFunction> valueFunctions=new Dictionary<string, ValueFunction>();
    public Dictionary<string, QFunction> qFunctions = new Dictionary<string, QFunction>();
    public Dictionary<string, PolicyFunction> policyFunctions = new Dictionary<string, PolicyFunction>();

    //action��ά��
    protected int nActions;

    //agent��һ����Ϸ������Ĵ���
    public int episode=1;
    //�ܹ��漸����Ϸ
    public int maxEpisode=10000;
    //��Ϸ�Ƿ����
    public bool gameEnd=false;
    //ÿ���غ�reportһ��agent�ı���
    public int reportPerEpisode=1000;
    //�ж��Ƿ�report�ı���
    protected int reportEpisodeCount;
    //��¼��reportʱtotalreward��ֵ
    protected float rewardCount;

    public float totalReward;
    public int currentAction;
    public string currentState;
    public float instantReward;

    //���ڴ洢AIģ�͵Ĵ洢��
    public AgentStorager storage;

    public delegate void AgentHandler();
    //ÿ�غ�ʱҪ������Щ��Ϊ
    public event AgentHandler OnEndRound;
    //ÿ����Ϸ����ʱҪ������Щ��Ϊ
    public event AgentHandler OnEndEpisode;

    //��ʼ��
    public virtual void Initiate(int maxEpisode,int nActions)
    {
        this.maxEpisode = maxEpisode;
        this.nActions = nActions;
        gameEnd = false;
        try
        {
            storage.Load();
        }
        catch
        {
            Debug.LogWarning("�������Ѿ���ѵ����ģ�ͣ�");
        }
    }

    //����state����policy
    public virtual float[] PolicyFromState(string state)
    {
        if (policyFunctions.ContainsKey(state))
        {
            return (policyFunctions[state].policy);
        }
        else
        {
            Debug.LogAssertion("�Ҳ�����Ӧ��policy��");
            return null;
        }
    }

    //greedy policy improvement
    public virtual void PolicyImprovement(string state,float epsilon)
    {
        List<float> qS = new List<float>();
        List<int> maxQsIndex = new List<int>();
        for (int i = 0; i < nActions; i++)
        {
            qS.Add(qFunctions[state + i.ToString()].value);
        }
        List<float> temp=new List<float>(qS);
        temp.Sort();
        float maxQ = temp[nActions-1];
        for (int i = 0; i < nActions; i++)
        {
            if (qS[i] == maxQ)
            {
                maxQsIndex.Add(i);
            }
        }
        int maxN = maxQsIndex.Count;
        float[] policy = new float[nActions];
        for (int i = 0; i < policy.Length; i++)
        {
            if (maxQsIndex.Count > 0)
            {
                if (i == maxQsIndex[0])
                {
                    policy[i] = (1 - epsilon) / maxN;
                    maxQsIndex.RemoveAt(0);
                }
                else
                {
                    policy[i] = episode / (nActions - maxN);
                }
            }
            else
            {
                policy[i] = episode / (nActions - maxN);
            }
        }
    }

    //angent����Ϸ����ʱ����Ϊ
    public virtual void Play()
    {
        if (episode > maxEpisode) gameEnd = true;
    }

    //agent learningʱ����
    public virtual void Learning()
    {
        if (gameEnd)
        {
            storage.Save();
            Debug.LogWarning("������ɣ�");
            foreach (PolicyFunction p in policyFunctions.Values)
            {
                Debug.LogWarning("state" + p.state + "��ѡstick�ĸ���" + p.policy[1].ToString());
            }
        }
    }


    //ÿ����Ϸ����ʱ����
    public virtual void EndEpisode()
    {
        episode += 1;
        instantReward = 0;
        currentAction = 0;
        currentState = "";
        reportEpisodeCount++;
        if (reportEpisodeCount >= reportPerEpisode)
        {
            reportEpisodeCount = 0;
            Debug.LogWarning("��"+reportPerEpisode+"�غ��У�agent��totalreward��:"+(totalReward-rewardCount).ToString());
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

    //agent��õ���reward
    public virtual void GetReward(float reward)
    {
        instantReward = reward;
        totalReward += instantReward;
    }
}
