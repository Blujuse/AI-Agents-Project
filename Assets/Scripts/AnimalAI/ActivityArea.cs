using UnityEngine;

// This script is simply used to get the bounds of a collider and use it as an activity area for animals
public class ActivityArea : MonoBehaviour
{
    public string activityName;
    public Bounds areaBounds;

    private void Start()
    {
        Collider collider = GetComponent<Collider>();
        if (collider != null)
        {
            areaBounds = collider.bounds;
        }
    }
}