using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// https://stackoverflow.com/questions/34117591/c-sharp-with-unity-3d-how-do-i-make-a-camera-move-around-an-object-when-user-mo/48997101#48997101
public class CameraController : MonoBehaviour
{
    public Transform cameraOrbit;
    public Transform target;

    // Start is called before the first frame update
    void Start()
    {
        cameraOrbit.position = target.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, 0);

        transform.LookAt(target.position);
    }
}
