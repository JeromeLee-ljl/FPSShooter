using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gun : Weapon
{
    public float fireInterval = 0.5f;
    public float reloadTime = 3f;
    public bool auto;
    public Transform gunPoint;
    public float recoilForce = 1;
    public float shakeForce = 1;
    public float damage;
    public float speed;

    public int capacity = 100;

    // 弹夹中子弹数
    public int BulletCountInClip { get; private set; }

    public int allBulletCount = 100;
    [Header("Audio")] public AudioClip reloadClip;
    public AudioClip fireClip;
    private AudioSource gunAudio;

    [HideInInspector] public bool canFire = true;

    private void Awake()
    {
        gunAudio = GetComponent<AudioSource>();
    }

    private void Start()
    {
        LoadConfig();
        BulletCountInClip = capacity;
    }

    private void LoadConfig()
    {
        var config = ConfigsManager.Instance.GetConfig<JsonConfig<WeaponsConfig>>();
        var gunData = auto ? config.Data.autoGun : config.Data.semiGun;
        fireInterval = gunData.fireInterval;
        reloadTime = gunData.reloadTime;
        recoilForce = gunData.recoilForce;
        shakeForce = gunData.shakeForce;
        allBulletCount = gunData.bulletCount;
        capacity = gunData.capacity;
        damage = gunData.damage;
        speed = gunData.speed;
    }

    public override void Attack(Action success)
    {
        if (!canFire) return;
        AudioManager.Instance.Play(gunAudio, fireClip, 0.2f);
        AfterCanFire(fireInterval);
        success();
        Fire();
    }

    // 延迟一段时间后才能开火
    public void AfterCanFire(float time)
    {
        canFire = false;
        StartCoroutine(AfterTimeCanFireCor(time));
    }

    private IEnumerator AfterTimeCanFireCor(float time)
    {
        yield return new WaitForSeconds(time);
        canFire = true;
    }

    private void Fire()
    {
        // 火花
        MuzzleFlash muzzleFlash = PoolsManager.Instance.Get<MuzzleFlash>();
        muzzleFlash.SetMuzzlePos(gunPoint);

        // 发射子弹
        Bullet bullet = PoolsManager.Instance.Get<Bullet>();
        bullet.Set(damage, gunPoint, speed, gameObject.layer);
        BulletCountInClip--;
    }

    public void Reload(Func<bool> breakLoad, Action succeed, Action fail)
    {
        StartCoroutine(TryReload(breakLoad, succeed, fail));
    }

    private IEnumerator TryReload(Func<bool> breakReload, Action succeed, Action fail)
    {
        AudioManager.Instance.Play(gunAudio, reloadClip, 0.2f);
        for (float time = reloadTime; time > 0; time -= Time.deltaTime)
        {
            if (breakReload()) // 装子弹被打断
            {
                gunAudio.Stop();
                fail();
                yield break;
            }

            yield return null;
        }

        Debug.Log("reload bullet succeed");
        ReloadBulletCount();
        succeed();
        yield return null;
    }

    private void ReloadBulletCount()
    {
        int needCount = capacity - BulletCountInClip;
        if (needCount > allBulletCount)
            needCount = allBulletCount;
        BulletCountInClip += needCount;
        allBulletCount -= needCount;
        Debug.Log($"addBullet {needCount}");
    }
}