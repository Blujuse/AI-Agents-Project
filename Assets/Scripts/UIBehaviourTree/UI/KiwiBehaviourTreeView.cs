using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using static UnityEditor.Experimental.GraphView.GraphView;

public class KiwiBehaviourTreeView : GraphView
{
    public Action<KiwiNodeView> OnNodeSelected;
    public new class UxmlFactory : UxmlFactory<KiwiBehaviourTreeView, GraphView.UxmlTraits> { }
    KiwiBehaviourTree tree;

    public KiwiBehaviourTreeView() 
    {
        Insert(0, new GridBackground());

        this.AddManipulator(new ContentZoomer());
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Scripts/UIBehaviourTree/UI/KiwiBehaviourTreeEditor.uss");
        styleSheets.Add(styleSheet);

        Undo.undoRedoPerformed += OnUndoRedo;
    }

    private void OnUndoRedo()
    {
        PopulateView(tree);
        AssetDatabase.SaveAssets();
    }

    KiwiNodeView FindNodeView(KiwiNode node)
    {
        return GetNodeByGuid(node.guid) as KiwiNodeView;
    }

    internal void PopulateView(KiwiBehaviourTree tree)
    {
        this.tree = tree;

        graphViewChanged -= OnGraphViewChanged;
        DeleteElements(graphElements);
        graphViewChanged += OnGraphViewChanged;

        if (tree.rootNode == null)
        {
            tree.rootNode = tree.CreateNode(typeof(KiwiRootNode)) as KiwiRootNode;
            EditorUtility.SetDirty(tree);
            AssetDatabase.SaveAssets();
        }

        // Creates node view
        tree.nodes.ForEach(n => CreateNodeView(n));
        
        // Create edges
        tree.nodes.ForEach(n => {
            var children = tree.GetChildren(n);
            children.ForEach(c =>
            {
                KiwiNodeView parentView = FindNodeView(n);
                KiwiNodeView childView = FindNodeView(c);

                Edge edge = parentView.output.ConnectTo(childView.input);
                AddElement(edge);
            });
        });
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        return ports.ToList().Where(endPort => endPort.direction != startPort.direction && endPort.node != startPort.node).ToList();
    }

    private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
    {
        if (graphViewChange.elementsToRemove != null)
        {
            graphViewChange.elementsToRemove.ForEach(elem =>
            {
                KiwiNodeView nodeView = elem as KiwiNodeView;

                if (nodeView != null)
                {
                    tree.DeleteNode(nodeView.node);
                }

                Edge edge = elem as Edge;
                if (edge != null)
                {
                    KiwiNodeView parentView = edge.output.node as KiwiNodeView;
                    KiwiNodeView childView = edge.input.node as KiwiNodeView;
                    tree.RemoveChild(parentView.node, childView.node);
                }
            });
        }

        if (graphViewChange.edgesToCreate != null)
        {
            graphViewChange.edgesToCreate.ForEach(edge =>
            {
                KiwiNodeView parentView = edge.output.node as KiwiNodeView;
                KiwiNodeView childView = edge.input.node as KiwiNodeView;
                tree.AddChild(parentView.node, childView.node);
            });
        }

        if (graphViewChange.movedElements != null)
        {
            nodes.ForEach((n) =>
            {
                KiwiNodeView view = n as KiwiNodeView;
                view.SortChildren();
            });
        }

        return graphViewChange;
    }

    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
    {
        //base.BuildContextualMenu(evt);
        {
            var types = TypeCache.GetTypesDerivedFrom<KiwiActionNode>();
            foreach (var type in types)
            {
                evt.menu.AppendAction($"[{type.BaseType.Name}] {type.Name}", (a) => CreateNode(type));
            }
        }

        {
            var types = TypeCache.GetTypesDerivedFrom<KiwiCompositeNode>();
            foreach (var type in types)
            {
                evt.menu.AppendAction($"[{type.BaseType.Name}] {type.Name}", (a) => CreateNode(type));
            }
        }

        {
            var types = TypeCache.GetTypesDerivedFrom<KiwiDecoratorNode>();
            foreach (var type in types)
            {
                evt.menu.AppendAction($"[{type.BaseType.Name}] {type.Name}", (a) => CreateNode(type));
            }
        }
    }

    void CreateNode(System.Type type)
    {
        KiwiNode node = tree.CreateNode(type);
        CreateNodeView(node);
    }

    void CreateNodeView(KiwiNode node)
    {
        KiwiNodeView nodeView = new KiwiNodeView(node);
        nodeView.OnNodeSelected = OnNodeSelected;
        AddElement(nodeView);
    }

    public void UpdateNodeStates()
    {
        nodes.ForEach(n =>
        {
            KiwiNodeView view = n as KiwiNodeView;
            view.UpdateState();
        });
    }
}