using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Chonometer : MonoBehaviour
{
    public ProgressBar progress;
    public float secondsPerDay = 60f;

    float currentTime = 0f;

    // Start is called before the first frame update
    void Start()
    {
        secondsPerDay = Mathf.Clamp(secondsPerDay, 10f, 120f);
    }

    // Update is called once per frame
    void Update()
    {
        currentTime += Time.deltaTime;

        progress.UpdateCurrent((int) (secondsPerDay / currentTime));

        if (currentTime >= secondsPerDay) currentTime = 0f;
    }
}
