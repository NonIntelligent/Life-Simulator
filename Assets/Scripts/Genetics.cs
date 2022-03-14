using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
/// Have an enum to determine if a gene is altered to be GREATER, POSITIVE, NEUTRAL, NEGATIVE, DISORDER
/// Add mutation factor to base variablity of mutations for any given creature
/// Mutation factor is determined per gene based on parents genes (lowers chance) and their activity levels (alters mutation range)
public class Genetics
{
    // Hashtables used as a lookup table that supports multiple types
    static Hashtable genome_Values = null;

    // Attribute genes
    GeneFactor<float> athleticism;
    GeneFactor<float> offense;
    GeneFactor<float> defense;
    GeneFactor<float> size;

    // Behaviour genes
    GeneFactor<float> agression;


    // Randomly generate all genes
    public Genetics()
    {
        // Create constant default values for the genomes for each DNA a single time
        if(genome_Values == null)
        {
            genome_Values = new Hashtable((int) GENOME.SIZE);
            genome_Values.Add(GENOME.ATHLETICISM, new float[] { 0.5f, 0.8f, 1f, 1.18f, 1.4f});
            genome_Values.Add(GENOME.OFFENSE, new float[] { 5f, 10f, 15f, 23f, 30f});
            genome_Values.Add(GENOME.DEFENSE, new float[] {7f, 10f, 13f, 16f, 20f});
            genome_Values.Add(GENOME.SIZE, new float[] { 0.1f, 0.3f, 0.5f, 0.8f, 1.2f});
            genome_Values.Add(GENOME.AGRESSION, new float[] { 0.05f, 0.2f, 0.4f, 0.7f, 0.9f});
        }

        // get value assigned to genome from lookup table
        CreateGeneFactor(out athleticism, 1, 1, 0.05f, GENOME.ATHLETICISM);
        CreateGeneFactor(out offense, 1, 1, 0.05f, GENOME.ATHLETICISM);
        CreateGeneFactor(out defense, 1, 1, 0.05f, GENOME.ATHLETICISM);
        CreateGeneFactor(out size, 1, 1, 0.05f, GENOME.ATHLETICISM);

        CreateGeneFactor(out agression, 1, 1, 0.05f, GENOME.ATHLETICISM);
    }

    // Generate genes based on parents (limits mutation chance and mutation range)
    public Genetics(Genetics[] parents, Genetics[] grandParents)
    {

    }

    bool CreateGeneFactor<T>(out GeneFactor<T> factor, int highRange, int lowRange, float mutationRate, GENOME genome)
    {
        int dnaSelection = Random.Range(0, (int)DNA.NUM_DNA - 1);
        T[] values = (T[]) genome_Values[genome];


        if (values != null)
        {
            factor = new GeneFactor<T>(highRange, lowRange, mutationRate, values[dnaSelection], (DNA) dnaSelection);
            return true;
        } else {
            factor = new GeneFactor<T>(highRange, lowRange, 1f, ((T[]) (genome_Values[0]))[0], (DNA)dnaSelection);
            return false;
        }

    }
}

public class GeneFactor<T>
{
    public readonly Pair<T, DNA> gene;

    public int mutationRange_higher;
    public int mutationRange_lower;
    public float mutationRate; // Volatility of the gene (Frequency of change between generations)

    public float experience; // Affects the +-chance of mutation for the next generation as well as mutation range

    public GeneFactor(int mutationHigher, int mutationLower, float baseMutationRate, T value, DNA dna)
    {
        gene = new Pair<T,DNA>(value, dna);
        experience = 0f;
    }

}

public class Pair<T, U>
{
    public readonly T first;
    public readonly U second;

    public Pair(T item1, U item2)
    {
        first = item1;
        second = item2;
    }

}

public enum DNA
{
    DISORDER,
    NEGATIVE,
    NEUTRAL,
    POSITIVE,
    GREATER,

    NUM_DNA
}

public enum GENOME
{
    // Governs Attributes
    ATHLETICISM,
    OFFENSE,
    DEFENSE,
    SIZE,

    // Governs Behaviour
    AGRESSION,

    NUM_GENOMES
}
