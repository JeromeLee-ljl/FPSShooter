using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Compression;
using UnityEngine;
using UnityEngine.UI;

public class WeaponIcon : MonoBehaviour
{
    public float hideSize = 0.8f;
    public Weapon weapon;

    private void Start()
    {
        GetComponent<Image>().sprite = weapon.icon;
    }

    void FixedUpdate()
    {
        float size = weapon.gameObject.activeSelf ? 1 : hideSize;
        transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(size, size, 1), 0.1f);
    }
}