using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used to help pass evaluation data to methods
/// </summary>
public struct CurvePair
{
    public AnimationCurve curve;
    public float value;

    public CurvePair(AnimationCurve curve, float value) {
        this.curve = curve;
        this.value = value;
    }
}

/// <summary>
/// Static function library to operate and evaluate animation curves for fuzzy logic
/// </summary>
public static class Fuzzy
{

    public static float EvaluateCurve(in CurvePair pair) {
        return pair.curve.Evaluate(pair.value);
    }

    public static float[] EvaluateCurve(in CurvePair pair1, in CurvePair pair2) {
        return new float[] { pair1.curve.Evaluate(pair1.value), pair2.curve.Evaluate(pair2.value) };
    }

    // The minimum result taken from the evaluation of both curves
    public static float AND(in CurvePair a, in CurvePair b) {
        return Mathf.Min(a.curve.Evaluate(a.value), b.curve.Evaluate(b.value));
    }

    public static float AND(in float a, in CurvePair b) {
        return Mathf.Min(a, b.curve.Evaluate(b.value));
    }

    // The maximum result taken from the evaluation of both curves
    public static float OR(in CurvePair a, in CurvePair b) {
        return Mathf.Max(a.curve.Evaluate(a.value), b.curve.Evaluate(b.value));
    }

    public static float OR(in float a, in CurvePair b) {
        return Mathf.Max(a, b.curve.Evaluate(b.value));
    }

    // The inverted result of the curve evaluation
    public static float NOT(in CurvePair a) {
        return 1f - a.curve.Evaluate(a.value);
    }

    public static float N_AND(in CurvePair a, in CurvePair b) {
        return 1f - Mathf.Min(a.curve.Evaluate(a.value), b.curve.Evaluate(b.value));
    }

    public static float N_OR(in CurvePair a, in CurvePair b) {
        return 1f - Mathf.Max(a.curve.Evaluate(a.value), b.curve.Evaluate(b.value));
    }

    // returns the greater of the two (defaults to a if they are the same)
    public static CurvePair GREATER(in CurvePair a, in CurvePair b) {
        float aEval = a.curve.Evaluate(a.value);
        float bEval = b.curve.Evaluate(b.value);
        return aEval >= bEval ? a : b;
    }

    // returns the lesser of the two (defaults to a if they are the same)
    public static CurvePair LESSER(in CurvePair a, in CurvePair b) {
        float aEval = a.curve.Evaluate(a.value);
        float bEval = b.curve.Evaluate(b.value);
        return aEval <= bEval ? a : b;
    }

    // XOR requires reconstruction of animation curves to create. Future work when needed
    public static float XOR() { 
        return 0f; 
    }

    // Remaps the value given a range of from1 -> to1, mapped to From2 -> To2
    // code taken from Fuzzy Logic Lecture 7
    public static float Remap(float value, float from1, float To1, float From2, float To2) {
        return (value - from1) / (To1 - from1) * (To2 - From2) + From2;
    }

    public static bool Roll(float chance) {
        float test = Mathf.Clamp01(chance);

        return Random.value <= chance;
    }
}
