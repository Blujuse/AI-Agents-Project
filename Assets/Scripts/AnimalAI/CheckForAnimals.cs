using BehaviourTree;
using UnityEngine;

public class CheckForAnimals : Node
{
    private AiSensor _sensor;
    private AnimalKnowledge _animalKnowledge;

    public CheckForAnimals(AiSensor sensor, AnimalKnowledge animalKnowledge)
    {
        _sensor = sensor;
        _animalKnowledge = animalKnowledge;
    }

    public override NodeState Evaluate()
    {
        if (_sensor.Objects.Count > 0)
        {
            GameObject target = _sensor.Objects[0];

            // Check if we have greeted this animal recently
            if (_animalKnowledge.greetingCooldowns.ContainsKey(target))
            {
                if (Time.time < _animalKnowledge.greetingCooldowns[target])
                    return NodeState.FAILURE; // Already said hi! Carry on with the schedule.
            }

            state = NodeState.SUCCESS;
            return state;
        }
        return NodeState.FAILURE;
    }
}