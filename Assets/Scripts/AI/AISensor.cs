using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class to implement line-of-sight follows this tutorial
// https://www.youtube.com/watch?v=znZXmmyBF-o
[ExecuteAlways]
public class AISensor : MonoBehaviour
{
    public float distance = 10.0f;
    public float angle = 30.0f; // Degrees
    public float lowerBound = 0.0f;
    public float height = 1.0f;
    public Color meshColor = Color.red;
    public int scanFrequency = 30; // number of scans per second
    public LayerMask layers;
    public LayerMask occlusionLayers;
    public List<GameObject> objects = new List<GameObject>(); // Objects in sight

    // Buffer for colliders
    Collider[] colliders = new Collider[50];
    int count;
    float scanInterval, scanTimer;
    Mesh mesh;

    // Start is called before the first frame update
    void Start()
    {
        scanInterval = 1.0f / scanFrequency;
    }

    // Update is called once per frame
    void Update()
    {
        scanTimer -= Time.deltaTime;
        if(scanTimer < 0) {
            scanTimer += scanInterval;
            Scan();
        }
    }
    private void Scan() {
        count = Physics.OverlapSphereNonAlloc(transform.position, distance, colliders, layers, QueryTriggerInteraction.Collide);

        objects.Clear();
        for (int i = 0; i < count; i++) {
            GameObject obj = colliders[i].gameObject;
            if (IsInSight(obj)) {
                objects.Add(obj);
            }
        }
    }

    public bool IsInSight(GameObject obj) {
        Vector3 origin = transform.position;
        Vector3 destination = obj.transform.position;
        Vector3 direction = destination - origin;

        // Check if the object is within height bounds
        if(direction.y < lowerBound || direction.y > height) return false;

        // only check for horizontal direction
        direction.y = 0;
        float deltaAngle = Vector3.Angle(direction, transform.forward);
        if(deltaAngle > angle) return false;

        origin.y += (lowerBound + height) / 2;
        destination.y = origin.y;
        if (Physics.Linecast(origin, destination, occlusionLayers)) return false;

        return true;
    }

    // Creates a triangulated cone mesh
    Mesh CreateWedgeMesh() {
        Mesh mesh = new Mesh();

        int segments = 16;
        int numTriangles = (segments * 4) + 2 + 2;
        int numVertices = numTriangles * 3;

        Vector3[] vertices = new Vector3[numVertices];
        int[] triangles = new int[numVertices];

        // Bottom vertices of the cone (bottom triangle)
        Vector3 bottomCenter = Vector3.up * lowerBound;
        Vector3 bottomLeft = Quaternion.Euler(0, -angle, 0) * Vector3.forward * distance + Vector3.up * lowerBound;
        Vector3 bottomRight = Quaternion.Euler(0, angle, 0) * Vector3.forward * distance + Vector3.up * lowerBound;

        // Top vertices of the Cone (top triangle)
        Vector3 topCenter = bottomCenter + Vector3.up * height;
        Vector3 topLeft = bottomLeft + Vector3.up * height;
        Vector3 topRight = bottomRight + Vector3.up * height;

        int verts = 0;

        // Create triangles for these sides (like a vertex array buffer)
        // Left side
        vertices[verts++] = bottomCenter;
        vertices[verts++] = bottomLeft;
        vertices[verts++] = topLeft;

        vertices[verts++] = topLeft;
        vertices[verts++] = topCenter;
        vertices[verts++] = bottomCenter;

        // right side
        vertices[verts++] = bottomCenter;
        vertices[verts++] = topCenter;
        vertices[verts++] = topRight;

        vertices[verts++] = topRight;
        vertices[verts++] = bottomRight;
        vertices[verts++] = bottomCenter;

        // Split arc into multiple segments to make it more smooth
        float currentAngle = -angle;
        float deltaAngle = (angle * 2) / segments;
        for(int i = 0; i < segments; i++) {
            // recalculate vertices for the triangles in the segment
            bottomLeft = Quaternion.Euler(0, currentAngle, 0) * Vector3.forward * distance + Vector3.up * lowerBound;
            bottomRight = Quaternion.Euler(0, currentAngle + deltaAngle, 0) * Vector3.forward * distance + Vector3.up * lowerBound;

            topLeft = bottomLeft + Vector3.up * height;
            topRight = bottomRight + Vector3.up * height;

            // Far side
            vertices[verts++] = bottomLeft;
            vertices[verts++] = bottomRight;
            vertices[verts++] = topRight;

            vertices[verts++] = topRight;
            vertices[verts++] = topLeft;
            vertices[verts++] = bottomLeft;

            // Top side
            vertices[verts++] = topCenter;
            vertices[verts++] = topLeft;
            vertices[verts++] = topRight;

            // Bottom side
            vertices[verts++] = bottomCenter;
            vertices[verts++] = bottomRight;
            vertices[verts++] = bottomLeft;

            currentAngle += deltaAngle;
        }


        for (int i = 0; i < numVertices; i++) {
            triangles[i] = i;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        return mesh;
    }

    private void OnValidate() {
        mesh = CreateWedgeMesh();
    }

    private void OnDrawGizmos() {
        if (mesh) {
            Gizmos.color = meshColor;
            Gizmos.DrawMesh(mesh, transform.position, transform.rotation);
        }

        for (int i = 0; i < count; i++) {
            Gizmos.DrawSphere(colliders[i].transform.position, 0.4f);
        }

        Gizmos.color = Color.green;
        foreach (var obj in objects) {
            Gizmos.DrawSphere(obj.transform.position, 0.4f);
        }
    }
}
