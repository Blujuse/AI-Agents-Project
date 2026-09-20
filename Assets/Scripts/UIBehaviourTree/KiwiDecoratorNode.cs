using UnityEngine;

public abstract class KiwiDecoratorNode : KiwiNode
{
    [HideInInspector] public KiwiNode child;

    public override KiwiNode Clone()
    {
        KiwiDecoratorNode node = Instantiate(this);
        node.child = child.Clone();
        return node;
    }
}