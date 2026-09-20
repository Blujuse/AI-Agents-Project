using UnityEngine;

public class ConditionalNode : KiwiDecoratorNode
{
    public bool conditionMet = false;

    protected override void OnStart()
    {
        
    }

    protected override void OnStop()
    {
        
    }

    protected override State OnUpdate()
    {
        if (conditionMet)
        {
            return child.Update();
        }

        return State.Failure;
    }
}