using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    public Transform target;
    [Range(0, 1)] public float smooth = 1;
    private Vector3 direciton;

    private void Awake()
    {
        direciton = target.position - transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 targetPos = target.position - direciton;
        transform.position = Vector3.Lerp(transform.position, targetPos, smooth);
    }
}