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
    AIstates previousState = AIstates.IDLE;
    AIstates currentState = AIstates.IDLE;

    float coroutineDelay = 0.5f;
    float moveCooldown = 2.8f;
    float wanderRadius = 24.0f;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        StartCoroutine(RunAI());
    }

    // Update is called once per frame
    void Update()
    {
        // Rotate icon image towards camera
        // https://answers.unity.com/questions/319924/object-rotation-relative-to-camera.html
        icon.transform.rotation = Quaternion.LookRotation(-Camera.main.transform.forward, Camera.main.transform.up);
    }

    IEnumerator RunAI() {
        // Avoids getting stuck in AI due to game pause
        while (Time.timeScale <= 0f) {
            yield return new WaitForSeconds(0.2f);
        }

        yield return new WaitForSeconds(coroutineDelay / Time.timeScale);

        determineState();

        actionOnState();

        moveRandomly();

        StartCoroutine(RunAI());
    }

    // Use FuzzyLogic to determine state
    void determineState() {
        if (currentState == AIstates.IDLE) {
            changeState(AIstates.WANDER);
        }
    }

    // Perform actions based on states and fuzzy logic
    void actionOnState() {
        if (currentState == AIstates.WANDER) {
            moveRandomly();
        }
        
    }

    void moveRandomly() {
        // Check if destination has been reached
        if (agent.destination == Vector3.positiveInfinity) return;
        float distance = Vector3.Distance(transform.position, agent.destination);
        if (distance > 1.5f) return;

        // Wander cooldown before selecting new position
        moveCooldown -= coroutineDelay * Time.timeScale;
        if (moveCooldown > 0f) return;

        // Find position on navmesh to move to
        Vector3 randomPoint = transform.position + Random.insideUnitSphere * wanderRadius;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, agent.height * wanderRadius, NavMesh.AllAreas)) {
            agent.SetDestination(hit.position);
        }

        moveCooldown = (2.8f + Random.Range(-0.5f, 1f)) / Time.timeScale;
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.cyan;
        if (agent) { 
            Gizmos.DrawSphere(agent.destination, 0.5f);
            Gizmos.DrawLine(transform.position, agent.destination);
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
    }

}
