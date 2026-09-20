using UnityEngine;

// Scriptable object to hold lighting preset data
[System.Serializable]
[CreateAssetMenu(fileName = "Lighting Preset", menuName = "Scriptables/Lighting Preset", order = 1)]

public class LightingPreset : ScriptableObject
{
    // Gradients for ambient, directional, and fog colours
    public Gradient ambientColour;
    public Gradient directionalColour;
    public Gradient fogColour;
}