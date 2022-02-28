using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// List of Fuzzy Logic definitions for various attributes and actions
/// </summary>
public class FuzzyDefinitions : MonoBehaviour
{
    public AnimationCurve health_Healthy;
    public AnimationCurve health_Hurt;
    public AnimationCurve health_Critical;

    void Start()
    {

        Debug.Log(TestAccuracy(health_Hurt));

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

        foreach(Vector2 key in keyPositions)
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
            a = Mathf.Sin(inputs[i] + Mathf.PI / 2f);
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
}
