using BehaviourTree;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class GreetAnimal : Node
{
    /// <summary>
    /// Node to greet animals when spotted
    /// </summary>

    [Header("Animal Bits")]
    private NavMeshAgent _agent;
    private AiSensor _sensor;
    private GameObject _currentTarget;
    private AnimalKnowledge _animalKnowledge;
    private GameObject _textBubble;
    private TextMeshProUGUI _textBubbleText;
    
    [Header("Variables")]
    private float _greetDuration = 2.0f;
    private float _timer = 0;

    // Constructor
    public GreetAnimal(NavMeshAgent agent, AiSensor sensor, AnimalKnowledge animalKnowledge, GameObject textBubble)
    {
        _agent = agent;
        _sensor = sensor;
        _animalKnowledge = animalKnowledge;
        _textBubble = textBubble;
        _textBubbleText = textBubble.GetComponentInChildren<TextMeshProUGUI>();
    }

    public override NodeState Evaluate()
    {

        if (_currentTarget == null && (_sensor.Objects == null || _sensor.Objects.Count == 0))
        {
            _textBubble.SetActive(false);
            return NodeState.FAILURE;
        }

        // Pick the first animal seen
        _currentTarget = _sensor.Objects[0];
        _timer = _greetDuration;

        // Smile and wave boys... smile and wave
        _agent.isStopped = true;
        Debug.Log("Hi there, " + _currentTarget.name + "!");
        _textBubble.SetActive(true);
        _textBubbleText.text = "Hi there, " + _currentTarget.name + "!";

        // Look at the target to wave
        Vector3 direction = (_currentTarget.transform.position - _agent.transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        _agent.transform.rotation = Quaternion.Slerp(_agent.transform.rotation, lookRotation, Time.deltaTime * 5f);

        // Timer for greeting
        _timer -= Time.deltaTime;
        if (_timer <= 0)
        {
            _textBubble.SetActive(false);
            _agent.isStopped = false;

            _animalKnowledge.greetingCooldowns[_currentTarget] = Time.time + 30f;

            _currentTarget = null; // Reset for next greeting
            state = NodeState.SUCCESS;
            return state;
        }

        state = NodeState.RUNNING;
        return state;
    }

    public override void Reset()
    {
        _textBubble.SetActive(false);
        _currentTarget = null;
        _timer = 0;
        if (_agent != null) _agent.isStopped = false;
        state = NodeState.FAILURE;
    }
}