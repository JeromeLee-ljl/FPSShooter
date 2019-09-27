using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MuzzleFlash : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;

    public Sprite[] muzzleFlashes;
    public float showTime = 0.1f;
    private Transform muzzlePos;

    public void SetMuzzlePos(Transform pos)
    {
        muzzlePos = pos;
        FollowMuzzlePos();
    }

    private void OnEnable()
    {
        RandomFlash();
        StartCoroutine(Recycle());
    }

    private void Update()
    {
        FollowMuzzlePos();
    }

    void FollowMuzzlePos()
    {
        transform.position = muzzlePos.position;
        transform.rotation = muzzlePos.rotation;
    }

    void RandomFlash()
    {
        int i = Random.Range(0, muzzleFlashes.Length);
        spriteRenderer.sprite = muzzleFlashes[i];
    }

    IEnumerator Recycle()
    {
        yield return new WaitForSeconds(showTime);
        PoolsManager.Instance.Recycle(gameObject);
    }
}