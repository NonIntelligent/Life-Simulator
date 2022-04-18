using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Chonometer : MonoBehaviour
{
    public ProgressBar progress;
    public Text dayCount;
    public Text generationCount;
    public static float secondsInDay {get; private set;}
    public static int daysInGeneration {get; private set;}

    public float secondsPerDay = 60f;
    public int daysPerGeneration = 5;

    float currentTime = 0f;
    long day = 1;
    long generation = 1;

    // Start is called before the first frame update
    void Start()
    {
        if (secondsPerDay <= 0f) secondsPerDay = 60f;
        if (daysPerGeneration <= 0) daysPerGeneration = 5;

        secondsInDay = secondsPerDay;
        daysInGeneration = daysPerGeneration;

        secondsPerDay = Mathf.Clamp(secondsPerDay, 10f, 120f);
        updateDayText(day);
        updateGenerationText(generation);
    }

    // Update is called once per frame
    void Update()
    {
        currentTime += Time.deltaTime;

        progress.UpdateCurrent(currentTime / secondsPerDay);

        CheckNextDay();
    }

    void CheckNextDay()
    {
        if (currentTime < secondsPerDay) return;

        currentTime = 0f;
        day++;
        updateDayText(day);

        CheckNextGeneration();
    }

    void CheckNextGeneration()
    {
        if (day % daysPerGeneration != 0) return;

        generation++;
        updateGenerationText(generation);
    }

    void updateDayText(long value)
    {
        dayCount.text = "Day: " + value;
    }

    void updateGenerationText(long value)
    {
        generationCount.text = "Generation: " + value;
    }
}
