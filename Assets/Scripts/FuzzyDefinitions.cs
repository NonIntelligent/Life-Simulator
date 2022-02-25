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
        for(int i = 0; i < health_Hurt.length; i++)
        {
            health_Hurt.RemoveKey(i);
        }

        Vector2[] keys = new Vector2[] { new Vector2(0.5f, 0f), new Vector2(1.0f, 0.25f), new Vector2(1.207f, 0.5f),
                                         new Vector2(1.366f, 0.75f), new Vector2(0f, 0.25f), new Vector2(-0.207f, 0.5f),
                                         new Vector2(-0.366f, 0.75f), new Vector2(1.5f, 1f), new Vector2(-0.5f, 1f) };

        CurveBuilder(health_Hurt, keys);

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
}
