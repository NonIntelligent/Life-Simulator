using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Spawner : MonoBehaviour
{
    public GameObject creature;
    public Transform spawnCentre;
    public float spawnRadius = 60f;

    List<GameObject> objects;

    // Start is called before the first frame update
    void Start()
    {
        objects = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public List<GameObject> spawnObjects(int numCreatures) {
        objects.Clear();
        // Create creatures to manage
        Vector3 position = spawnCentre.position + Random.insideUnitSphere * spawnRadius;
        NavMeshHit ray;
        bool navPointFound = NavMesh.SamplePosition(position, out ray, spawnRadius / 2, NavMesh.AllAreas);

        for (int i = 0; i < numCreatures; i++) {
            // Keep sampling until suitable position is found
            while (!navPointFound) { 

                position = spawnCentre.position + Random.insideUnitSphere * spawnRadius;
                navPointFound = NavMesh.SamplePosition(position, out ray, 5f, NavMesh.AllAreas);
            }

            GameObject obj = Instantiate(creature, ray.position, Quaternion.identity);
            objects.Add(obj);
            navPointFound = false;
        }

        return objects;
    }

    public List<GameObject> spawnObjectsAroundPoint(int numObjects, Vector3 pos, float radius) {
        List<GameObject> objects = new List<GameObject>();

        // Create creatures to manage
        Vector3 position = pos + Random.insideUnitSphere * radius;
        NavMeshHit ray;
        bool navPointFound = NavMesh.SamplePosition(position, out ray, spawnRadius / 2, NavMesh.AllAreas);

        for (int i = 0; i < numObjects; i++) {
            // Keep sampling until suitable position is found
            while (!navPointFound) {

                position = pos + Random.insideUnitSphere * spawnRadius;
                navPointFound = NavMesh.SamplePosition(position, out ray, 5f, NavMesh.AllAreas);
            }

            GameObject obj = Instantiate(creature, ray.position, Quaternion.identity);
            objects.Add(obj);
            navPointFound = false;
        }

        return objects;
    }


}
