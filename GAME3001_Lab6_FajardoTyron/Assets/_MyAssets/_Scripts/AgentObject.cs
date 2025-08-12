using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ActionState
{
    NO_ACTION = -1,
    ATTACK,
    MOVE_TO_LOS,
    MOVE_TO_PLAYER,
    PATROL
};

public class AgentObject : MonoBehaviour
{
    [SerializeField] protected Transform m_target = null;

    public ActionState state { get; set; }
    public Vector3 TargetPosition
    {
        get { return m_target.position; }
        set { m_target.position = value; }
    }
    // Note. I only want the above property here so the class cannot be abstract.

    public void Start()
    {
        Debug.Log("Starting Agent.");
    }

    public bool HasLOS(GameObject source, string targetTag, Vector2 whiskerDirection, float whiskerLength)
    {
        // Set the layer of the source to Ignore Linecast.
        source.layer = 3;

        // Create the layermask for the ship.
        int layerMask = ~(1 << LayerMask.NameToLayer("Ignore Linecast"));

        // Cast a ray in the whisker direction.
        RaycastHit2D hit = Physics2D.Raycast(transform.position, whiskerDirection, whiskerLength, layerMask);

        // Reset the source's layer.
        source.layer = 0;

        if (hit.collider != null && hit.collider.CompareTag(targetTag))
        {
            return true;
        }
        return false;
    }
}
