using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LOSCondition : ConditionNode
{
    public bool HasLOS { get; set; }

    public LOSCondition()
    {
        name = "LOS Condition";
        HasLOS = false;
    }

    public override bool Condition() // Checked within our MakeDecision method of tree.
    {
        Debug.Log("Checking " + name);
        return HasLOS;
    }
}
