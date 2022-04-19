using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

[System.Serializable]
public class CreatureControlEvent : UnityEvent<CreatureControl>
{

}

public class CreatureControl : MonoBehaviour
{
    // Events
    CreatureControlEvent onTakeDamage;

    Genetics genetics;
    public Attributes attributes { get; private set; }
    BoxCollider coll;
    NavMeshAgent agent;
    CreatureAI creatureAI;
    FuzzyDefinitions definitions;

    Genetics[] grandParents = new Genetics[4];
    Genetics[] parents = new Genetics[2];
    public CreatureFamily family { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        genetics = new Genetics();
        attributes = new Attributes(genetics);
        transform.localScale = new Vector3(1f, 1f, 1f) * attributes.size;
        coll = GetComponent<Collider>() as BoxCollider;
        agent = GetComponent<NavMeshAgent>();
        creatureAI = GetComponent<CreatureAI>();
        definitions = GetComponent<FuzzyDefinitions>();
        definitions.genetics = genetics;
        definitions.attributes = attributes;

        setupEvents();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale <= 0f) return;

        float timePassed = Time.deltaTime / Chonometer.secondsInDay;

        // Lose 0.5 saturation per day
        attributes.saturation -= 0.5f * (timePassed);

        // Restore 25 health per day
        if (attributes.saturation >= 0.3f) attributes.restoreHealth(25f * (timePassed));

        if (attributes.saturation <= 0f) die();
    }

    void setupEvents() {
        if (onTakeDamage == null) onTakeDamage = new CreatureControlEvent();

        onTakeDamage.AddListener(recievedDamage);
    }

    public void UpdateSpeed(float speed) {
        agent.speed = speed;
        agent.acceleration = speed / 2f;
    }

    public void dealDamage(CreatureControl opponent) {
        float damage = opponent.attributes.attack - attributes.defense;
        damage = Mathf.Max(damage, 3f);

        attributes.health -= damage;
        onTakeDamage.Invoke(opponent);
    }

    void recievedDamage(CreatureControl opponent) {
        var currentState = creatureAI.currentState;
        // Change state to defending if you recieved damage but did not initiate the attack
        if (currentState != AIstates.ATTACK && currentState != AIstates.DEFEND) {
            creatureAI.changeState(AIstates.DEFEND);
        }

        // Fills hunger for opponent on death
        if (attributes.health <= 0f) {
            opponent.attributes.UpdateSaturation(opponent.attributes.saturation + attributes.saturation * 0.5f);
            die();
        }
    }

    void die() {
        creatureAI.changeState(AIstates.DEATH);
        Destroy(gameObject);
    }

}
