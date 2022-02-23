
public enum ZombieStates
{
    WANDER, PATROL, GUARD, CHASE,
    ATTACK,
    DEATH,
}

public enum WaypointType
{
    CYCLIC, // A -> B -> C -> A...
    RETRACE, // A -> B -> C -> B -> A...
}
