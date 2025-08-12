using UnityEngine;

public class SmartMissile : AgentObject
{
    [Header("Seek Fields")]
    [SerializeField] private float movementSpeed;
    [SerializeField] private float rotationSpeed;

    [SerializeField] private float rotationSnapThreshold = 1.0f; // degrees

    [Header("Obstacle Avoidance Fields")]

    [SerializeField] private float whiskerLength;
    [SerializeField] private float whiskerAngle;
    [SerializeField] private float avoidanceWeight;

    [Header("Misc. Fields")]
    [SerializeField] private float attackRange;

    private Rigidbody2D rb;
    private DecisionTree dt;

    void Awake()
    {
        Debug.Log("Starting Smart Missile.");
        rb = GetComponent<Rigidbody2D>();

        m_target = GameObject.FindGameObjectWithTag("Player").transform;

        // Construct and build tree and nodes.
        dt = new DecisionTree(this.gameObject);
        BuildTree();
    }


    void Update()
    {
        // Set the CloseCombatCondition on the tree.
        dt.CloseCombatNode.IsWithinRange = Vector3.Distance(TargetPosition, transform.position) <= attackRange;
        
        // Make Decision and parse the state.
        dt.MakeDecision();

        switch (state)
        {
            case ActionState.MOVE_TO_PLAYER:
                SeekForward();
                AvoidObstacles();
                break;
            //case ActionState.ATTACK:
            //    // Explode.
            //    break;
            default: // Unhandled state.
                rb.linearVelocity = Vector3.zero;
                break;
        }
    }

    private void AvoidObstacles()
    {
        // Cast whiskers to detect obstacles
        bool hitLeft = CastWhisker(whiskerAngle, Color.red);
        bool hitRight = CastWhisker(-whiskerAngle, Color.blue);

        // Adjust rotation based on detected obstacles
        if (hitLeft)
        {
            // Rotate counterclockwise if the left whisker hit
            RotateClockwise();
        }
        else if (hitRight && !hitLeft)
        {
            // Rotate clockwise if the right whisker hit
            RotateCounterClockwise();
        }
    }

    private void RotateCounterClockwise()
    {
        // Rotate counterclockwise based on rotationSpeed and a weight.
        transform.Rotate(Vector3.forward, rotationSpeed * avoidanceWeight * Time.deltaTime);
    }

    private void RotateClockwise()
    {
        // Rotate clockwise based on rotationSpeed.
        transform.Rotate(Vector3.forward, -rotationSpeed * avoidanceWeight * Time.deltaTime);
    }

    private bool CastWhisker(float angle, Color color)
    {
        bool hitResult = false;
        Color rayColor = color;
        // Calculate the direction of the whisker.
        Vector2 whiskerDirection = Quaternion.Euler(0, 0, angle) * transform.up;

        // Cast a ray in the whisker direction.
        RaycastHit2D hit = Physics2D.Raycast(transform.position, whiskerDirection, whiskerLength);

        // Check if the ray hits an obstacle.
        if (hit.collider != null)
        {
            Debug.Log("Obstacle detected!");
            rayColor = Color.green;
            hitResult = true;
        }
        // Debug ray visualization
        Debug.DrawRay(transform.position, whiskerDirection * whiskerLength, rayColor);
        return hitResult;
    }

    private void SeekForward() // A seek with rotation to target but only moving along forward vector.
    {
        Vector2 toTarget = (TargetPosition - transform.position).normalized;
        Vector2 forward = transform.up;

        float angleDifference = Vector2.SignedAngle(forward, toTarget);

        Debug.Log("angleToTarget:" + angleDifference);

        if (Mathf.Abs(angleDifference) < rotationSnapThreshold)
        {
            transform.up = toTarget; // Snap rotation
        }
        else // Do need to rotate agent towards target.
        {
            float rotationStep = rotationSpeed * Time.deltaTime;
            float rotationAmount = Mathf.Clamp(angleDifference, -rotationStep, rotationStep);
            transform.Rotate(Vector3.forward, rotationAmount);
        }

        rb.linearVelocity = transform.up * movementSpeed;
    }

    private void BuildTree()
    {
        // CloseCombatCondition root node.
        dt.CloseCombatNode = new CloseCombatCondition();
        dt.treeNodeList.Add(dt.CloseCombatNode);

        // Second level.

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
