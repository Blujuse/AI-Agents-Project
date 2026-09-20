using UnityEngine;

[ExecuteAlways] // Calls the script in edit mode in the unity editor
public class LightingManager : MonoBehaviour
{
    // Refernces
    [SerializeField] private Light directionalLight;
    [SerializeField] private LightingPreset preset;

    // Variables
    [SerializeField, Range(0, 24)] private float timeOfDay;
    [SerializeField] private float timeMultiplier = 1f;

    private void Update()
    {
        // Check if preset is assigned, no point in continuing if not
        if (preset == null)
            return;

        if (Input.GetKeyUp(KeyCode.Q))
        {
            timeMultiplier -= 0.01f;
        }
        else if (Input.GetKeyUp(KeyCode.E))
        {
            timeMultiplier += 0.01f;
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }


        // While playing game time will move
        if (Application.isPlaying)
        {
            timeOfDay += timeMultiplier * Time.deltaTime;
            timeOfDay %= 24; // Clamps
            UpdateLighting(timeOfDay / 24f);
        }
        else // Time will not move but lighting will update in editor
        {
            UpdateLighting(timeOfDay / 24f);
        }
    }

    // Function to update lighting based on time of day
    private void UpdateLighting(float timePercent)
    {
        // Uses the gradients in the preset to set the ambient and fog light colours
        RenderSettings.ambientLight = preset.ambientColour.Evaluate(timePercent);
        RenderSettings.fogColor = preset.fogColour.Evaluate(timePercent);

        // If there is a directional light assigned, set its colour and rotation
        if (directionalLight != null)
        {
            // Uses the gradient in the preset to set the directional light colour based on time of day
            directionalLight.color = preset.directionalColour.Evaluate(timePercent);

            // Rotates the light based on time of day, -90 to 270 degrees on the x axis
            directionalLight.transform.localRotation = Quaternion.Euler(new Vector3((timePercent * 360f) - 90f, 170f, 0));
        }
    }

    // Function to get the current time of day
    public float GetTimeOfDay()
    {
        return timeOfDay;
    }

    // Called when a value is changed in the inspector
    private void OnValidate()
    {
        // If no light is assigned, no point in continuing
        if (directionalLight != null)
            return;

        // Searches for a directional light in the scene
        if (RenderSettings.sun != null)
        {
            directionalLight = RenderSettings.sun;
        }
        else
        {
            // This section finds any directional lights in the scene
            Light[] lights = GameObject.FindObjectsByType<Light>(FindObjectsSortMode.None);

            foreach (Light light in lights)
            {
                if (light.type == LightType.Directional)
                    return;
            }
        }
    }
}
