using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;


[RequireComponent(typeof(NavMeshAgent))]
public class CreatureAI : MonoBehaviour
{
    public Image icon;

    NavMeshAgent agent;

    FuzzyDefinitions logic;
    CreatureControl control;
    public AIstates previousState { get; private set; }
    public AIstates currentState { get; private set; }

    List<CreatureControl> creaturesInVicinity = new List<CreatureControl>();
    List<FoodZone> foodZonesInVicinity = new List<FoodZone>();
    CreatureControl targetCreature = null;

    float coroutineDelay = 0.5f;
    float moveCooldown = 1.4f;
    float wanderRadius = 24.0f;
    float stateChangeLockout = 1f;
    float attackTimeout = 7f;
    public float vicinityRadius = 18f;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        logic = GetComponent<FuzzyDefinitions>();
        control = GetComponent<CreatureControl>();

        StartCoroutine(RunAI());
    }

    // Update is called once per frame
    void Update()
    {
        // Rotate icon image towards camera
        // https://answers.unity.com/questions/319924/object-rotation-relative-to-camera.html
        icon.transform.rotation = Quaternion.LookRotation(-Camera.main.transform.forward, Camera.main.transform.up);

        checkSurroundings(vicinityRadius);
    }

    IEnumerator RunAI() {
        // Avoids getting stuck in AI due to game pause
        while (Time.timeScale <= 0f) {
            yield return new WaitForSeconds(0.2f);
        }

        yield return new WaitForSeconds(coroutineDelay / Time.timeScale);

        determineState();

        actionOnState();

        if (currentState != AIstates.DEATH) StartCoroutine(RunAI());
    }

    // Use FuzzyLogic and current state to determine new state
    void determineState() {
        // short Lockout between changing state
        stateChangeLockout -= coroutineDelay / Time.timeScale;
        if (stateChangeLockout > 0f) return;

        // Check if there's any opponents first
        List<CreatureControl> opponents = new List<CreatureControl>();
        foreach (CreatureControl creature in creaturesInVicinity) {
            if(control.family != creature.family) {
                opponents.Add(creature);

            }
        }

        // Change to attack state
        bool attack = determineAttackState(opponents);

        // Change to searching for food
        bool food = determineFoodSearchState();

        // Slime is doing nothing so it should wander
        if (currentState == AIstates.IDLE) {
            changeState(AIstates.WANDER);
        }
    }

    // Determines whether it should switch to the attack state
    bool determineAttackState(List<CreatureControl> opponents) {
        GENOME genome = GENOME.AGRESSION;

        foreach (CreatureControl opponent in opponents) {
            if (opponent == null) continue;

            // Fuzzy logic based on agression gene and distance
            CurvePair attackAgression = new CurvePair(logic.attack_creature, logic.genetics.GetFactor<float>(genome).value);
            float distanceRemap = Vector3.Distance(transform.position, opponent.transform.position);
            distanceRemap = Fuzzy.Remap(distanceRemap, 0f, vicinityRadius, 1f, 0f);
            CurvePair distance = new CurvePair(logic.distance_linear, distanceRemap);
            CurvePair healthy = new CurvePair(logic.health_Healthy, control.attributes.health / control.attributes.maxHealth);

            // Chance to attack is determined by lowest evaluation
            // so pacifict creatures are still less likly to attack even when close
            float chance = Fuzzy.AND(distance, attackAgression);
            chance = Fuzzy.AND(chance, healthy);
            // Magic scalar to reduce attacking chance as the world went crazy
            chance *= 0.3f;

            if (Fuzzy.Roll(chance)) {
                targetCreature = opponent;
                changeState(AIstates.ATTACK);
                return true;
            }
        }

        return false;
    }

    bool determineFoodSearchState() {
        foreach (FoodZone zone in foodZonesInVicinity) {
            float saturationRatio = logic.attributes.saturation / logic.attributes.maxSaturation;

            // Fuzzy logic based on saturation
            CurvePair hungerStarving = new CurvePair(logic.hunger_starving, saturationRatio);

            // Chance to search for food.
            float chance = logic.hunger_starving.Evaluate(saturationRatio);

            if (Fuzzy.Roll(chance)) {
                changeState(AIstates.RESOURCE_SEARCH);
                agent.SetDestination(zone.transform.position);
                return true;
            }
        }

        return false;
    }

    // Perform actions based on states and fuzzy logic
    void actionOnState() {
        if (currentState == AIstates.WANDER) {
            moveRandomly();
        }
        else if (currentState == AIstates.ATTACK) {
            // opponent died or there's no target
            if (targetCreature == null || attackTimeout <= 0f) {
                changeState(AIstates.IDLE);
                attackTimeout = 7f;
                return;
            }
            agent.SetDestination(targetCreature.transform.position);
            attackTimeout -= coroutineDelay / Time.timeScale;
        }
        else if (currentState == AIstates.DEFEND) {
            moveRandomly();
        }
        else if (currentState == AIstates.RESOURCE_SEARCH) {
            moveToFoodZoneAndConsume();
        }
        
    }

    void moveRandomly() {
        if (!checkIfReachedDestination(1.5f)) return;

        // Wander cooldown before selecting new position
        moveCooldown -= coroutineDelay / Time.timeScale;
        if (moveCooldown > 0f) return;

        // Find position on navmesh to move to
        Vector3 randomPoint = transform.position + Random.insideUnitSphere * wanderRadius;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, agent.height * wanderRadius, NavMesh.AllAreas)) {
            agent.SetDestination(hit.position);
        }

        moveCooldown = (1.4f + Random.Range(-0.5f, 1f)) / Time.timeScale;
    }

    void moveToFoodZoneAndConsume() {
        if (!checkIfReachedDestination(4f)) return;

        control.attributes.restoreSaturation(0.7f);
        agent.SetDestination(transform.position);

        changeState(AIstates.IDLE);

    }

    void OnDrawGizmos() {
        Gizmos.color = Color.cyan;
        if (agent) { 
            Gizmos.DrawSphere(agent.destination, 0.5f);
            //Gizmos.DrawWireSphere(transform.position, vicinityRadius);
            Gizmos.DrawLine(transform.position, agent.destination);
        }
    }

    bool checkIfReachedDestination(float minimum) {
        
        if (agent.destination == Vector3.positiveInfinity) return false;

        return Vector3.Distance(transform.position, agent.destination) <= minimum;
    }

    void checkSurroundings(float radius) {
        creaturesInVicinity.Clear();
        foodZonesInVicinity.Clear();

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);

        foreach(Collider hit in hitColliders) { 
            // Creature is nearby
            if(hit.tag == "Creature") {
                creaturesInVicinity.Add(hit.gameObject.GetComponent<CreatureControl>());
            }
            else if (hit.tag == "ResourceZone") {
                foodZonesInVicinity.Add(hit.gameObject.GetComponent<FoodZone>());
            }
        }
    }

    void updateIconState(AIstates state) { 
        Sprite sprite = EnumImageLink.getSpriteFromState(state);
        icon.sprite = sprite;
    }

    public void changeState(AIstates state) { 
        previousState = currentState;
        currentState = state;
        updateIconState(state);
        stateChangeLockout = 1f;
    }

}
