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
    static IReadOnlyDictionary<GENOME, GenomeConstant> genome_Values = null;

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
        athleticism = GenerateRandomFactor<float>(GENOME.ATHLETICISM);
        offense = GenerateRandomFactor<float>(GENOME.OFFENSE);
        defense = GenerateRandomFactor<float>(GENOME.DEFENSE);
        size = GenerateRandomFactor<float>(GENOME.SIZE);

        agression = GenerateRandomFactor<float>(GENOME.AGRESSION);
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
            GenerateFactorFromGenetics(parents, 2, 0.8f, athleticism, GENOME.ATHLETICISM);
            GenerateFactorFromGenetics(parents, 2, 0.8f, offense, GENOME.OFFENSE);
            GenerateFactorFromGenetics(parents, 2, 0.8f, defense, GENOME.DEFENSE);
            GenerateFactorFromGenetics(parents, 2, 0.8f, size, GENOME.SIZE);

            GenerateFactorFromGenetics(parents, 2, 0.8f, agression, GENOME.AGRESSION);
        }

        // Use grandparents' genes to determine mutation 
        if (grandParents != null) {
            GenerateFactorFromGenetics(grandParents, 4, 0.2f, athleticism, GENOME.ATHLETICISM);
            GenerateFactorFromGenetics(grandParents, 4, 0.2f, offense, GENOME.OFFENSE);
            GenerateFactorFromGenetics(grandParents, 4, 0.2f, defense, GENOME.DEFENSE);
            GenerateFactorFromGenetics(grandParents, 4, 0.2f, size, GENOME.SIZE);

            GenerateFactorFromGenetics(grandParents, 4, 0.2f, agression, GENOME.AGRESSION);
        }
        
    }

    GeneFactor<T> GenerateRandomFactor<T>(GENOME genome)
    {
        GenomeConstant genomeConstant = genome_Values[genome];
        
        int dnaSelection = UnityEngine.Random.Range(0, (int)DNA.NUM_DNA - 1);

        T[] values = (T[]) genomeConstant.dnaValuesArray;
        T val = values[dnaSelection];

        // Add variance to floating point values by +-10% 
        if (val is float) {
            val = (T)(object) ((float)(object)val * Random.Range(0.9f, 1.1f));
        }

        return new GeneFactor<T>(val, (DNA) dnaSelection);

    }

    void GenerateFactorFromGenetics<T>(Genetics[] genes, int length, float influence, GeneFactor<T> factor, GENOME genome) {
        // Check if factors have already been created, if so then alter genes rather than generate new ones
        GenomeConstant genomeConstant = genome_Values[genome];
        T[] dnaValues = (T[]) genomeConstant.dnaValuesArray;

        if (factor == null) factor = new GeneFactor<T>(dnaValues[(int) DNA.NEUTRAL], DNA.NEUTRAL);

        int[] randomRangeList = new int[4];

        for (int i = 0; i < length; i+=2) {
            // Determine mutation rate higher and lower

            DNA parent1Dna = genes[0].athleticism.dna;
            DNA parent2Dna = genes[1].athleticism.dna;
            DNA myDna = factor.dna;
            float avg = ((float)parent1Dna + (float)parent2Dna) / 2f;
        }
    }

    void GenerateAllFactorsFromGenetics(Genetics[] genes, int length, float influence) {
        // Check if factors have already been created, if so then alter genes rather than generate new ones
        GenomeConstant genomeConstant = genome_Values[GENOME.ATHLETICISM];
        float[] dnaValues = (float[])genomeConstant.dnaValuesArray;

        if (athleticism == null) athleticism = new GeneFactor<float>(dnaValues[(int)DNA.NEUTRAL], DNA.NEUTRAL);

        int[] randomRangeList = new int[100];
        int itemsToFillFromParents = (int) (influence * randomRangeList.Length);

        DNA parent1Dna = genes[0].athleticism.dna;
        DNA parent2Dna = genes[1].athleticism.dna;
        DNA myDna = athleticism.dna;

        // Determine starting gene by selecting randomly from this weighted list
        for (int i = 0; i < itemsToFillFromParents; i+=2)
        {
            randomRangeList[i] = (int)parent1Dna;
            randomRangeList[i + 1] = (int)parent2Dna;
        }

        for (int i = itemsToFillFromParents; i < randomRangeList.Length; i++)
        {
            randomRangeList[i] = (int)myDna;
        }

        // Choose gene from list randomly
        DNA randomDna = (DNA) Random.Range(0, randomRangeList.Length);

        for (int i = 0; i < length; i += 2)
        {

            // Determine mutation rate higher and lower

        }

    }

    void GenerateAllFactorsFromAllGenetics(Genetics[] parents, float influence_1st, Genetics[] grandParents, float influence_2nd) {
        // Check if factors have already been created, if so then alter genes rather than generate new ones
        GenomeConstant genomeConstant = genome_Values[GENOME.ATHLETICISM];
        float[] dnaValues = (float[])genomeConstant.dnaValuesArray;

        if (athleticism == null) athleticism = new GeneFactor<float>(dnaValues[(int)DNA.NEUTRAL], DNA.NEUTRAL);

        int[] randomRangeList = new int[300];
        int itemsToFillFromParents = (int)(influence_1st * randomRangeList.Length);
        int itemsToFillFromGrandParents = (int)(influence_2nd * randomRangeList.Length);

        DNA parent1Dna = parents[0].athleticism.dna;
        DNA parent2Dna = parents[1].athleticism.dna;

        DNA grandParent1Dna = grandParents[0].athleticism.dna;
        DNA grandParent2Dna = grandParents[1].athleticism.dna;
        DNA grandParent3Dna = grandParents[2].athleticism.dna;
        DNA grandParent4Dna = grandParents[3].athleticism.dna;
        DNA myDna = athleticism.dna;

        // Creates a weighted array based on the influence values. e.g. an influence value of 0.8 will fill 80% of the array with the parent's dna
        // If influence is >= 1.0 then the genetics of the grandparents and self are not considered.
        for (int i = 0; i < itemsToFillFromParents; i += 2)
        {
            randomRangeList[i] = (int)parent1Dna;
            randomRangeList[i + 1] = (int)parent2Dna;
        }

        int fillCap = itemsToFillFromParents + itemsToFillFromGrandParents < randomRangeList.Length - 4 ? itemsToFillFromParents + itemsToFillFromGrandParents : 0;

        for (int i = itemsToFillFromParents; i < itemsToFillFromParents + itemsToFillFromGrandParents; i += 4)
        {
            randomRangeList[i] = (int)grandParent1Dna;
            randomRangeList[i + 1] = (int)grandParent2Dna;
            randomRangeList[i + 2] = (int)grandParent3Dna;
            randomRangeList[i + 3] = (int)grandParent4Dna;
        }

        for (int i = itemsToFillFromParents + itemsToFillFromGrandParents; i < randomRangeList.Length; i++)
        {
            randomRangeList[i] = (int)myDna;
        }

        // Choose gene from list randomly
        DNA randomDna = (DNA)Random.Range(0, randomRangeList.Length);

        for (int i = 0; i < 5; i += 2)
        {

            // Determine mutation rate higher and lower

        }
    }

    Dictionary<GENOME, GenomeConstant> CreateGenomeValueLookup() {
        // Create constant default values for the genomes for each DNA a single time
        var temp = new Dictionary<GENOME, GenomeConstant>((int)GENOME.NUM_GENOMES);

        temp[GENOME.ATHLETICISM] = new GenomeConstant(new float[]   { 0.5f, 0.8f, 1f, 1.18f, 1.4f }, 0.1f, 1, 1);
        temp[GENOME.OFFENSE] = new GenomeConstant(new float[]       { 5f, 10f, 15f, 23f, 30f }, 0.3f, 2, 2);
        temp[GENOME.DEFENSE] = new GenomeConstant(new float[]       { 7f, 10f, 13f, 16f, 20f }, 0.3f, 2, 2);
        temp[GENOME.SIZE] = new GenomeConstant(new float[]          { 0.1f, 0.3f, 0.5f, 0.8f, 1.2f }, 0.2f, 3, 3);
        temp[GENOME.AGRESSION] = new GenomeConstant(new float[]     { 0.05f, 0.2f, 0.4f, 0.7f, 0.9f }, 0.5f, 4, 4);

        return temp;
    }
}

public class GeneFactor<T>
{

    public readonly T value;
    public readonly DNA dna;

    public float experience; // Affects the +-chance of mutation for the next generation as well as mutation range

    public GeneFactor(T value, DNA dna)
    {
        this.value = value;
        this.dna = dna;

        experience = 0f;
    }

}

readonly public struct GenomeConstant
{
    public object dnaValuesArray { get; } // An array of any type i.e. float[] (cast object -> T[])
    public float mutationRateBase {get; } // Volatility of the gene (Frequency of change between generations)
    public int mutationRange_higher {get;} // How high the gene can mutate to a better version
    public int mutationRange_lower {get;} // How low the gene can mutate to a worse version

    public GenomeConstant(object arrayConstants, float mutationRate, int mutationRangeHigh, int mutationRangeLow) {
        dnaValuesArray = arrayConstants;
        mutationRateBase = mutationRate;
        mutationRange_higher = mutationRangeHigh;
        mutationRange_lower = mutationRangeLow;
    }

} 