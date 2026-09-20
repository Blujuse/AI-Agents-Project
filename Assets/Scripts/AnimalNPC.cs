using UnityEngine;
using UnityEngine.AI;

public class AnimalNPC : MonoBehaviour
{
    [HideInInspector]
    public NavMeshAgent agent;

    public float currentSpeed
    {
        get { return agent.velocity.magnitude; }
    }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }
}