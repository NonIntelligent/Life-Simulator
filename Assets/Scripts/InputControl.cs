using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// https://stackoverflow.com/questions/34117591/c-sharp-with-unity-3d-how-do-i-make-a-camera-move-around-an-object-when-user-mo/48997101#48997101
public class InputControl : MonoBehaviour
{
    public GameObject cameraOrbit;
    public float rotationSpeed = 0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(2))
        {
            float h = rotationSpeed * Input.GetAxis("Mouse X");
            float v = rotationSpeed * Input.GetAxis("Mouse Y");

            // Limit z-rotation
            if (cameraOrbit.transform.eulerAngles.z + v <= 0.1f || cameraOrbit.transform.eulerAngles.z + v >= 85f)
            {
                v = 0;
            }

            cameraOrbit.transform.eulerAngles = new Vector3(cameraOrbit.transform.eulerAngles.x, cameraOrbit.transform.eulerAngles.y + h, cameraOrbit.transform.eulerAngles.z + v);

        }

        float scrollFactor = Input.GetAxis("Mouse ScrollWheel");

        if (scrollFactor != 0)
        {
            cameraOrbit.transform.localScale = cameraOrbit.transform.localScale * (1f - scrollFactor);
        }
    }
}
