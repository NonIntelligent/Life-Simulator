using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

/// <summary>
/// Generate terrain using the Diamond Square algorithm
/// </summary>
[ExecuteAlways]
[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour
{
    public int map_Size = 32;
    int _real_map_size = 33;
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

        if (seed <= 0) seed = (int) DateTime.Now.Ticks;

        _real_map_size = CorrectMapSize(map_Size);

        CreateShape();
        BuildTextureData();
        UpdateMesh();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CreateShape() {
        int numStrips = _real_map_size - 1;

        GenerateAlgorithms.SetRandomSeed(seed);

        vertices = new Vector3[_real_map_size * _real_map_size];
        float[,] terrainMap = new float[_real_map_size, _real_map_size];

        terrainMap[0, 0] = initialHeight.x;
        terrainMap[numStrips, 0] = initialHeight.y;
        terrainMap[0, numStrips] = initialHeight.z;
        terrainMap[numStrips, numStrips] = initialHeight.w;

        GenerateAlgorithms.DiamondSquare(terrainMap, _real_map_size, _real_map_size, randomRange, roughness);
        int i = 0;

        // Set height values for the vertices across the map
        for (int z = 0; z < _real_map_size; z++) {
            for (int x = 0; x < _real_map_size; x++) {
                vertices[i] = new Vector3(x, terrainMap[x, z], z);
                i++;
            }
        }

        int vert = 0;
        int tris = 0;
        triangles = new int[_real_map_size * _real_map_size * 6];

        for (int x = 0; x < numStrips; x++) {
            // create the index data for the triangles
            for (int z = 0; z < numStrips; z++) { 
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + numStrips + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + numStrips + 1;
                triangles[tris + 5] = vert + numStrips + 2;

                vert++;
                tris += 6;
            }
            vert++;
        }

    }

    void BuildTextureData() {
        float textureS = (float)_real_map_size * 0.04f;
        float textureT = (float)_real_map_size * 0.04f;
        int i = 0;
        float scaleC, scaleR;

        float divisor = _real_map_size - 1f;
        uv = new Vector2[_real_map_size * _real_map_size];

        for (int x = 0; x < _real_map_size; x++) {
            for (int z = 0; z < _real_map_size; z++) {
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
