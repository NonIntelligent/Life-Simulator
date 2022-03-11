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
    GeneFactor<float> athleticism;
    GeneFactor<float> offense;
    GeneFactor<float> defense;
    GeneFactor<float> size;

    // Randomly generate all genes
    public Genetics()
    {

    }

    // Generate genes based on parents (limits mutation chance and mutation range)
    public Genetics(Genetics[] parents, Genetics[] grandParents)
    {

    }
}

public class GeneFactor<T>
{
    public readonly Pair<T, GENES> gene;

    public int mutationRange_higher;
    public int mutationRange_lower;
    public float mutationRate_base; // Volatility of the gene (Frequency of change between generations)

    public float experience; // Affects the +-chance of mutation for the next generation as well as mutation range

    public GeneFactor(int mutationHigher, int mutationLower, float baseMutationRate, T value)
    {
        gene = new Pair<T,GENES>(value, GENES.GREATER);
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

public enum GENES
{
    GREATER = 4,
    POSITIVE = GREATER - 1,
    NEUTRAL = POSITIVE - 1,
    NEGATIVE = NEUTRAL - 1,
    DISORDER = NEGATIVE - 1
};
