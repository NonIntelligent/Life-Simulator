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
    static Dictionary<GENOME, float[]> genome_Values = null;

    GeneFactor<float> athleticism;
    GeneFactor<float> offense;
    GeneFactor<float> defense;
    GeneFactor<float> size;

    // Randomly generate all genes
    public Genetics()
    {
        // Create constant default values for the genomes for each DNA a single time
        if(genome_Values == null)
        {
            genome_Values = new Dictionary<GENOME, float[]>((int) GENOME.SIZE);
            genome_Values.Add(GENOME.ATHLETICISM, new float[] { 1.4f, 1.18f, 1f, 0.8f, 0.5f});
            genome_Values.Add(GENOME.OFFENSE, new float[] { 30f, 23f, 15f, 10f, 5f});
            genome_Values.Add(GENOME.DEFENSE, new float[] { 20f, 16f, 13f, 10f, 7f});
            genome_Values.Add(GENOME.SIZE, new float[] { 1.2f, 0.8f, 0.5f, 0.3f, 0.1f});
        }
        // get value assigned to genome from lookup table

        int dnaSelection = Random.Range(0, (int) DNA.NUM_DNA - 1);
        float[] values;
        bool exists = genome_Values.TryGetValue(GENOME.ATHLETICISM, out values);

        athleticism = new GeneFactor<float>(1, 1, 0.05f, values[dnaSelection], (DNA)dnaSelection);
    }

    // Generate genes based on parents (limits mutation chance and mutation range)
    public Genetics(Genetics[] parents, Genetics[] grandParents)
    {

    }

    bool CreateGeneFactor<T>(out GeneFactor<T> factor, T value)
    {
        factor = new GeneFactor<T>(5, 5, 3f, value, DNA.DISORDER);
        return true;
    }
}

public class GeneFactor<T>
{
    public readonly Pair<T, DNA> gene;

    public int mutationRange_higher;
    public int mutationRange_lower;
    public float mutationRate_base; // Volatility of the gene (Frequency of change between generations)

    public float experience; // Affects the +-chance of mutation for the next generation as well as mutation range

    public GeneFactor(int mutationHigher, int mutationLower, float baseMutationRate, T value, DNA genome)
    {
        gene = new Pair<T,DNA>(value, DNA.GREATER);
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
    ATHLETICISM,
    OFFENSE,
    DEFENSE,
    SIZE,

    NUM_GENOMES
}
