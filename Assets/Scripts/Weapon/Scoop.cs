using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scoop : MonoBehaviour
{
    public Camera scoopCamera;

    public float scale = 2;
    [Range(0, 60)] public float oneScaleFieldOfView = 30;
    private Transform mainCamera;
    private Vector3 oldScoopCameraPos;

    private void Awake()
    {
        mainCamera = Camera.main.transform;
        oldScoopCameraPos = scoopCamera.transform.localPosition;
        SetByScale();
    }

    private void Update()
    {
        scoopCamera.transform.rotation = mainCamera.rotation;
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        Vector3 delta = new Vector3(mouseX, mouseY, 0);
        scoopCamera.transform.localPosition =
            Vector3.Lerp(scoopCamera.transform.localPosition, oldScoopCameraPos + delta * 0.005f, 0.2f);
    }

    private void SetByScale()
    {
        scoopCamera.fieldOfView = oneScaleFieldOfView / scale;
    }
}