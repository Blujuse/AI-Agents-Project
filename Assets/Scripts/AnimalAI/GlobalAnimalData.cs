using UnityEngine;

// Holds global data for animal activities and locations
// Sorta works like a hivemind as animals have lived on island for a long time
// So would make sense they know activity spots around the island
public class GlobalAnimalData : MonoBehaviour
{
    public ActivityArea[] bugCatchingSpots;
    public ActivityArea[] picnicSpots;
    public ActivityArea[] shopSpots;
    public ActivityArea[] beachSpots;
}