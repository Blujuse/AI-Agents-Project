using UnityEngine;

public class NPCComponent : MonoBehaviour
{
    protected AnimalNPC animalNPC;

    protected virtual void Awake()
    {
        animalNPC = GetComponentInParent<AnimalNPC>();
    }
}