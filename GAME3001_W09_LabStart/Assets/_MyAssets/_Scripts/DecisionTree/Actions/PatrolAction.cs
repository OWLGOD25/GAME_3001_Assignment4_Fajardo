using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolAction : ActionNode
{
    public PatrolAction()
    {
        name = this.GetType().Name; // Doing this just to demo for actions.
    }

    public override void Action()
    {
        if (Agent.GetComponent<AgentObject>().state != ActionState.PATROL) // Serves as an "Enter" for the state
        {
            Debug.Log("Starting " + name);
            Starship ss = Agent.GetComponent<Starship>();
            ss.state = ActionState.PATROL;
            ss.StartPatrol();
        }
        Debug.Log("Performing " + name);
    }
}
