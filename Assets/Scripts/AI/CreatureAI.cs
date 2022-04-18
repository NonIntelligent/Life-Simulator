﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(FuzzyAI))]
[RequireComponent(typeof(NavMeshAgent))]
public class CreatureAI : MonoBehaviour
{
    NavMeshAgent agent;
    FuzzyAI fuzzyAI;

    float coroutineDelay = 0.5f;
    float moveCooldown = 2.8f;
    float wanderRadius = 24.0f;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        fuzzyAI = GetComponent<FuzzyAI>();

        StartCoroutine(RunAI());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator RunAI() {
        while (Time.timeScale <= 0f) {
            yield return new WaitForSeconds(0.2f);
        }

        yield return new WaitForSeconds(coroutineDelay / Time.timeScale);

        moveRandomly();

        StartCoroutine(RunAI());
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
}
