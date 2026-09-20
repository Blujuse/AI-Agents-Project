using UnityEngine;

public class FlockEntity : MonoBehaviour
{
    public float speed = 1f;
    GameObject center;
    Vector3 centerPos;
    float rotSpeed = 3f;
    float neighbourDistance = 10f;
    bool turning = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        speed = Random.Range(0.5f, 2f);
        center = GameObject.Find("Floor Center");
        centerPos = center.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(transform.position, centerPos) >= GlobalFlock.airSize)
        {
            turning = true;
        }
        else if (transform.position.y <= 10f)
        {
            turning = true;
        }
        else
        {
            turning = false;
        }

        if (turning)
        {
            Vector3 direction = (centerPos + new Vector3(0, Random.Range(10f, GlobalFlock.airSize)) - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), rotSpeed * Time.deltaTime);
            speed = Random.Range(.5f, 3f);
        }

        if (Random.Range(0, 5) < 1) // 20% do steering forces on each entity
        {
            GameObject[] goBirds;
            goBirds = GlobalFlock.birds;

            Vector3 vCentre = this.transform.position;
            Vector3 vAvoid = centerPos;

            float gSpeed = 0.5f;
            Vector3 goalPos = GlobalFlock.goalPos;

            float dist;
            int groupSize = 0;

            foreach (GameObject go in goBirds)
            {
                if (go != this.gameObject)
                {
                    dist = Vector3.Distance(go.transform.position, this.transform.position);

                    if (dist <= neighbourDistance)
                    {
                        vCentre += go.transform.position;
                        groupSize++;
                        if (dist < 6f)
                        {
                            vAvoid = vAvoid + (this.transform.position - go.transform.position);
                        }

                        FlockEntity otherFlockEntity = go.GetComponent<FlockEntity>();
                        gSpeed = gSpeed + otherFlockEntity.speed;
                    }
                }
            }

            if (groupSize > 0)
            {
                vCentre = vCentre / groupSize + (goalPos - this.transform.position);
                speed = (gSpeed / groupSize) + Random.Range(-0.1f, 0.1f);
                if (speed > 5f)
                {
                    speed = Random.Range(0.5f, 3f);
                }
                Vector3 direction = (vCentre - vAvoid) - transform.position;
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), rotSpeed * Time.deltaTime);
            }
        }
        transform.Translate(0, 0, Time.deltaTime * speed);
    }
}
