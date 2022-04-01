using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Spawner))]
public class SimulationManager : MonoBehaviour
{
    Spawner CreatureSpawner;

    List<GameObject> creatures;
    bool gamePaused = false;
    public int numberToSpawn = 10;

    // Start is called before the first frame update
    void Start()
    {
        CreatureSpawner = GetComponent<Spawner>();

        numberToSpawn = Mathf.Max(1, numberToSpawn);

        // Create all creatures in family groups
        List<GameObject> natureCreatures = CreatureSpawner.spawnObjects(numberToSpawn);
        foreach (GameObject obj in natureCreatures)
        {
            var control = obj.GetComponent<CreatureControl>();
            control.family = CreatureFamily.NATURE;
        }
        List<GameObject> bloodcreatures = CreatureSpawner.spawnObjects(numberToSpawn);
        foreach (GameObject obj in bloodcreatures)
        {
            var control = obj.GetComponent<CreatureControl>();
            control.family = CreatureFamily.BLOOD;
        }
        List<GameObject> fruitCreatures = CreatureSpawner.spawnObjects(numberToSpawn);
        foreach (GameObject obj in fruitCreatures)
        {
            var control = obj.GetComponent<CreatureControl>();
            control.family = CreatureFamily.FRUIT;
        }

        creatures.AddRange(natureCreatures);
        creatures.AddRange(bloodcreatures);
        creatures.AddRange(fruitCreatures);
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

    public void PauseResumeGame()
    {
        if (Time.timeScale <= 0f)
        {
            gamePaused = false;
            Time.timeScale = 1f;
        }
        else
        {
            gamePaused = true;
            Time.timeScale = 0f;
        }
    }

    public void SpeedUpDownSimulation(float multiplier)
    {
        gamePaused = false;
        float scale = multiplier <= 0f ? 1f : multiplier;
        Time.timeScale = scale;
    }

    public void CloseGame()
    {
        Application.Quit();
    }
}
