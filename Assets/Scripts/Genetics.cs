using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DNA
{
    DISABILITY,
    NEGATIVE,
    NEUTRAL,
    POSITIVE,
    FREAK,

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

/// <summary>
/// 
/// </summary>
/// Have an enum to determine if a gene is altered to be GREATER, POSITIVE, NEUTRAL, NEGATIVE, DISORDER
/// Add mutation factor to base variablity of mutations for any given creature
/// Mutation factor is determined per gene based on parents genes (lowers chance) and their activity levels (alters mutation range)
public class Genetics
{
    // Readonly dictionary used as a lookup table that supports multiple types
    static IReadOnlyDictionary<GENOME, DnaConstants> genome_Values = null;

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
            genome_Values = CreateGenomeValueLookup();
        }

        // get value assigned to genome from lookup table
        GenerateRandomFactor(out athleticism, 1, 1, 0.05f, GENOME.ATHLETICISM);
        GenerateRandomFactor(out offense, 1, 1, 0.05f, GENOME.OFFENSE);
        GenerateRandomFactor(out defense, 1, 1, 0.05f, GENOME.DEFENSE);
        GenerateRandomFactor(out size, 1, 1, 0.05f, GENOME.SIZE);

        GenerateRandomFactor(out agression, 1, 1, 0.05f, GENOME.AGRESSION);
    }

    // Generate genes based on parents (limits mutation chance and mutation range)
    public Genetics(Genetics[] parents, Genetics[] grandParents)
    {
        // Create constant default values for the genomes for each DNA a single time
        if (genome_Values == null) {
            genome_Values = CreateGenomeValueLookup();
        }

        // Use parents genes to determine mutation rate, the range of mutation if successfull and the starting gene
        if (parents != null) {
            GenerateFactorsFromGenetics(parents, 2, 0.8f);
        }

        // Use grandparents' genes to determine mutation 
        if (grandParents != null) {
            GenerateFactorsFromGenetics(grandParents, 4, 0.2f);
        }

        
    }

    bool GenerateRandomFactor<T>(out GeneFactor<T> factor, int highRange, int lowRange, float mutationRate, GENOME genome)
    {
        int dnaSelection = UnityEngine.Random.Range(0, (int)DNA.NUM_DNA - 1);
        T[] values = (T[]) genome_Values[genome].dnaValuesArray;

        T val = values[dnaSelection];
        // Add variance to floating point values by +-10% 
        if (typeof(T) == typeof(float)) {
            val = (T)(object) ((float)(object)val * Random.Range(0.9f, 1.1f));
        }

        factor = new GeneFactor<T>(highRange, lowRange, mutationRate, val, (DNA) dnaSelection);
        return true;

    }

    void GenerateFactorsFromGenetics(Genetics[] genes, int length, float influence) {
        // Check if factors have already been created, if so then alter genes rather than generate new ones

        if (athleticism == null) athleticism = new GeneFactor<float>(0,0,0f, 0f, DNA.POSITIVE);

        for (int i = 0; i < length; i++) {
            // Determine mutation rate higher and lower
            
        }
    }

    Dictionary<GENOME, DnaConstants> CreateGenomeValueLookup() {
        // Create constant default values for the genomes for each DNA a single time
        var temp = new Dictionary<GENOME, DnaConstants>((int)GENOME.NUM_GENOMES);

        temp.Add(GENOME.ATHLETICISM, new DnaConstants(new float[]   { 0.5f, 0.8f, 1f, 1.18f, 1.4f }, 0.1f, 1, 1));
        temp.Add(GENOME.OFFENSE, new DnaConstants(new float[]       { 5f, 10f, 15f, 23f, 30f }, 0.3f, 2, 2));
        temp.Add(GENOME.DEFENSE, new DnaConstants(new float[]       { 7f, 10f, 13f, 16f, 20f }, 0.3f, 2, 2));
        temp.Add(GENOME.SIZE, new DnaConstants(new float[]          { 0.1f, 0.3f, 0.5f, 0.8f, 1.2f }, 0.2f, 3, 3));
        temp.Add(GENOME.AGRESSION, new DnaConstants(new float[]     { 0.05f, 0.2f, 0.4f, 0.7f, 0.9f }, 0.5f, 4, 4));

        return temp;
    }
}

public class GeneFactor<T>
{

    public readonly T value;
    public readonly DNA dna;

    public int mutationRange_higher;
    public int mutationRange_lower;
    public float mutationRate;

    public float experience; // Affects the +-chance of mutation for the next generation as well as mutation range

    public GeneFactor(int mutationHigher, int mutationLower, float baseMutationRate, T value, DNA dna)
    {
        this.value = value;
        this.dna = dna;
        mutationRange_higher = mutationHigher;
        mutationRange_lower = mutationLower;
        mutationRate = baseMutationRate;

        experience = 0f;
    }

}

readonly public struct DnaConstants
{
    public object dnaValuesArray { get; } // An array of any type i.e. 
    public float mutationRateBase {get; } // Volatility of the gene (Frequency of change between generations)
    public int mutationRange_higher {get;} // How high the gene can mutate to a better version
    public int mutationRange_lower {get;} // How low the gene can mutate to a worse version

    public DnaConstants(object arrayConstants, float mutationRate, int mutationRangeHigh, int mutationRangeLow) {
        dnaValuesArray = arrayConstants;
        mutationRateBase = mutationRate;
        mutationRange_higher = mutationRangeHigh;
        mutationRange_lower = mutationRangeLow;
    }

} 