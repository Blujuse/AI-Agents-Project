using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

// Node base class for behaviour tree
namespace BehaviourTree
{
    // Enum contains possible states for a node
    public enum NodeState
    {
        RUNNING,
        SUCCESS,
        FAILURE
    }

    public class Node
    {
        protected NodeState state; // State tracking

        public Node parent;
        protected List<Node> children = new List<Node>(); // List of child nodes

        private Dictionary<string, object> dataContext = new Dictionary<string, object>(); // Dictionary for storing node data

        public Node()
        {
            parent = null;
        }

        public Node(List<Node> children)
        {
            foreach (Node child in children)
            {
                Attach(child);
            }
        }

        // Helper to attach child nodes
        private void Attach(Node node)
        {
            node.parent = this;
            children.Add(node);
        }

        public virtual NodeState Evaluate() => NodeState.FAILURE; // Return failure by default

        // Helper to set node data
        public void SetData(string key, object value)
        {
            dataContext[key] = value;
        }

        // Helper to get node data
        // Searches current node first, then traverses up the tree to find the data
        public object GetData(string key) 
        {
            object value = null;
            if (dataContext.TryGetValue(key, out value)) // Check current node first
                return value;

            Node node = parent; // Traverse up the tree
            while (node != null)
            {
                value = node.GetData(key); // Recursive call to get data from parent
                if (value != null) 
                    return value; // Return if found
                node = node.parent; // Move up the tree
            }
            return null;
        }

        // Helper to clear node data
        public bool ClearData(string key)
        {
            if (dataContext.ContainsKey(key)) // Check current node first
            {
                dataContext.Remove(key); // Remove the data
                return true;
            }

            Node node = parent;

            while (node != null)
            {
                bool cleared = node.ClearData(key); // Recursive call to clear data from parent
                if (cleared)
                    return true; // Return if cleared
                node = node.parent; // Move up the tree
            }
            return false;
        }

        public virtual void Reset()
        {

        }
    }
}