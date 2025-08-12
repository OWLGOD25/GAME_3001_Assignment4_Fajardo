using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseCombatCondition : ConditionNode
{
    public bool IsWithinRange { get; set; }

    public CloseCombatCondition()
    {
        name = "Close Range Condition"; // or this.GetType().Name to convert the class name to string
        IsWithinRange = false;
    }

    public override bool Condition() // Checked within our MakeDecision method of tree.
    {
        Debug.Log("Checking " + name);
        return IsWithinRange;
    }
}
