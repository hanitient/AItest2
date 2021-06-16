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
    public int maxEpisode;
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
    public int nextAction;
    public string currentState;
    public float instantReward;

    public string nextState;
    public bool episodeEnd = false;
    public float stepSize = 0.1f;
    //���ڴ洢AIģ�͵Ĵ洢��
    public AgentStorager storage;

    public delegate void AgentHandler();
    //ÿ�غ�ʱҪ������Щ��Ϊ
    public event AgentHandler OnEndRound;
    //ÿ����Ϸ����ʱҪ������Щ��Ϊ
    public event AgentHandler OnEndEpisode;

    public float discount = 1f;

    //��ʼ��
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

    //����state������ж�Ӧ��qfunction
    protected List<float> GetQS(string state)
    {
        List<float> qS = new List<float>();
        for (int i = 0; i < nActions; i++)
        {
            qS.Add(qFunctions[state + i.ToString()].value);
        }
        return qS;
    }
    //�ڶ�Ӧһ��state������qfunction��ѡ�����value���Ǹ�
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
            //foreach (PolicyFunction p in policyFunctions.Values)
            //{
            //    if (p.policy[1] != 0.5f) Debug.LogWarning("state:" + p.state + " strick:" + p.policy[1]);
            //}
            storage.Save();
            Debug.LogWarning("model������ɣ�");
        }
    }


    //ÿ����Ϸ����ʱ����
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

    public virtual void CountingInstantReward()
    {

    }
}
