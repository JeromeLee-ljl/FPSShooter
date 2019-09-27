using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private void Awake()
    {
    }

    // Start is called before the first frame update
    void Start()
    {
//        PageManager.Instance.Open<MainPage>();
    }

    //todo test
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!GameProcessManager.Instance.Pausing)
                PageManager.Instance.Open<PausePage>(false, 10);
            else
                PageManager.Instance.Close<PausePage>();
        }
    }
}