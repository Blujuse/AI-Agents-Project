using UnityEngine;
using UnityEngine.AI;

public class NPCWander : NPCComponent
{
    [SerializeField] public WalkingArea walkingArea;

    private void Start()
    {
        SetRandomDest();
    }

    private void Update()
    {
        if (HasArrived())
        {
            SetRandomDest();
        }
    }

    bool HasArrived()
    {
        return animalNPC.agent.remainingDistance <= animalNPC.agent.stoppingDistance;
    }

    void SetRandomDest()
    {
        animalNPC.agent.SetDestination(walkingArea.GetRandomPoint());
    }
}