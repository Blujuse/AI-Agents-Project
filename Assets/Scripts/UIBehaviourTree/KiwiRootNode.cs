using UnityEngine;

public class KiwiRootNode : KiwiNode
{
    public KiwiNode child;

    protected override void OnStart()
    {

    }

    protected override void OnStop()
    {

    }

    protected override State OnUpdate()
    {
        return child.Update();
    }

    public override KiwiNode Clone()
    {
        KiwiRootNode node = Instantiate(this);
        node.child = child.Clone();
        return node;
    }
}