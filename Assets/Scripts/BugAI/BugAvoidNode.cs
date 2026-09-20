using UnityEngine;

public class BugAvoidNode : KiwiActionNode
{
    float waitTime;

    public GameObject agent;

    protected override void OnStart()
    {
        waitTime = Time.time + 1.0f;

        agent = blackboard.agentObj;
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
        if (Time.time < waitTime)
        {
            Destroy(agent);

            return State.Running;
        }
        return State.Success;
    }
}