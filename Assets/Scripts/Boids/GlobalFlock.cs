using UnityEngine;

public class GlobalFlock : MonoBehaviour
{
    public GameObject birdPrefab;
    public static int airSize = 50;
    static int numBirds = 200;
    public static GameObject[] birds = new GameObject[numBirds];
    public static Vector3 goalPos = new Vector3(0, 0, 0);
    GameObject center;
    Vector3 centerPos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        center = GameObject.Find("Floor Center");
        centerPos = center.transform.position;
        goalPos = new Vector3(Random.Range(centerPos.x - airSize, centerPos.x + airSize), 
                              Random.Range(centerPos.y + 10f, centerPos.y + airSize), 
                              Random.Range(centerPos.z - airSize, centerPos.z + airSize));

        for (int i = 0; i < numBirds; i++)
        {
            Vector3 pos = new Vector3(Random.Range(centerPos.x - airSize, centerPos.x + airSize),
                                      Random.Range(centerPos.y + 10f, centerPos.y + airSize),
                                      Random.Range(centerPos.z - airSize, centerPos.z + airSize));
            birds[i]=(GameObject)Instantiate(birdPrefab, pos, Quaternion.LookRotation(new Vector3(Random.Range(-1,1), 0, Random.Range(-1,1))));
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Random.Range(0, 100) < 1)
        {
            goalPos = new Vector3(Random.Range(centerPos.x - airSize, centerPos.x + airSize),
                                  Random.Range(centerPos.y + 10f, centerPos.y + airSize),
                                  Random.Range(centerPos.z - airSize, centerPos.z + airSize));
        }
    }
}