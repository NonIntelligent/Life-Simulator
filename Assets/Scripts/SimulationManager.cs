using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CreatureManager))]
public class SimulationManager : MonoBehaviour
{
    CreatureManager creatureManager;
    FuzzyDefinitions fuzzy;

    List<GameObject> creatures;

    // Start is called before the first frame update
    void Start()
    {
        creatureManager = GetComponent<CreatureManager>();
        fuzzy = GetComponent<FuzzyDefinitions>();

        creatures = creatureManager.spawnCreatures(6, 2);
    }

    // Update is called once per frame
    void Update()
    {
        foreach(GameObject obj in creatures)
        {
            CreatureControl control = obj.GetComponent<CreatureControl>();
            Attributes att = control.attributes;
            float[] values = fuzzy.EvaluateHealth(att.health / att.maxHealth);

            Debug.Log("Healthy: " + values[0] + ", Hurt: " + values[1] + ", Critical: " + values[2]);

            if (values[0] > values[1]) control.UpdateSpeed(20f);
            else if (values[1] > values[2]) control.UpdateSpeed(5f);
            else if (values[2] > values[1]) control.UpdateSpeed(1f);

        }
    }
}
