using UnityEngine;

public class NearbyAnimalNode : KiwiDecoratorNode
{
    [Header("Detection Settings")]
    public LayerMask animalLayer;

    public SphereCollider senseZone;
    public Collider[] results = new Collider[1];

    private bool isInit = false;

    protected override void OnStart()
    {
        if (!isInit)
        {
            senseZone = blackboard.senseZone;
            isInit = true;
        }
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
        if (senseZone == null) return State.Failure;

        Vector3 center = senseZone.transform.TransformPoint(senseZone.center);
        float radius = senseZone.radius * Mathf.Max(senseZone.transform.lossyScale.x,
                                                    senseZone.transform.lossyScale.y,
                                                    senseZone.transform.lossyScale.z);

        int count = Physics.OverlapSphereNonAlloc(center, radius, results, animalLayer);

        if (count > 0)
        {
            return child.Update();
        }

        return State.Failure;
    }
}