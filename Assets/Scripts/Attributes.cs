using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attributes
{
    readonly Genetics genes;

    // Derived stats determined by genetics, traits and features
    float attack;
    float defense;
    float speed;
    float attackSpeed;
    float weight;
    float energyRegenRate;
    float size; // as radius in meters

    // Maximum stats derived from attributes
    public float maxHealth;
    float maxSaturation;
    float maxEnergy;

    // Current stats
    public float health; // 0 -> maxHealth
    public float saturation; // 0.0 -> 1.0
    public float calories; // Kj
    public float energy; // 0  -> maxEnergy

    public Attributes(Genetics genetics) {
        genes = genetics;

        // Use genes to determine base stats
        maxHealth = 100f;
        maxSaturation = 100f;
        maxEnergy = 100f;

        // Calculate all derived stats

        // Assign current stats to maximum
        health = maxHealth;
        saturation = maxSaturation;
        calories = 2000f * 5f;
        energy = maxEnergy;
    }
}
