using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
public class CanvasResolutionScalar : MonoBehaviour
{
    CanvasScaler canvasScalar;

    // Start is called before the first frame update
    void Start()
    {
        canvasScalar = gameObject.GetComponent<CanvasScaler>();
        // Scale the canvas based on resolution (Only for 16:9)

        int width = 1280;
        int windowWidth = Screen.width;

        float scaleWidth = (float) windowWidth / width;

        canvasScalar.scaleFactor = scaleWidth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
