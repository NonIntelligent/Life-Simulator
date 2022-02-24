using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CreatureManager))]
public class SimulationManager : MonoBehaviour
{
    CreatureManager creatureManager;
    // Start is called before the first frame update
    void Start()
    {
        creatureManager = GetComponent<CreatureManager>();

        creatureManager.spawnCreatures(6, 2);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
