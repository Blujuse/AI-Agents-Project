using UnityEngine;
using UnityEngine.AI;

public class WalkingArea : MonoBehaviour
{
    public float radius = 5f;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    public Vector3 GetRandomPoint()
    {
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection.y = 0f; // Keep the point on the same horizontal plane

        Vector3 randomPoint = transform.position + randomDirection;

        NavMeshHit hit;
        Vector3 finalPosition = transform.position;

        if (NavMesh.SamplePosition(randomPoint, out hit, 2f, 1))
        {
            finalPosition = hit.position;
        }

        return finalPosition;
    }
}