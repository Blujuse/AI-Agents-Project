using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

// Similar to a sequence node but it returns success if any child returns success
// Only fails if all children fail
// Returns running if any child is running
namespace BehaviourTree
{
    public class Selector : Node
    {
        public Selector() : base() { }
        public Selector(List<Node> children) : base(children) { }

        public override NodeState Evaluate()
        {
            foreach (Node node in children)
            {
                switch (node.Evaluate())
                {
                    case NodeState.FAILURE:
                        continue; // If fails try next node

                    case NodeState.SUCCESS:
                        state = NodeState.SUCCESS; // If any node succeeds, selector succeeds
                        return state;

                    case NodeState.RUNNING:
                        state = NodeState.RUNNING; // If any node is running, selector is running
                        return state;

                    default:
                        continue;
                }
            }

            // If all nodes fail, selector fails
            state = NodeState.FAILURE;
            return state;
        }
    }
}