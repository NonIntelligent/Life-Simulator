using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq; // Used to cast enum into a list

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

    ArrayList geneFactors = new ArrayList((int) GENOME.NUM_GENOMES);

    // Randomly generate all genes
    public Genetics()
    {
        
        // Create constant default values for the genomes for each DNA a single time
        if(genome_Values == null)
        {
            genome_Values = CreateGenomeValueLookup();
        }

        // get value assigned to genome from lookup table
        InitialiseAllGeneFactors();
    }

    // Generate genes based on parents (limits mutation chance and mutation range)
    public Genetics(Genetics[] parents, Genetics[] grandParents)
    {
        InitialiseAllGeneFactors();

        // Create constant default values for the genomes for each DNA a single time
        if (genome_Values == null) {
            genome_Values = CreateGenomeValueLookup();
        }

        // Invalid use of constructor, act as if default constructor was used
        if (parents == null && grandParents == null) return;

        // Use parents genes to determine mutation rate, the range of mutation if successfull and the starting gene
        if (grandParents == null) {
            GenerateFactorFromParents<float>(parents, 0.9f, GENOME.ATHLETICISM);
            GenerateFactorFromParents<float>(parents, 0.9f, GENOME.OFFENSE);
            GenerateFactorFromParents<float>(parents, 0.9f, GENOME.DEFENSE);
            GenerateFactorFromParents<float>(parents, 0.9f, GENOME.SIZE);

            GenerateFactorFromParents<float>(parents, 0.9f, GENOME.AGRESSION);

            return;
        }

        // Use both parents and granparents genes to determine genetics
        GenerateFactorFromAllGenetics<float>(parents, 0.7f, grandParents, 0.2f, GENOME.ATHLETICISM);
        GenerateFactorFromAllGenetics<float>(parents, 0.7f, grandParents, 0.2f, GENOME.OFFENSE);
        GenerateFactorFromAllGenetics<float>(parents, 0.7f, grandParents, 0.2f, GENOME.DEFENSE);
        GenerateFactorFromAllGenetics<float>(parents, 0.7f, grandParents, 0.2f, GENOME.SIZE);

        GenerateFactorFromAllGenetics<float>(parents, 0.7f, grandParents, 0.2f, GENOME.AGRESSION);
        
    }

    GeneFactor<T> GenerateRandomFactor<T>(GENOME genome)
    {
        GenomeConstant genomeConstant = genome_Values[genome];
        
        int dnaSelection = Random.Range(0, (int)DNA.NUM_DNA - 1);

        T[] values = (T[]) genomeConstant.dnaValuesArray;
        T val = values[dnaSelection];

        // Add variance to floating point values by +-10% 
        if (val is float) {
            val = (T)(object) ((float)(object)val * Random.Range(0.9f, 1.1f));
        }

        return new GeneFactor<T>(val, (DNA) dnaSelection);

    }

    void GenerateFactorFromParents<T>(Genetics[] parents, float influence_1st, GENOME genome)
    {
        int genomeIndex = (int)genome;
        int[] randomRangeList = new int[100];
        DNA[] dnaList = new DNA[2];
        DNA myDna = ((GeneFactor<T>)geneFactors[genomeIndex]).dna;
    }

    void GenerateFactorFromAllGenetics<T>(Genetics[] parents, float influence_1st, Genetics[] grandParents, float influence_2nd, GENOME genome)
    {
        int genomeIndex = (int)genome;
        int[] randomRangeList = new int[300];
        DNA[] dnaList = new DNA[6];
        GeneFactor<T>[] parentFactors = new GeneFactor<T>[6];
        DNA myDna = ((GeneFactor<T>)geneFactors[genomeIndex]).dna;


        foreach (GeneFactor<T> factor in geneFactors)
        {
            parentFactors[0] = (GeneFactor<T>)parents[0].geneFactors[genomeIndex];
            parentFactors[1] = (GeneFactor<T>)parents[1].geneFactors[genomeIndex];
            parentFactors[2] = (GeneFactor<T>)grandParents[0].geneFactors[genomeIndex];
            parentFactors[3] = (GeneFactor<T>)grandParents[1].geneFactors[genomeIndex];
            parentFactors[4] = (GeneFactor<T>)grandParents[2].geneFactors[genomeIndex];
            parentFactors[5] = (GeneFactor<T>)grandParents[3].geneFactors[genomeIndex];

            dnaList[0] = parentFactors[0].dna;
            dnaList[1] = parentFactors[1].dna;
            dnaList[3] = parentFactors[2].dna;
            dnaList[2] = parentFactors[3].dna;
            dnaList[4] = parentFactors[4].dna;
            dnaList[5] = parentFactors[5].dna;

            // Creates a weighted array based on the influence values. e.g. an influence value of 0.8 will fill 80% of the array with the parent's dna
            // If influence is >= 1.0 then the genetics of the grandparents and self are not considered.
            FillDnaArray(randomRangeList, dnaList, influence_1st, influence_2nd, myDna);

            // Choose gene from list randomly
            DNA randomDna = (DNA)Random.Range(0, randomRangeList.Length);

            // Adjust mutation ranges based on experience
            int mutationHigh = genome_Values[genome].mutationRange_higher;
            int mutationLow = genome_Values[genome].mutationRange_lower;
            float mutationChance = genome_Values[genome].mutationRateBase;

            // Sum experience from parents and is modified by influence
            float experienceSum = (parentFactors[0].experience + parentFactors[1].experience) * influence_1st + (parentFactors[2].experience
                                  + parentFactors[3].experience + parentFactors[4].experience + parentFactors[5].experience) * influence_2nd;

            // Every 100 exp the high range is increased and every 200, the low range is decreased.
            // The chance of mutation increases with abnormally high experience. It may decrease depending the number of the exact same gene in the dnaList.
            mutationHigh += (int)(experienceSum / 100f);
            mutationLow -= (int)(experienceSum / 200f);
            mutationChance += (experienceSum / 1000f) * 0.1f; // 10% per 1000 exp

            // Alters mutation chance by +- 10%
            int dnaCount = randomRangeList.Count(d => d == (int)randomDna);
            mutationChance += (((float) dnaCount / randomRangeList.Length) - 0.5f) * 0.2f;

            // Roll the dice to see if gene mutates as new final output
            
        }
    }

    void FillDnaArray(int[] output, DNA[] dnaList, float influenceA, DNA myDna) {
        int itemsToFillFromParents = (int)(influenceA * output.Length);

        // Creates a weighted array based on the influence values. e.g. an influence value of 0.8 will fill 80% of the array with the parent's dna
        // If influence is >= 1.0 then the genetics of the self are not considered.
        for (int i = 0; i < itemsToFillFromParents; i += 2)
        {
            output[i] = (int)dnaList[0];
            output[i + 1] = (int)dnaList[1];
        }

        for (int i = itemsToFillFromParents; i < output.Length; i++)
        {
            output[i] = (int)myDna;
        }
    }

    void FillDnaArray(int[] output, DNA[] dnaList, float influenceA, float influenceB, DNA myDna) {
        int itemsToFillFromParents = (int)(influenceA * output.Length);
        int itemsToFillFromGrandParents = (int)(influenceB * output.Length);
        int totalItemsToFill = itemsToFillFromParents + itemsToFillFromGrandParents;

        int fillCap = totalItemsToFill < output.Length - 4 ? totalItemsToFill : output.Length;

        // Creates a weighted array based on the influence values. e.g. an influence value of 0.8 will fill 80% of the array with the parent's dna
        // If influence is >= 1.0 then the genetics of the grandparents and self are not considered.
        for (int i = 0; i < itemsToFillFromParents; i += 2)
        {
            output[i] = (int)dnaList[0];
            output[i + 1] = (int)dnaList[1];
        }
        
        for (int i = itemsToFillFromParents; i < fillCap; i += 4)
        {
            output[i] = (int)dnaList[2];
            output[i + 1] = (int)dnaList[3];
            output[i + 2] = (int)dnaList[4];
            output[i + 3] = (int)dnaList[5];
        }

        for (int i = fillCap; i < output.Length; i++)
        {
            output[i] = (int)myDna;
        }

    }

    DNA MutateGene(GenomeConstant constant, float experienceSum, int[] randomDnaRange, DNA randomChosen)
    {
        DNA result = randomChosen;

        // Adjust mutation ranges based on experience
        int mutationHigh = constant.mutationRange_higher;
        int mutationLow = constant.mutationRange_lower;
        float mutationChance = constant.mutationRateBase;

        // Every 100 exp the high range is increased and every 200, the low range is decreased.
        // The chance of mutation increases with abnormally high experience. It may decrease depending the number of the exact same gene in the dnaList.
        mutationHigh += (int)(experienceSum / 100f);
        mutationLow -= (int)(experienceSum / 200f);
        mutationChance += (experienceSum / 1000f) * 0.1f; // 10% per 1000 exp

        // Alters mutation chance by +- 10%
        int dnaCount = randomDnaRange.Count(d => d == (int)result);
        mutationChance += (((float)dnaCount / randomDnaRange.Length) - 0.5f) * 0.2f;
        mutationChance = Mathf.Clamp(mutationChance, 0f, 1f);

        // Roll the dice to see if gene mutates as new final output
        if(mutationChance >= Random.value)
        {
            // Adjust highs and lows to be appropriate and within bounds of the DNA Enum values
            mutationHigh = System.Math.Max(0, (int)result - mutationHigh);
            mutationLow = System.Math.Min((int)DNA.NUM_DNA - 1, (int)result + mutationLow);

            result = (DNA)Random.Range(mutationHigh, mutationLow);

        }

        return result;
    }

    void InitialiseAllGeneFactors()
    {
        geneFactors[(int)GENOME.ATHLETICISM] = GenerateRandomFactor<float>(GENOME.ATHLETICISM);
        geneFactors[(int)GENOME.OFFENSE] = GenerateRandomFactor<float>(GENOME.OFFENSE);
        geneFactors[(int)GENOME.DEFENSE] = GenerateRandomFactor<float>(GENOME.DEFENSE);
        geneFactors[(int)GENOME.SIZE] = GenerateRandomFactor<float>(GENOME.SIZE);

        geneFactors[(int)GENOME.AGRESSION] = GenerateRandomFactor<float>(GENOME.AGRESSION);

        geneFactors.TrimToSize();
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
    public float mutationRateBase { get; } // Volatility of the gene (Frequency of change between generations)
    public int mutationRange_higher { get;} // How high the gene can mutate to a better version
    public int mutationRange_lower { get;} // How low the gene can mutate to a worse version

    public GenomeConstant(object arrayConstants, float mutationRate, int mutationRangeHigh, int mutationRangeLow) {
        dnaValuesArray = arrayConstants;
        mutationRateBase = mutationRate;
        mutationRange_higher = mutationRangeHigh;
        mutationRange_lower = mutationRangeLow;
    }

} 