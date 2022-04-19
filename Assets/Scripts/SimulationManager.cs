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
        List<GameObject> familyNature = new List<GameObject>();
        List<GameObject> familyFruit = new List<GameObject>();
        List<GameObject> familyBlood = new List<GameObject>();
        List<GameObject> nextGeneration = new List<GameObject> ();

        // Sort lists by family
        foreach (GameObject obj in creatures) {
            if (obj == null) continue;

            var control = obj.GetComponent<CreatureControl>();

            if(control.family == CreatureFamily.FRUIT) {
                familyFruit.Add(obj);
            }
            else if (control.family == CreatureFamily.BLOOD) {
                familyBlood.Add(obj);
            }
            else familyNature.Add(obj);
        }

        nextGeneration.AddRange(generateOffspring(familyNature));
        nextGeneration.AddRange(generateOffspring(familyFruit));
        nextGeneration.AddRange(generateOffspring(familyBlood));

        // Destroy all parent creatures
        foreach (GameObject obj in creatures) {
            if(obj == null) continue;

            Destroy(obj);
        }

        creatures.Clear();

        creatures.AddRange(nextGeneration);
    }

    List<GameObject> generateOffspring(List<GameObject> candidates) {
        List<GameObject> offspring = new List<GameObject>();

        // Generate offspring using genetics of parents
        for (int i = 1; i < candidates.Count; i += 2) {
            var parent1 = candidates[i].GetComponent<CreatureControl>();
            var parent2 = candidates[i - 1].GetComponent<CreatureControl>();

            Genetics[] parents = { parent1.GetGenetics(), parent2.GetGenetics() };

            Genetics[] grandparents = { parent1.GetParents()[0], parent1.GetParents()[1], parent2.GetParents()[0], parent2.GetParents()[1] };

            // Check if grandparents contain any null genetics
            for (int gp = 0; gp < 4; gp++) {
                if (grandparents[gp] == null) {
                    grandparents = null;
                    break;
                }
            }

            var newborns = CreatureSpawner.spawnObjects(Random.Range(0, 4));

            foreach (GameObject newborn in newborns) {
                newborn.GetComponent<CreatureControl>().generateGenetics(parents, grandparents);
            }

            offspring.AddRange(newborns);

        }

        return offspring;
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
