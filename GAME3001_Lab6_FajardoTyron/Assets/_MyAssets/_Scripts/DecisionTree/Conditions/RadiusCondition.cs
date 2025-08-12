using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadiusCondition : ConditionNode
{
    public bool IsWithinRadius { get; set; } // <node>.IsWithinRadius = <distance check>;

    public RadiusCondition()
    {
        name = "Radius Condition";
        IsWithinRadius = false;
    }

    public override bool Condition() // Checked within our MakeDecision method of tree.
    {
        Debug.Log("Checking " + name);
        return IsWithinRadius;
    }
}
