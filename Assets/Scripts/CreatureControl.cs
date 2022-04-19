using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CreatureControl : MonoBehaviour
{
    Genetics genetics;
    public Attributes attributes;
    BoxCollider coll;
    NavMeshAgent agent;

    Genetics[] grandParents = new Genetics[4];
    Genetics[] parents = new Genetics[2];
    public CreatureFamily family { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        family = CreatureFamily.NATURE;
        genetics = new Genetics();
        attributes = new Attributes(genetics);
        transform.localScale = new Vector3(1f, 1f, 1f) * attributes.size;
        coll = GetComponent<Collider>() as BoxCollider;
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale <= 0f) return;

        // Lose 0.5 saturation per day
        attributes.saturation -= 0.5f * (Time.deltaTime / Chonometer.secondsInDay);
    }

    public void UpdateSpeed(float speed) {
        agent.speed = speed;
        agent.acceleration = speed / 2f;
    }

}
