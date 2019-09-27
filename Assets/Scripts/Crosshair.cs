using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshair : MonoBehaviour
{
    public PlayerWeaponController playerWeaponController;
    public PlayerMovement playerMovement;
    private Animator crosshair;

    private enum CrosshairState
    {
        In,
        Out,
        Hide,
    }

    private void Awake()
    {
        crosshair = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerWeaponController.isAuto && playerWeaponController.aiming)
        {
            SetCrosshairState(CrosshairState.In);
            return;
        }

        if (playerMovement.isRunning ||
            playerWeaponController.reloading ||
            !playerWeaponController.isGun ||
            !playerWeaponController.isAuto && playerWeaponController.aiming)
        {
            SetCrosshairState(CrosshairState.Hide);
            return;
        }

        SetCrosshairState(CrosshairState.Out);

//        if (playerWeaponController.isGun)
//        {
//            if (playerWeaponController.aiming)
//            {
//                if (playerWeaponController.isAuto)
//                {
//                    SetCrosshairState(CrosshairState.In);
//                }
//                else
//                {
//                    SetCrosshairState(CrosshairState.Hide);
//                }
//            }
//            else if (playerMovement.isRunning)
//            {
//                SetCrosshairState(CrosshairState.Hide);
//            }
//            else
//            {
//                SetCrosshairState(CrosshairState.Out);
//            }
//        }
//        else
//        {
//            SetCrosshairState(CrosshairState.Hide);
//        }
    }

    private CrosshairState currentState;

    private void SetCrosshairState(CrosshairState state)
    {
        if (currentState == state) return;
        currentState = state;
        crosshair.SetTrigger(state.ToString());
    }
}