using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Tutorial for progress bars https://www.youtube.com/watch?v=J1ng1zA3-Pk
[ExecuteAlways]
public class ProgressBar : MonoBehaviour
{
    public int maximum;
    public int current;
    public Image mask;
    public Color color;

    // Start is called before the first frame update
    void Start()
    {
        if (maximum <= 0) maximum = 100;
    }

    // Update is called once per frame
    void Update()
    {
        GetCurrentFill();
    }

    void GetCurrentFill() 
    {
        float fillAmount = current / (float) maximum;
        mask.fillAmount = fillAmount;

        mask.color = color;
    }

    public void UpdateCurrent(int value)
    {
        float newCurrent = Mathf.Clamp(value, 0f, maximum);
        current = (int) newCurrent;
    }
}
