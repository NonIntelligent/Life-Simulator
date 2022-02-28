using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CreatureManager : MonoBehaviour
{
    public GameObject creature;
    public Transform spawnCentre;
    public float spawnRadius = 20f;

    List<GameObject> creatures;

    // Start is called before the first frame update
    void Start()
    {
        creatures = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public List<GameObject> spawnCreatures(int numCreatures, int numFamilies) {
        // Create creatures to manage
        Vector3 position = spawnCentre.position + Random.insideUnitSphere * spawnRadius;
        NavMeshHit ray;
        bool navPointFound = NavMesh.SamplePosition(position, out ray, 5f, NavMesh.AllAreas);

        for (int i = 0; i < numFamilies; i++) {
            for (int j = 0; j < numCreatures; j++) {
                // Keep sampling until suitable position is found
                while (!navPointFound) { 

                    position = spawnCentre.position + Random.insideUnitSphere * spawnRadius;
                    navPointFound = NavMesh.SamplePosition(position, out ray, 5f, NavMesh.AllAreas);
                }

                GameObject obj = Instantiate(creature, ray.position, Quaternion.identity);
                creatures.Add(obj);
            }
        }

        return creatures;
    }


}
