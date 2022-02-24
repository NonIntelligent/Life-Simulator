using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

/// <summary>
/// Generate terrain using the Diamond Square algorithm
/// </summary>
[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour
{
    public int map_Size = 32;
    int diamond_Square_size = 33;
    public int seed = 123;
    public float randomRange = 5f;
    public float roughness = 1f;
    public Vector4 initialHeight;
    Mesh mesh;

    Vector3[] vertices;
    Vector2[] uv;
    int[] triangles;

    // Start is called before the first frame update
    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32; // Support 4 billion indicies

        if (seed <= 0) seed = (int) DateTime.Now.Ticks;

        diamond_Square_size = CorrectMapSize(map_Size);

        CreateShape();
        StartCoroutine(BuildTriangles());
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMesh();
    }

    void CreateShape() {
        int numStrips = diamond_Square_size - 1;

        GenerateAlgorithms.SetRandomSeed(seed);

        vertices = new Vector3[diamond_Square_size * diamond_Square_size];
        float[,] terrainMap = new float[diamond_Square_size, diamond_Square_size];

        terrainMap[0, 0] = initialHeight.x;
        terrainMap[numStrips, 0] = initialHeight.y;
        terrainMap[0, numStrips] = initialHeight.z;
        terrainMap[numStrips, numStrips] = initialHeight.w;

        GenerateAlgorithms.DiamondSquare(terrainMap, diamond_Square_size, diamond_Square_size, randomRange, roughness);
        int i = 0;

        // Set height values for the vertices across the map
        for (int x = 0; x < diamond_Square_size; x++) {
            for (int z = 0; z < diamond_Square_size; z++) {
                vertices[i] = new Vector3(x, terrainMap[x, z], z);
                i++;
            }
        }

        triangles = new int[numStrips * numStrips * 6];

        BuildTextureData();

    }

    IEnumerator BuildTriangles() {
        int numStrips = diamond_Square_size - 1;

        int vert = 0;
        int tris = 0;

        for (int x = 0; x < numStrips; x++) {
            // create the index data for the triangles
            for (int z = 0; z < numStrips; z++) {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + 1;
                triangles[tris + 2] = vert + numStrips + 2;
                triangles[tris + 3] = vert + numStrips + 2;
                triangles[tris + 4] = vert + numStrips + 1;
                triangles[tris + 5] = vert + 0;

                vert++;
                tris += 6;
            }
            vert++;
            yield return new WaitForSeconds(0.01f);
        }
    }

    void BuildTextureData() {
        float textureS = (float)diamond_Square_size * 0.04f;
        float textureT = (float)diamond_Square_size * 0.04f;
        int i = 0;
        float scaleC, scaleR;

        float divisor = diamond_Square_size - 1f;
        uv = new Vector2[diamond_Square_size * diamond_Square_size];

        for (int x = 0; x < diamond_Square_size; x++) {
            for (int z = 0; z < diamond_Square_size; z++) {
                scaleC = x / divisor;
                scaleR = z / divisor;
                uv[i] = new Vector2(textureS * scaleC, textureT * scaleR);
                i++;
            }
        }
    }

    void UpdateMesh() {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uv;

        mesh.RecalculateNormals();
    }

    int CorrectMapSize(int mapSize) {
        // Find next power of 2
        //////////////////////////
        mapSize--;
        mapSize |= mapSize >> 1;
        mapSize |= mapSize >> 2;
        mapSize |= mapSize >> 4;
        mapSize |= mapSize >> 8;
        mapSize |= mapSize >> 16;
        mapSize++;
        /////////////////////////
        
        mapSize++;
        return mapSize;
    }
}
