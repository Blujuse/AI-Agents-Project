using System.Collections.Generic;
using UnityEngine;

// This script holds all the knowledge an individual animal needs to make decisions
public class AnimalKnowledge : MonoBehaviour
{
    [Header("Animal Knowledge Needed")]
    public GlobalAnimalData animalData;
    public LightingManager lightingManager;
    [SerializeField] private float timeOfDay;
    public ActivityArea currentActivityArea;

    [Header("Animal Settings")]
    public string animalName;
    public ActivityArea homeArea;
    public float[] activityTimes;

    public Dictionary<GameObject, float> greetingCooldowns = new Dictionary<GameObject, float>();

    private void Update()
    {
        timeOfDay = lightingManager.GetTimeOfDay();
    }

    public float GetTimeOfDay()
    {
        return timeOfDay;
    }
}