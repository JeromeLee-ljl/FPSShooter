using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFaceToMainCamera : MonoBehaviour
{
    private Transform mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main.transform;
    }

    private void LateUpdate()
    {
        transform.LookAt(mainCamera);
    }
}