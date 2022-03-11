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
    
    float _size;
}

public class GeneFactor
{
    string name;
    Pair<float, GENES> size;
}

public class Pair<T, U>
{
    readonly T first;
    readonly U second;

    Pair(T item1, U item2)
    {
        first = item1;
        second = item2;
    }

    public T getFirst() { return first; }
    public U getSecond() { return second; }
}

public enum GENES
{
    GREATER,
    POSITIVE,
    NEUTRAL,
    NEGATIVE,
    DISORDER
};
