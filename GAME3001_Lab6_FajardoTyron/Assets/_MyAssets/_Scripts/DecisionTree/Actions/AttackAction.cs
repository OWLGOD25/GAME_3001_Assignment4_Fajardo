using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackAction : ActionNode
{
    public AttackAction()
    {
        name = this.GetType().Name;
    }

    public override void Action()
    {
        if (Agent.GetComponent<AgentObject>().state != ActionState.ATTACK)
        {
            Debug.Log("Starting " + name);
            AgentObject ao = Agent.GetComponent<AgentObject>();
            ao.state = ActionState.ATTACK;
        }
        Debug.Log("Performing " + name);
    }
}
