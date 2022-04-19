using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// List of Fuzzy Logic definitions for various attributes and actions
/// </summary>
public class FuzzyDefinitions : MonoBehaviour
{
    readonly Attributes attributes;

    // Attribute curves
    public AnimationCurve health_Healthy;
    public AnimationCurve health_Hurt;
    public AnimationCurve health_Critical;

    public AnimationCurve hunger_satiated;
    public AnimationCurve hunger_starving;

    public AnimationCurve energy_priority;

    // Behaviour curves
    public AnimationCurve search_food;
    public AnimationCurve attack_creature;
    public AnimationCurve run_away;

    void Start()
    {
        // Future work.
        // Alter curves based on attributes and traits
    }

    void Update()
    {

    }

    public void CurveBuilder(AnimationCurve curve, Vector2[] keyPositions)
    {
        for (int i = 0; i < curve.length; i++)
        {
            curve.RemoveKey(i);
        }

        foreach (Vector2 key in keyPositions)
        {
            curve.AddKey(key.x, key.y);
        }

    }

    public double TestAccuracy(AnimationCurve curve)
    {
        List<float> inputs = new List<float>();
        List<double> accuracy = new List<double>();

        for (int i = 0; i < 10000; i++)
        {
            inputs.Add(Random.Range(-1000f, 1000f));
        }

        float a = 0.0f;
        float b = 0.0f;

        for (int i = 0; i < 10000; i++)
        {
            a = Mathf.Cos(inputs[i]);
            b = curve.Evaluate(inputs[i]);

            accuracy.Add(Mathf.Abs((b - a)) / a);
        }

        double sum = 0;

        foreach (double num in accuracy)
        {
            sum += num;
        }

        sum /= accuracy.Count;

        return (1 - sum) * 100.0;
    }

    public float[] EvaluateHealth(float health)
    {
        return new float[] { health_Healthy.Evaluate(health), health_Hurt.Evaluate(health), health_Critical.Evaluate(health) };
    }

}
