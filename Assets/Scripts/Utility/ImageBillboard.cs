using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageBillboard : MonoBehaviour
{
    Camera cam;

    void Awake()
    {
        cam = Camera.main;
    }
    void LateUpdate()
    {
        //transform.forward = cam.transform.forward;
        transform.LookAt(cam.transform.position);
    }
}
