using System;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor;

public class KiwiNodeView : UnityEditor.Experimental.GraphView.Node
{
    public Action<KiwiNodeView> OnNodeSelected;
    public KiwiNode node;
    public Port input, output;

    public KiwiNodeView(KiwiNode node) : base("Assets/Scripts/UIBehaviourTree/UI/KiwiNodeView.uxml")
    {
        this.node = node;
        this.title = node.name;
        this.viewDataKey = node.guid;

        style.left = node.position.x;
        style.top = node.position.y;

        CreateInputPorts();
        CreateOutputPorts();
        SetupClasses();

        Label descriptionLabel = this.Q<Label>("description");
        descriptionLabel.bindingPath = "description";
        descriptionLabel.Bind(new SerializedObject(node));
    }

    private void SetupClasses()
    {
        if (node is KiwiActionNode)
        {
            AddToClassList("action");
        }
        else if (node is KiwiCompositeNode)
        {
            AddToClassList("composite");
        }
        else if (node is KiwiDecoratorNode)
        {
            AddToClassList("decorator");
        }
        else if (node is KiwiRootNode)
        {
            AddToClassList("root");
        }
    }

    private void CreateInputPorts()
    {
        if (node is KiwiActionNode)
        {
            input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
        }
        else if (node is KiwiCompositeNode)
        {
            input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
        }
        else if (node is KiwiDecoratorNode)
        {
            input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
        }
        else if (node is KiwiRootNode)
        {

        }

        if (input != null)
        {
            input.portName = "";
            input.style.flexDirection = FlexDirection.Column;
            inputContainer.Add(input);
        }
    }

    private void CreateOutputPorts()
    {
        if (node is KiwiActionNode)
        {

        }
        else if (node is KiwiCompositeNode)
        {
            output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Multi, typeof(bool));
        }
        else if (node is KiwiDecoratorNode)
        {
            output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Single, typeof(bool));
        }
        else if (node is KiwiRootNode)
        {
            output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Single, typeof(bool));
        }

        if (output != null)
        {
            output.portName = "";
            output.style.flexDirection = FlexDirection.ColumnReverse;
            outputContainer.Add(output);
        }
    }

    public override void SetPosition(Rect newPos)
    {
        base.SetPosition(newPos);
        Undo.RecordObject(node, "Behaviour Tree (Set Position");

        node.position.x = newPos.x;
        node.position.y = newPos.y;
        EditorUtility.SetDirty(node);
    }

    public override void OnSelected()
    {
        base.OnSelected();
        if (OnNodeSelected != null)
        {
            OnNodeSelected.Invoke(this);
        }
    }

    public void SortChildren()
    {
        KiwiCompositeNode composite = node as KiwiCompositeNode;

        if (composite)
        {
            composite.children.Sort(SortByHorizontalPosition);
        }
    }

    private int SortByHorizontalPosition(KiwiNode left, KiwiNode right)
    {
        return left.position.x < right.position.x ? -1: 1;
    }

    public void UpdateState()
    {
        RemoveFromClassList("running");
        RemoveFromClassList("failure");
        RemoveFromClassList("success");

        if (Application.isPlaying)
        {
            switch (node.state)
            {
                case KiwiNode.State.Running:
                    if (node.started)
                    {
                        AddToClassList("running");
                    }
                    break;
                case KiwiNode.State.Failure:
                    AddToClassList("failure");
                    break;
                case KiwiNode.State.Success:
                    AddToClassList("success");
                    break;
            }
        }
    }
}