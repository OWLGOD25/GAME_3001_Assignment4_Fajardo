using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class DecisionTree
{
    public GameObject Agent { get; set; }
    // All of our Condition nodes.
    public LOSCondition LOSNode { get; set; }
    public RadiusCondition RadiusNode { get; set; }
    public CloseCombatCondition CloseCombatNode { get; set; }
    // List of TreeNodes.
    public List<TreeNode> treeNodeList; // First node in List is root of tree.

    public DecisionTree(GameObject agent)
    {
        Agent = agent;
        treeNodeList = new List<TreeNode>();
    }

    public void MakeDecision() // Traversal of the tree.
    {
        TreeNode currentNode = treeNodeList[0];
        while (!currentNode.isLeaf) // currentNode always start as a ConditionNode.
        {
            currentNode = ((ConditionNode)currentNode).Condition() ? currentNode.right : currentNode.left;
        }
        ((ActionNode)currentNode).Action();
    }

    public TreeNode AddNode(TreeNode parent, TreeNode child, TreeNodeType type)
    {
        switch (type)
        {
            case TreeNodeType.LEFT_TREE_NODE:
                parent.left = child;
                break;
            case TreeNodeType.RIGHT_TREE_NODE:
                parent.right = child;
                break;
        }
        child.parent = parent;
        return child;
    }
}
