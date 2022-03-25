using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The physical stats to represent the creatures abilities and capabilities based on its genetics
/// </summary>
/// <remarks>
/// Default values are assumed base values for a generic application
/// </remarks>
public class Attributes
{
    Genetics genes { get; }

    // Derived stats determined by genetics, traits and features
    float attack;
    float defense;
    float speed;
    float attackSpeed;
    float weight;
    float energyRegenRate;
    public float size; // as radius in meters

    // Maximum stats derived from attributes
    public float maxHealth = 100f;
    float maxSaturation = 1f; // creature hunger levels
    float maxEnergy = 100f; // Similar to action/sprint stamina

    // Current stats
    public float health = 100f; // 0 -> maxHealth
    public float saturation = 1f; // 0.0 -> maxSaturation
    public float energy = 100f; // 0  -> maxEnergy

    public Attributes(Genetics genetics) {
        genes = genetics;

        // Use genes to determine base stats
        maxHealth *= genes.GetFactorValue<float>(GENOME.ATHLETICISM) + genes.GetFactorValue<float>(GENOME.SIZE);
        maxSaturation = 1f;
        maxEnergy = 100f;

        // Calculate all derived stats
        size = genes.GetFactorValue<float>(GENOME.SIZE);

        // Assign current stats to maximum
        health = maxHealth;
        saturation = maxSaturation;
        energy = maxEnergy;
    }

}
