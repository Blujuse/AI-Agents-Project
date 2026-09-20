using UnityEngine;

public class KiwiBehaviourTreeRunner : MonoBehaviour
{
    public KiwiBehaviourTree tree;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        tree = tree.Clone();
        //tree.Bind(/* AI Agent */);
    }

    // Update is called once per frame
    void Update()
    {
        tree.Update();
    }
}