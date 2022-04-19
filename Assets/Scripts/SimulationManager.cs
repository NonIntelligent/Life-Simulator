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
    public Material nature;
    public Material blood;
    public Material fruit;

    // Start is called before the first frame update
    void Start()
    {
        CreatureSpawner = GetComponent<Spawner>();

        numberToSpawn = Mathf.Max(1, numberToSpawn);

        creatures = new List<GameObject>();

        // Create all creatures in family groups
        List<GameObject> natureCreatures = CreatureSpawner.spawnObjects(numberToSpawn);
        foreach (GameObject obj in natureCreatures)
        {
            var control = obj.GetComponent<CreatureControl>();
            control.family = CreatureFamily.NATURE;
            var components = obj.GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer mesh in components)
            {
                if (mesh.material.name == "phong1 (Instance)")
                {
                    mesh.material = nature;
                }
            }
        }
        List<GameObject> bloodcreatures = CreatureSpawner.spawnObjects(numberToSpawn);
        foreach (GameObject obj in bloodcreatures)
        {
            var control = obj.GetComponent<CreatureControl>();
            control.family = CreatureFamily.BLOOD;
            var components = obj.GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer mesh in components)
            {
                if (mesh.material.name == "phong1 (Instance)")
                {
                    mesh.material = blood;
                }
            }
        }
        List<GameObject> fruitCreatures = CreatureSpawner.spawnObjects(numberToSpawn);
        foreach (GameObject obj in fruitCreatures)
        {
            var control = obj.GetComponent<CreatureControl>();
            control.family = CreatureFamily.FRUIT;
            var components = obj.GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer mesh in components)
            {
                if (mesh.material.name == "phong1 (Instance)")
                {
                    mesh.material = fruit;
                }
            }
        }

        creatures.AddRange(natureCreatures);
        creatures.AddRange(bloodcreatures);
        creatures.AddRange(fruitCreatures);
    }

    // Update is called once per frame
    void Update()
    {
        List<GameObject> objectToRemove = new List<GameObject>();

        for (int i = 0; i < creatures.Count; i++)
        {
            GameObject obj = creatures[i];
            if (obj == null) {
                objectToRemove.Add(obj);
                continue;
            }
            // Get components from creature and evaluate based on stats
            CreatureControl control = obj.GetComponent<CreatureControl>();
            FuzzyDefinitions fuzzy = obj.GetComponent<FuzzyDefinitions>();

            Attributes att = control.attributes;
        }

        // remove null objects from list
        foreach (GameObject obj in objectToRemove) {
            creatures.Remove(obj);
        }
    }

    public void SpawnNextGeneration() {
        Debug.Log("Spawn stuff with the next generation");
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
