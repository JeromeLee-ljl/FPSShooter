using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletHole : MonoBehaviour
{
    public float showTime;

    private void OnEnable()
    {
        StartCoroutine(DelayRecycle());
    }
    
    IEnumerator DelayRecycle()
    {
        yield return new WaitForSeconds(showTime);
        PoolsManager.Instance.Recycle(gameObject);
    }
}