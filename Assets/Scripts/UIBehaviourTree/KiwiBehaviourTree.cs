using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[CreateAssetMenu()]
public class KiwiBehaviourTree : ScriptableObject
{
    public KiwiNode rootNode;
    public KiwiNode.State treeState = KiwiNode.State.Running;
    public List<KiwiNode> nodes = new List<KiwiNode>();
    [SerializeField] public KiwiBlackboard blackboard = new KiwiBlackboard();

    public KiwiNode.State Update()
    {
        if (rootNode.state == KiwiNode.State.Running)
        {
            treeState = rootNode.Update();
        }

        return treeState;
    }

    public KiwiNode CreateNode (System.Type type)
    {
        KiwiNode node = ScriptableObject.CreateInstance(type) as KiwiNode;
        node.name = type.Name;
#if UNITY_EDITOR
        node.guid = GUID.Generate().ToString();

        Undo.RecordObject(this, "Behaviour Tree (Create Node)");
#endif
        nodes.Add(node);

#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            AssetDatabase.AddObjectToAsset(node, this);
        }

        Undo.RegisterCreatedObjectUndo(node, "Behaviour Tree (Create Node)");

        AssetDatabase.SaveAssets();
#endif
        return node;
    }

    public void DeleteNode (KiwiNode node)
    {
#if UNITY_EDITOR
        Undo.RecordObject(this, "Behaviour Tree (Create Node)");
#endif
        nodes.Remove(node);

#if UNITY_EDITOR
        Undo.DestroyObjectImmediate(node);
        AssetDatabase.SaveAssets();
#endif
    }

    public void AddChild(KiwiNode parent, KiwiNode child)
    {
        KiwiDecoratorNode decorator = parent as KiwiDecoratorNode;
        if (decorator)
        {
#if UNITY_EDITOR
            Undo.RecordObject(decorator, "Behaviour Tree (Add Child)");
#endif
            decorator.child = child;
#if UNITY_EDITOR
            EditorUtility.SetDirty(decorator);
#endif
        }

        KiwiRootNode rootNode = parent as KiwiRootNode;
        if (rootNode)
        {
#if UNITY_EDITOR
            Undo.RecordObject(rootNode, "Behaviour Tree (Add Child)");
#endif
            rootNode.child = child;
#if UNITY_EDITOR
            EditorUtility.SetDirty(rootNode);
#endif
        }

        KiwiCompositeNode composite = parent as KiwiCompositeNode;
        if (composite)
        {
#if UNITY_EDITOR
            Undo.RecordObject(composite, "Behaviour Tree (Add Child)");
#endif
            composite.children.Add(child);
#if UNITY_EDITOR
            EditorUtility.SetDirty(composite);
#endif
        }
    }

    public void RemoveChild(KiwiNode parent, KiwiNode child)
    {
        KiwiDecoratorNode decorator = parent as KiwiDecoratorNode;
        if (decorator)
        {
#if UNITY_EDITOR
            Undo.RecordObject(decorator, "Behaviour Tree (Add Remove)");
#endif
            decorator.child = null;
#if UNITY_EDITOR
            EditorUtility.SetDirty(decorator);
#endif
        }

        KiwiRootNode rootNode = parent as KiwiRootNode;
        if (rootNode)
        {
#if UNITY_EDITOR
            Undo.RecordObject(rootNode, "Behaviour Tree (Add Remove)");
#endif
            rootNode.child = null;
#if UNITY_EDITOR
            EditorUtility.SetDirty(rootNode);
#endif
        }

        KiwiCompositeNode composite = parent as KiwiCompositeNode;
        if (composite)
        {
#if UNITY_EDITOR
            Undo.RecordObject(composite, "Behaviour Tree (Add Remove)");
#endif
            composite.children.Remove(child);
#if UNITY_EDITOR
            EditorUtility.SetDirty(composite);
#endif
        }
    }

    public List<KiwiNode> GetChildren(KiwiNode parent)
    {
        List<KiwiNode> children = new List<KiwiNode>();

        KiwiDecoratorNode decorator = parent as KiwiDecoratorNode;
        if (decorator && decorator.child != null)
        {
            children.Add(decorator.child);
        }

        KiwiRootNode rootNode = parent as KiwiRootNode;
        if (rootNode && rootNode.child != null)
        {
            children.Add(rootNode.child);
        }

        KiwiCompositeNode composite = parent as KiwiCompositeNode;
        if (composite)
        {
            return composite.children;
        }

        return children;
    }

    public void Traverse(KiwiNode node, System.Action<KiwiNode> visiter)
    {
        if (node)
        {
            visiter.Invoke(node);
            var children = GetChildren(node);
            children.ForEach((n) => Traverse(n, visiter));
        }    
    }

    public KiwiBehaviourTree Clone()
    {
        KiwiBehaviourTree tree = Instantiate(this);
        tree.rootNode = this.rootNode.Clone();
        tree.nodes = new List<KiwiNode>();
        Traverse(tree.rootNode, (n) =>
        {
            tree.nodes.Add(n);
        });

        return tree;
    }

    public void Bind(WalkingGrid _walkingGrid, GameObject _agentObj, SphereCollider _senseZone)
    {
        Traverse(rootNode, node =>
        {
            //node.agent = agent;
            blackboard.walkingGrid = _walkingGrid;
            blackboard.agentObj = _agentObj;
            blackboard.senseZone = _senseZone;
            node.blackboard = blackboard;
        });
    }
}