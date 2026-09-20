using BehaviourTree;
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class AnimalBT : BehaviourTree.Tree
{
    [Header("Animal AI Components")]
    [HideInInspector] public NavMeshAgent agent;
    public WalkingArea walkingArea;
    public GameObject textBubble;
    private AnimalKnowledge animalKnowledge;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animalKnowledge = GetComponent<AnimalKnowledge>();
    }

    protected override Node SetupTree()
    {
        // Order is key here fallback action is follwing the schedule, so it is the last node

        AiSensor sensor = GetComponent<AiSensor>();

        Node root = new Selector(new List<Node>
        {
            new Sequence(new List<Node>
            {
                new CheckForAnimals(sensor, animalKnowledge), // Conditional node for checking animal is there
                new GreetAnimal(agent, sensor, animalKnowledge, textBubble) // Action to say hello
            }),

            new FollowShedule(agent, walkingArea, animalKnowledge), // Default action is following the shedule
        });
                      
        return root;
    }
}