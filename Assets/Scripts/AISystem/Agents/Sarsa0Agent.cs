using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sarsa0Agent : Agent
{
    public int valueSum = 0;


    public override void Initiate(int maxEpisode, int nActions, List<string> states)
    {
        storage = new AgentStorager(this, "TD");
        base.Initiate(maxEpisode, nActions,states);
    }

    public override void Play()
    {
        if (GameManager.Instance.GameState == GameState.PlayerState)
        {
            currentState = valueSum.ToString() + GameManager.Instance.Dealer.valueSum.ToString();
            if (nextAction != -1) currentAction = nextAction;
            else { currentAction = StaticData.GetRandomElement(PolicyFromState(currentState)); }
                if (currentAction == 0)
            {
                GameManager.Instance.Hit();
            }
            else
            {
                GameManager.Instance.Stick();
            }
            //想想怎么精简这一句
            nextState = valueSum.ToString() + GameManager.Instance.Dealer.valueSum.ToString();
        }
        base.Play();
    }


    //sarsa0 control algorithm
    public override void Learning()
    {
        string currentSA = currentState + currentAction.ToString();
        nextAction= StaticData.GetRandomElement(PolicyFromState(currentState));
        string nextSA = nextState+nextAction.ToString();
        qFunctions[currentSA].value+=stepSize*(instantReward+qFunctions[nextSA].value-qFunctions[currentSA].value*discount);
        PolicyImprovement(currentState,0.1f);
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
