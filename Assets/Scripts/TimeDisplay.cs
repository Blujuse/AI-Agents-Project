using System;
using TMPro;
using UnityEngine;

public class TimeDisplay : MonoBehaviour
{
    [Header("Components")]
    private TextMeshProUGUI timeText;
    private LightingManager lightingManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Gets the text on the object
        timeText = gameObject.GetComponent<TextMeshProUGUI>();

        // Gets the lighting manager from its tag
        lightingManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<LightingManager>();
    }

    // Update is called once per frame
    void Update()
    {
        // Updates text based on time of day from lighting manager
        timeText.text = TimeSpan.FromHours(lightingManager.GetTimeOfDay()).ToString(@"hh\:mm"); // Formatted to hh:mm
    }
}