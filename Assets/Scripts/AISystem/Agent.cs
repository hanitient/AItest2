using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class Agent
{
    public Dictionary<int[],float> values= new Dictionary<int[], float>();
    public Dictionary<int[],float[]> policies= new Dictionary<int[], float[]>();
    
    public void UpdateValue(int[] state,float value)
    {

    }
    public void UpdatePolicy(int[] state, float[] policy)
    {

    }
    public float ValueFunction(int[] state)
    {
        if (values.ContainsKey(state))
        {
            return (values[state]);
        }
        else
        {
            Debug.LogAssertion("找不到对应的valuefunction！");
            return 0f;
        }
    }
    public float[] PolicyFunction(int[] state)
    {
        if (policies.ContainsKey(state))
        {
            return (policies[state]);
        }
        else
        {
            Debug.LogAssertion("找不到对应的valuefunction！");
            return null;
        }
    }
}

public enum Action
{
    Stick, Hit
}