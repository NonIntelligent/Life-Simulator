using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GENOME;

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
    public float attack { get; }
    public float defense { get; }
    public float speed { get; }
    public float attackSpeed { get; }
    public float weight { get; }
    public float energyRegenRate { get; }
    public float size { get; }

    // Maximum stats derived from attributes
    public float maxHealth { get; }
    public float maxSaturation { get; } // creature hunger levels
    public float maxEnergy { get; } // Similar to action/sprint stamina

    // Current stats
    public float health = 100f; // 0 -> maxHealth
    public float saturation = 1f; // 0.0 -> maxSaturation
    public float energy = 100f; // 0  -> maxEnergy

    public Attributes(Genetics genetics) {
        genes = genetics;

        // Use genes to determine maximum stats
        maxHealth = 100f * genes.GetFactorValue<float>(ATHLETICISM) * genes.GetFactorValue<float>(SIZE);
        maxSaturation = genes.GetFactorValue<float>(SIZE);
        maxEnergy = 100f * genes.GetFactorValue<float>(ATHLETICISM);

        // Calculate all derived stats
        attack = genes.GetFactorValue<float>(OFFENSE);
        defense = genes.GetFactorValue<float>(DEFENSE);
        speed = 4f * (1f / genes.GetFactorValue<float>(SIZE)) +  2f * genes.GetFactorValue<float>(ATHLETICISM); // Size has a greater influence than athleticism
        attackSpeed = 0.5f * (1f / genes.GetFactorValue<float>(SIZE)) + 0.5f * genes.GetFactorValue<float>(ATHLETICISM);
        weight = Mathf.Pow(genes.GetFactorValue<float>(SIZE), 3f);
        energyRegenRate = 5f * genes.GetFactorValue<float>(ATHLETICISM);
        size = genes.GetFactorValue<float>(SIZE);

        // Assign current stats to maximum
        health = maxHealth;
        saturation = maxSaturation;
        energy = maxEnergy;
    }

}
