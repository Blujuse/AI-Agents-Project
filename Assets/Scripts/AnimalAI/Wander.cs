using UnityEngine;
using UnityEngine.AI;

using BehaviourTree;

// Wander node makes the agent walk to random points within a defined walking area
public class Wander : Node
{
    [Header("Animal References")]
    private NavMeshAgent _agent;
    private WalkingArea _walkingArea;

    // Contructor 
    public Wander(NavMeshAgent agent, WalkingArea walkingArea)
    {
        _agent = agent;
        _walkingArea = walkingArea;

        SetRandomDest(); // Set initial random destination
    }

    bool HasArrived() //Check arrival at destination
    {
        return _agent.remainingDistance <= _agent.stoppingDistance;
    }

    void SetRandomDest() // Set a new random destination within the walking area
    {
        _agent.SetDestination(_walkingArea.GetRandomPoint());
    }

    public override NodeState Evaluate()
    {
        // If arrived at destination, set a new random destination
        if (HasArrived())
        {
            SetRandomDest();
        }

        state = NodeState.RUNNING; // Set state to running
        return state; // Return current state
    }
}