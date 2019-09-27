using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerRotation : MonoBehaviour
{
    // 控制head的x轴旋转  
    public Transform headRotateX;
    public float mouseSensitivity = 5;
    [Range(0, 89)] public float upRange = 60;
    [Range(0, 89)] public float downRange = 45;
    private PlayerWeaponController playerWeaponController;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        playerWeaponController = GetComponent<PlayerWeaponController>();
    }

    void Update()
    {
        GetInput();
        ShowOrHideCursor();
    }

    private void FixedUpdate()
    {
        if (Cursor.lockState != CursorLockMode.Locked) return;
        RotateFollowMouse();
        RotateBody();
    }

    private float mouseX, mouseY;

    void GetInput()
    {
        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");
    }

    void RotateFollowMouse()
    {
        float sensitivity = mouseSensitivity;
        if (playerWeaponController.aiming)
        {
            sensitivity /= 2; // 瞄准时变慢}
            if (playerWeaponController.isAuto)
                sensitivity /= 2; // 有镜
        }

        transform.Rotate(sensitivity * mouseX * Vector3.up);

        float xAngles = headRotateX.localEulerAngles.x;
        if (xAngles > 180 && xAngles < 360 - upRange && mouseY > 0 ||
            xAngles < 180 && xAngles > downRange && mouseY < 0) return;
        headRotateX.Rotate(sensitivity * mouseY * Vector3.left);
    }

    private void ShowOrHideCursor()
    {
        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            if (Cursor.lockState == CursorLockMode.Locked)
                Cursor.lockState = CursorLockMode.None;
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }

    [Header("Swing body in camera")]
    // 武器将绕着这个点旋转
    public Transform body;

    public float swingSensitivity;

    private void RotateBody()
    {
        Quaternion quaternion = Quaternion.Euler(-mouseY * swingSensitivity, mouseX * swingSensitivity, 0);
        body.localRotation = Quaternion.Lerp(body.localRotation, quaternion, 0.1f);
    }
}