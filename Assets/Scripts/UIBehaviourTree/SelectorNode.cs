using UnityEngine;

public class SelectorNode : KiwiCompositeNode
{
    protected override void OnStart()
    {
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
        for (int i = 0; i < children.Count; i++)
        {
            var child = children[i];

            switch (child.Update())
            {
                case State.Running:
                    return State.Running;

                case State.Success:
                    return State.Success;

                case State.Failure:
                    continue;
            }
        }

        return State.Failure;
    }
}