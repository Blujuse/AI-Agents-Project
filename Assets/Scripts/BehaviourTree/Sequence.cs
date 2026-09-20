using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

// Sequence node used to run child nodes in order
// Node fails on the first child failure
// Node succeeds if all children succeed
// Node runs if any child is running
namespace BehaviourTree
{
    public class Sequence : Node
    {
        public Sequence() : base() { }
        public Sequence(List<Node> children) : base(children) { }

        public override NodeState Evaluate()
        {
            bool anyChildRunning = false;

            foreach (Node node in children)
            {
                switch (node.Evaluate())
                {
                    case NodeState.FAILURE:
                        foreach (Node child in children) child.Reset();
                        state = NodeState.FAILURE; // Stop squence if any child node fails
                        return state;

                    case NodeState.SUCCESS: // Go to next child
                        continue;

                    case NodeState.RUNNING: // If any child is running, mark the sequence as running
                        anyChildRunning = true;
                        continue;

                    default:
                        state = NodeState.SUCCESS; // Edge case
                        return state;
                }
            }

            // If gets to this point, all children nodes have succeeded or are running
            state = anyChildRunning ? NodeState.RUNNING : NodeState.SUCCESS;
            return state;
        }
    }
}