using UnityEngine;

// Tree class that contains the root node and handles what to do
namespace BehaviourTree
{
    public abstract class Tree : MonoBehaviour
    {
        private Node root = null;

        protected void Start()
        {
            root = SetupTree();
        }

        protected void Update()
        {
            if (root != null)
                root.Evaluate();
        }

        protected abstract Node SetupTree();
    }
}
