using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UIElements;

[ExecuteInEditMode]
public class AiSensor : MonoBehaviour
{
    [Header("Detection Settings")]
    public float distance = 10;
    public float angle = 30;
    public float height = 1.0f;
    public float originOffset = 0.5f;

    [Header("Filtering")]
    public Color meshColour = Color.red;
    public int scanFrequency = 30;
    public LayerMask layers;
    public LayerMask occulusionLayers;
    public List<GameObject> Objects = new List<GameObject>();

    private int animalLayer;

    Collider[] colliders = new Collider[50];
    Mesh mesh;
    int count;
    float scanInteval;
    float scanTimer;

    private void Start()
    {
        scanInteval = 1.0f / scanFrequency;
        animalLayer = LayerMask.NameToLayer("Animals");
    }

    private void Update()
    {
        scanTimer -= Time.deltaTime;
        if (scanTimer < 0)
        {
            scanTimer += scanInteval;
            Scan();
        }
    }

    private void Scan()
    {
        Vector3 sensorOrigin = transform.position + (transform.forward * originOffset);

        count = Physics.OverlapSphereNonAlloc(transform.position, distance, colliders, layers, QueryTriggerInteraction.Collide);

        Objects.Clear();
        for (int i = 0; i < count; ++i)
        {
            GameObject obj = colliders[i].gameObject;

            if (IsInSight(obj))
            {
                Objects.Add(obj);

                if (obj.layer == LayerMask.NameToLayer("Animals"))
                {
                    Debug.Log($"I see an animal: {obj.name}!");
                }
            }
        }
    }

    public bool IsInSight(GameObject obj)
    {
        Vector3 origin = transform.position + (transform.forward * originOffset);
        Vector3 dest = obj.transform.position;
        Vector3 direction = dest - origin;

        if (direction.y < 0 || direction.y > height)
        {
            return false;
        }

        direction.y = 0;
        float deltaAngle = Vector3.Angle(direction, transform.forward);
        if (deltaAngle > angle)
        {
            return false;
        }

        origin.y += height / 2;
        dest.y = origin.y;
        if (Physics.Linecast(origin, dest, occulusionLayers))
        {
            return false;
        }

        return true;
    }

    public int Filter(GameObject[] buffer, string layerName)
    {
        int layer = LayerMask.NameToLayer(layerName);
        int count = 0;

        foreach (var obj in Objects)
        {
            if (obj.layer == layer)
            {
                buffer[count++] = obj;
            }

            if (buffer.Length > 0)
            {
                break;
            }
        }

        return count;
    }

    Mesh CreateWedgeMesh()
    {
        Mesh mesh = new Mesh();

        int segments = 10;
        int numTriangles = (segments * 4) + 2 + 2;
        int numVertices = numTriangles * 3;

        Vector3[] vertices = new Vector3[numVertices];
        int[] triangles = new int[numVertices];

        Vector3 bottomCenter = Vector3.zero;
        Vector3 bottomLeft = Quaternion.Euler(0, -angle, 0) * Vector3.forward * distance;
        Vector3 bottomRight = Quaternion.Euler(0, angle, 0) * Vector3.forward * distance;

        Vector3 topCenter = bottomCenter + Vector3.up * height;
        Vector3 topRight = bottomRight + Vector3.up * height;
        Vector3 topLeft = bottomLeft + Vector3.up * height;

        int vert = 0;

        // L side
        vertices[vert++] = bottomCenter;
        vertices[vert++] = bottomLeft;
        vertices[vert++] = topLeft;

        vertices[vert++] = topLeft;
        vertices[vert++] = topCenter;
        vertices[vert++] = bottomCenter;

        // R side
        vertices[vert++] = bottomCenter;
        vertices[vert++] = topCenter;
        vertices[vert++] = topRight;

        vertices[vert++] = topRight;
        vertices[vert++] = bottomRight;
        vertices[vert++] = bottomCenter;

        float currentAngle = -angle;
        float deltaAngle = (angle * 2) / segments;
        for (int i =0; i < segments; ++i)
        {
            bottomLeft = Quaternion.Euler(0, currentAngle, 0) * Vector3.forward * distance;
            bottomRight = Quaternion.Euler(0, currentAngle + deltaAngle, 0) * Vector3.forward * distance;

            topRight = bottomRight + Vector3.up * height;
            topLeft = bottomLeft + Vector3.up * height;

            // FAHH side
            vertices[vert++] = bottomLeft;
            vertices[vert++] = bottomRight;
            vertices[vert++] = topRight;

            vertices[vert++] = topRight;
            vertices[vert++] = topLeft;
            vertices[vert++] = bottomLeft;

            // Top side
            vertices[vert++] = topCenter;
            vertices[vert++] = topLeft;
            vertices[vert++] = topRight;

            // Bottom side
            vertices[vert++] = bottomCenter;
            vertices[vert++] = bottomRight;
            vertices[vert++] = bottomLeft;

            currentAngle += deltaAngle;
        }

        for (int i = 0; i < numVertices; i++)
        {
            triangles[i] = i;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        return mesh;
    }

    private void OnValidate()
    {
        mesh = CreateWedgeMesh();
        scanInteval = 1.0f / scanFrequency;
    }

    private void OnDrawGizmos()
    {
        if (mesh)
        {
            Gizmos.color = meshColour;
            Vector3 drawPos = transform.position + (transform.forward * originOffset);
            Gizmos.DrawMesh(mesh, drawPos, transform.rotation);
        }

        Gizmos.DrawWireSphere(transform.position, distance);
        for (int i = 0; i < count; ++i)
        {
            Gizmos.DrawSphere(colliders[i].transform.position, 0.2f);
        }

        Gizmos.color = Color.green;
        foreach (var obj in Objects)
        {
            Gizmos.DrawSphere(obj.transform.position, 0.2f);
        }
    }
}