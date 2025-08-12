using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Starship : AgentObject
{
    [SerializeField] Transform[] patrolPoints;
    [SerializeField] float pointRadius; 
    [SerializeField] float movementSpeed;
    [SerializeField] float rotationSpeed;
    [SerializeField] float detectionRange;
    [SerializeField] float attackRange;

    [SerializeField] Transform LOSTarget;
    [SerializeField] float whiskerLength;
    [SerializeField] float whiskerAngle;
    // [SerializeField] float avoidanceWeight;
    private Rigidbody2D rb;
    
    // Decision Tree fields.
    private DecisionTree dt;

    private int patrolIndex; // Which patrol point is current one.

    new void Start() // Note the new.
    {
        base.Start(); // Explicitly invoking Start of AgentObject.
        Debug.Log("Starting Starship.");
        rb = GetComponent<Rigidbody2D>();
        
        // Construct and build tree and nodes.
        dt = new DecisionTree(this.gameObject);
        BuildTree();
        
        patrolIndex = 0;
    }

    void Update()
    {
        Vector2 direction = (LOSTarget.position - transform.position).normalized;
        float angleInRadians = Mathf.Atan2(direction.y, direction.x);
        whiskerAngle = angleInRadians * Mathf.Rad2Deg;
        whiskerLength = Vector3.Distance(LOSTarget.position, transform.position);
        bool hit = CastWhisker(whiskerAngle, Color.red);

        // Set the RadiusCondition on the tree.
        dt.RadiusNode.IsWithinRadius = whiskerLength <= detectionRange;

        // Set the LOSCondition based on hit.
        dt.LOSNode.HasLOS = hit;

        // Set the CloseCombatCondition on the tree.
        dt.CloseCombatNode.IsWithinRange = Vector3.Distance(LOSTarget.position, transform.position) <= attackRange;

        // Make Decision and parse the state.
        dt.MakeDecision();
        Debug.Log("Current state: " + state);
        switch (state)
        {
            case ActionState.PATROL:
                SeekForward(); // This is temporarily for our patrol.
                break;
            default: // Unhandled state.
                rb.linearVelocity = Vector3.zero;
                break;
        }
    }

    //private void AvoidObstacles()
    //{
    //    // Cast whiskers to detect obstacles
    //    bool hitLeft = CastWhisker(whiskerAngle, Color.red);
    //    bool hitRight = CastWhisker(-whiskerAngle, Color.blue);

    //    // Adjust rotation based on detected obstacles
    //    if (hitLeft)
    //    {
    //        // Rotate counterclockwise if the left whisker hit
    //        RotateClockwise();
    //    }
    //    else if (hitRight && !hitLeft)
    //    {
    //        // Rotate clockwise if the right whisker hit
    //        RotateCounterClockwise();
    //    }
    //}

    //private void RotateCounterClockwise()
    //{
    //    // Rotate counterclockwise based on rotationSpeed and a weight.
    //    transform.Rotate(Vector3.forward, rotationSpeed * avoidanceWeight * Time.deltaTime);
    //}

    //private void RotateClockwise()
    //{
    //    // Rotate clockwise based on rotationSpeed.
    //    transform.Rotate(Vector3.forward, -rotationSpeed * avoidanceWeight * Time.deltaTime);
    //}

    private bool CastWhisker(float angle, Color color)
    {
        bool hitResult = false;
        Color rayColor = color;

        // Calculate the direction of the whisker.
        Vector2 whiskerDirection = Quaternion.Euler(0, 0, angle) * Vector2.right;

        if (HasLOS(gameObject, LOSTarget.tag, whiskerDirection, whiskerLength))
        {
            // Debug.Log("Obstacle detected!");
            rayColor = Color.green;
            hitResult = true;
        }

        // Debug ray visualization
        Debug.DrawRay(transform.position, whiskerDirection * whiskerLength, rayColor);
        return hitResult;
    }

    private void SeekForward() // A seek with rotation to target but only moving along forward vector.
    {
        // Calculate direction to the target.
        Vector2 directionToTarget = (TargetPosition - transform.position).normalized;

        // Calculate the angle to rotate towards the target.
        float targetAngle = Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg + 90.0f; // Note the +90 when converting from Radians.

        // Smoothly rotate towards the target.
        float angleDifference = Mathf.DeltaAngle(targetAngle, transform.eulerAngles.z);
        float rotationStep = rotationSpeed * Time.deltaTime;
        float rotationAmount = Mathf.Clamp(angleDifference, -rotationStep, rotationStep);
        transform.Rotate(Vector3.forward, rotationAmount);

        // Move along the forward vector using Rigidbody2D.
        rb.linearVelocity = transform.up * movementSpeed;

        if (Vector3.Distance(transform.position, TargetPosition) <= pointRadius)
        { // We're within distance of the current wp, change to next
            m_target = GetNextPatrolPoint();
        }
    }

    public void StartPatrol()
    {
        m_target = patrolPoints[patrolIndex];
    }

    private Transform GetNextPatrolPoint()
    {
        patrolIndex++;
        if (patrolIndex == patrolPoints.Length)
        {
            patrolIndex = 0;
        }
        return patrolPoints[patrolIndex];
    }

    //private void OnTriggerEnter2D(Collider2D other)
    //{
    //    if (other.gameObject.tag == "Target")
    //    {
    //        GetComponent<AudioSource>().Play();
    //    }
    //}

    // TODO: Fill in for Lab 7a.
    private void BuildTree()
    {
        // Root condition node.
        dt.RadiusNode = new RadiusCondition();
        dt.treeNodeList.Add(dt.RadiusNode);

        // Second level.

        // PatrolAction leaf.
        TreeNode patrolNode = dt.AddNode(dt.RadiusNode, new PatrolAction(), TreeNodeType.LEFT_TREE_NODE);
        ((ActionNode)patrolNode).Agent = dt.Agent; // this.GameObject.
        dt.treeNodeList.Add(patrolNode);

        // LOSCondition node.
        dt.LOSNode = new LOSCondition();
        dt.treeNodeList.Add(dt.AddNode(dt.RadiusNode, dt.LOSNode, TreeNodeType.RIGHT_TREE_NODE));

        // Third level.

        // MoveToLOSAction leaf.
        TreeNode moveToLOSNode = new MoveToLOSAction(); // Create the node.
        dt.AddNode(dt.LOSNode, moveToLOSNode, TreeNodeType.LEFT_TREE_NODE); // Connect the node to the tree.
        ((ActionNode)moveToLOSNode).Agent = dt.Agent;
        dt.treeNodeList.Add(moveToLOSNode); // Adding node to the list.

        // CloseCombatCondition node.
        dt.CloseCombatNode = new CloseCombatCondition();
        dt.treeNodeList.Add(dt.AddNode(dt.LOSNode, dt.CloseCombatNode, TreeNodeType.RIGHT_TREE_NODE));

        // Fourth level.

        // MoveToPlayerAction leaf.
        TreeNode moveToPlayerNode = dt.AddNode(dt.CloseCombatNode, new MoveToPlayerAction(), TreeNodeType.LEFT_TREE_NODE);
        ((ActionNode)moveToPlayerNode).Agent = dt.Agent;
        dt.treeNodeList.Add(moveToPlayerNode);

        // AttackAction leaf.
        TreeNode attackNode = dt.AddNode(dt.CloseCombatNode, new AttackAction(), TreeNodeType.RIGHT_TREE_NODE);
        ((ActionNode)attackNode).Agent = dt.Agent;
        dt.treeNodeList.Add(attackNode);
    }
}
