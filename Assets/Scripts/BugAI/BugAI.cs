using UnityEngine;

public class BugAI : MonoBehaviour
{
    public KiwiBehaviourTree tree;
    public WalkingGrid walkingGrid;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        tree = tree.Clone();
        tree.Bind(walkingGrid, gameObject, GetComponent<SphereCollider>());
    }

    // Update is called once per frame
    void Update()
    {
        tree.Update();
    }
}