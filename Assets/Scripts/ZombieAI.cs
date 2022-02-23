using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieAI : MonoBehaviour
{
    private const float coroutineDelay = 0.2f;

    NavMeshAgent agent;
    GameObject player;
    AISensor sensor;
    Collider _collider;
    Animator _animator;

    public Transform currentTarget;
    public Vector3 lastPlayerPosition = Vector3.zero;
    public float health = 100f;
    public float damage = 40f;
    private bool dead = false;

    private ZombieStates defaultState = ZombieStates.WANDER; // The base state for this zombie (either patrol or wander)
    public ZombieStates currentState = ZombieStates.WANDER;
    public WaypointType waypointType = WaypointType.RETRACE;
    private bool backtracking = false;

    List<Transform> waypoints = new List<Transform>();
    private int targetWaypointIndex = 0;
    private int endWaypointIndex = 0;
    private float minDistance = 0.8f;
    private float guardTime = 3.0f;
    private float alertedTime = 5.0f;
    private float attackCooldown = 1.0f;
    private float wanderCooldown = 1.0f;
    public float wanderRadius = 4.0f;
    public GameObject patrolPath;

    // Waypoints Tutorial https://www.youtube.com/watch?v=GIDz0DjhA4E

    // Start is called before the first frame update
    void Start() {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");
        sensor = GetComponent<AISensor>();
        _collider = GetComponent<Collider>();
        _animator = GetComponent<Animator>();

        // Check if zombie has waypoints (Set default and current state to patrol)
        SetupWaypoints();

        StartCoroutine(RunAI());
    }

    void Update() {
        // The frame that you die
        if(health <= 0.0f && !dead) {
            dead = true;
            currentState = ZombieStates.DEATH;
            setAnimatorStates(true, ZombieStates.DEATH);
            onDeath();
        }
    }

    IEnumerator RunAI() {
        // Pause execution for some time and resume in later frames
        yield return new WaitForSeconds(coroutineDelay);

        // Check for states based on player
        UpdateState();
        setAnimatorStates(true, currentState);

        CheckDistanceToWaypoint();

        // Core actions for the AI
        switch (currentState) { 
            case ZombieStates.PATROL:
                PatrolWaypoints();
                break;
            case ZombieStates.GUARD:
                GuardWaypoint();
                break;
            case ZombieStates.ATTACK:
                AttackPlayer();
                break;
            case ZombieStates.CHASE:
                ChasePlayer();
                break;
            case ZombieStates.DEATH:
                dead = true;
                break;
            default: // WANDER and no state is default
                IamLost();
                break;
        }

        // Check if player is in line-of-sight
        // Check if Zombie has target

        // Move based on state (i.e. Wander in a circle or patrol around waypoints)

        // If chasing player and is in sight, current target should be set to the player's position

        // Restart Coroutine
        if (!dead) StartCoroutine(RunAI());

    }

    private void CheckDistanceToWaypoint() {
        if (currentState != ZombieStates.PATROL) return;

        float distance = Vector3.Distance(transform.position, currentTarget.position);

        if (distance <= minDistance) {
            currentState = ZombieStates.GUARD;
            selectNextWaypoint();
            UpdateTargetWaypoint();
            StartCoroutine(GuardWaypoint());
        }
    }

    private void UpdateTargetWaypoint() {
        currentTarget = waypoints[targetWaypointIndex];
    }

    private void PatrolWaypoints() {
        agent.SetDestination(currentTarget.position);
    }

    private int selectNextWaypoint() {
        if (!backtracking) {
            targetWaypointIndex++;
        } else {
            targetWaypointIndex--;
        }

        switch (waypointType) {
            case WaypointType.RETRACE:
                if (targetWaypointIndex > endWaypointIndex) {
                    backtracking = true;
                    targetWaypointIndex -= 2;
                }
                else if (targetWaypointIndex < 0) {
                    backtracking = false;
                    targetWaypointIndex += 2;
                }
                break;
            default :
                if (targetWaypointIndex > endWaypointIndex) {
                    targetWaypointIndex = 0;
                }
                break;
        }

        return targetWaypointIndex;
    }
    private void IamLost() {
        // Wander cooldown before selecting new position
        wanderCooldown -= coroutineDelay;
        if (wanderCooldown >= 0.0f) return;

        // Find position on navmesh to move to
        Vector3 randomPoint = transform.position + Random.insideUnitSphere * wanderRadius;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, agent.height * 2, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }

        wanderCooldown = 2.8f;
    }
    IEnumerator GuardWaypoint() {
        Debug.Log("Waiting");
        agent.isStopped = true;
        agent.ResetPath();
        //Quaternion LookPoint = Quaternion.AngleAxis(-45.0f, transform.up);
        yield return new WaitForSeconds(guardTime / 3.0f);
        //Quaternion.Slerp(transform.rotation, LookPoint, coroutineDelay);
        yield return new WaitForSeconds(guardTime / 3.0f);
        //LookPoint = Quaternion.AngleAxis(90.0f, transform.up);
        //Quaternion.Slerp(transform.rotation, LookPoint, coroutineDelay);
        yield return new WaitForSeconds(guardTime / 3.0f);
        agent.isStopped = false;
        if (currentState == ZombieStates.GUARD) currentState = ZombieStates.PATROL;
    }
    private void ChasePlayer() {
        Debug.Log("Chasing Player");
        if (lastPlayerPosition != Vector3.zero) {
            agent.ResetPath();
            agent.SetDestination(lastPlayerPosition);
            Quaternion lookRotation = Quaternion.LookRotation((lastPlayerPosition - transform.position).normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, coroutineDelay * agent.angularSpeed);
        }
    }

    private void AttackPlayer() {
        Debug.Log("Attacking Player");

        attackCooldown -= coroutineDelay;
        if (attackCooldown > 0f) return;

        // Ready to attack
        // Create vertical box collider to attack player (removed by player)
        BoxCollider bc = gameObject.AddComponent<BoxCollider>();
        BoxCollider col = _collider as BoxCollider;
        Vector3 newCenter = col.center;
        newCenter.y -= 0.1f;
        newCenter += Vector3.forward * 1.0f;
        bc.size = col.size;
        bc.center = newCenter;
        bc.isTrigger = true;

        attackCooldown = 1.0f;
    }

    private void UpdateState() {
        float distance = Vector3.Distance(transform.position, lastPlayerPosition);
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        // Check if player was in line-of-sight
        if (sensor.objects.Count > 0)
        {
            // Player SPOTTED!
            if (sensor.objects[0] == player)
            {
                currentState = ZombieStates.CHASE;
                
                currentTarget = player.transform;
                lastPlayerPosition = player.transform.position;
            }
        }
        // Reached last position and player is not found
        else if (distance <= minDistance)
        {
            currentState = ZombieStates.WANDER;
            lastPlayerPosition = Vector3.zero;
        }

        // handle case of wandering after chasing the player and return to default state
        if (currentState == ZombieStates.WANDER)
        {
            alertedTime -= coroutineDelay;
            if (alertedTime <= 0.0f)
            {
                currentState = defaultState;
                alertedTime = 5.0f;
            }
        }

        // Player is within range of attack
        if(currentState == ZombieStates.CHASE || currentState == ZombieStates.ATTACK && distanceToPlayer <= minDistance) {
            currentState = ZombieStates.ATTACK;
        }

    }

    private void SetupWaypoints() {
        if (patrolPath == null) return;

        foreach (Transform child in patrolPath.transform) {
            if (child.tag == "Waypoint") {
                waypoints.Add(child);
            }
        }

        if ((endWaypointIndex = waypoints.Count - 1) > 0) {
            defaultState = ZombieStates.PATROL;
            currentState = ZombieStates.PATROL;
        }

        UpdateTargetWaypoint();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

        if(lastPlayerPosition != Vector3.zero)
        {
            Gizmos.DrawSphere(lastPlayerPosition, 0.5f);
        }

        Gizmos.color = Color.cyan;
        if (agent) Gizmos.DrawSphere(agent.destination, 0.5f);
    }
    public void onDeath() {
        // terminate AI scripts and coroutines
        StopAllCoroutines();
        sensor.enabled = false;
        //agent.enabled = false;
        //_collider.enabled = false;

        Destroy(gameObject, 3f);
    }

    public void takeDamage(float damage) {
        health -= damage;
    }

    private void setAnimatorStates(bool active, ZombieStates state) {
        _animator.SetBool("Chasing", false);
        _animator.SetBool("Wandering", false);
        _animator.SetBool("Attacking", false);


        switch (state) { 
            case ZombieStates.CHASE:
                _animator.SetBool("Chasing", active);
                break;
            case ZombieStates.WANDER:
                _animator.SetBool("Wandering", active);
                break;
            case ZombieStates.PATROL:
                _animator.SetBool("Wandering", active);
                break;
            case ZombieStates.GUARD:
                _animator.SetBool("Idle", active);
                break;
            case ZombieStates.ATTACK:
                _animator.SetBool("Attacking", active);
                break;
            case ZombieStates.DEATH:
                _animator.SetBool("Dying", active);
                break;
            default:
                break;
        }
    }

    public bool isDead() { return dead; }
    public void setDead(bool death) { dead = death; }

    public float getDamage() { return damage; }

    
}
