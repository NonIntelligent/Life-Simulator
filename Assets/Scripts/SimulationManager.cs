using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Spawner))]
public class SimulationManager : MonoBehaviour
{
    Spawner CreatureSpawner;

    List<GameObject> creatures;

    // Start is called before the first frame update
    void Start()
    {
        CreatureSpawner = GetComponent<Spawner>();

        creatures = CreatureSpawner.spawnObjects(6);
    }

    // Update is called once per frame
    void Update()
    {
        foreach(GameObject obj in creatures)
        {
            // Get components from creature and evaluate based on stats
            CreatureControl control = obj.GetComponent<CreatureControl>();
            FuzzyDefinitions fuzzy = obj.GetComponent<FuzzyDefinitions>();

            Attributes att = control.attributes;
            float[] values = fuzzy.EvaluateHealth(att.health / att.maxHealth);

            //Debug.Log("Healthy: " + values[0] + " Hurt: " + values[1] + " Critical: " + values[2]);

            if (values[0] > values[1]) control.UpdateSpeed(20f);
            else if (values[1] > values[2]) control.UpdateSpeed(5f);
            else if (values[2] > values[1]) control.UpdateSpeed(1f);

        }
    }

    public void ChangeSimulationSpeed(float multiplier)
    {
        float scale = multiplier < 0f ? 0f : multiplier;
        Time.timeScale = scale;
    }

    public void CloseGame()
    {
        Application.Quit();
    }
}
