using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class PlayerWeaponController : MonoBehaviour
{
    [Header("Weapon")] public Weapon[] weapons;
    public AudioSource changeWeaponAudio;
    public Text bulletCountText;
    private static readonly int AnimParamChangeWeapon = Animator.StringToHash("ChangeWeapon");
    private static readonly int AnimParamReload = Animator.StringToHash("Reload");
    private static readonly int AnimParamAttack = Animator.StringToHash("Attack");
    private static readonly int AnimParamAiming = Animator.StringToHash("Aiming");
    private PlayerMovement playerMovement;
    private Animator animator;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        animator = GetComponentInChildren<Animator>();
        oldFieldOfView = mCamera.fieldOfView;
        HideOtherWeapon();
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();

        CheckAimState();

        CheckRecoilPos();

        ShowBulletCount();
    }

    private void ShowBulletCount()
    {
        if (isGun)
        {
            Gun gun = (Gun) currentWeapon;
            bulletCountText.text = $"{gun.BulletCountInClip}/{gun.allBulletCount}";
        }
        else
        {
            bulletCountText.text = "";
        }
    }

    private void GetInput()
    {
        if (Cursor.lockState != CursorLockMode.Locked) return;
        if (changingWeapon) return;

        if (Input.GetKeyDown(KeyCode.R) && isGun && !aiming && !reloading)
            ReloadGun();
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        if (Math.Abs(scrollInput) > 0.01f && !aiming)
            StartCoroutine(ChangeWeapon(scrollInput > 0));
        if (Input.GetButtonDown("Fire1") && !isAuto) // 单发
            AttemptAttack();
        if (Input.GetButton("Fire1") && isAuto) // 连发
            AttemptAttack();
        if (Input.GetMouseButtonDown(1) && isGun)
            Aim();
        if (Input.GetMouseButtonUp(1) && isGun)
            UnAim();
    }

    #region reload

    [HideInInspector] public bool reloading;

    private void ReloadGun()
    {
        animator.SetTrigger(AnimParamReload);
        reloading = true;
        ((Gun) currentWeapon).Reload(
            () => aiming || attemptAttack || changingWeapon || playerMovement.isRunning,
            () => reloading = false,
            () =>
            {
                reloading = false;
                StartCoroutine(BreakReloadAnim());
            });
    }

    // 打断reload 动画
    private IEnumerator BreakReloadAnim()
    {
        animator.SetBool(AnimParamAiming, true);
        yield return null;
        animator.SetBool(AnimParamAiming, false);
    }

    #endregion

    #region attack

    private bool attemptAttack;

    private void AttemptAttack()
    {
        if (isGun)
        {
            Gun gun = (Gun) currentWeapon;
            if (playerMovement.isRunning) // 打断奔跑状态
            {
                playerMovement.isRunning = false;
                gun.AfterCanFire(0.3f);
                return;
            }

            if (gun.BulletCountInClip <= 0) // 若没有子弹，则装子弹
            {
                if (!reloading)
                    ReloadGun();
                return;
            }
        }

        attemptAttack = true;
        animator.SetBool(AnimParamAiming, true);
        StartCoroutine(DelaySetAttemptAttackFalse());

        Attack();
    }

    private void Attack()
    {
        // 分别处理枪和剑
        if (isGun)
        {
            animator.SetBool(AnimParamAiming, true);
            currentWeapon.Attack(SetRecoilEffect);
        }
        else
        {
            animator.SetTrigger(AnimParamAttack);
            currentWeapon.Attack(() => { StartCoroutine(DisactiveSword()); });
        }
    }

    IEnumerator DisactiveSword()
    {
        yield return new WaitForSeconds(0.5f);
        (currentWeapon as Sword)?.SetDotAttack();
    }

    private IEnumerator DelaySetAttemptAttackFalse()
    {
        yield return null;
        attemptAttack = false; // 延迟一帧 为打断其他动作
        if (!aiming)
            animator.SetBool(AnimParamAiming, false);
    }

    #endregion

    #region recoil effect

    public Transform model; // 改变位置，呈现后座力效果
    private Vector3 modelTargetPos;

    private void CheckRecoilPos()
    {
        model.localPosition = Vector3.Lerp(model.localPosition, modelTargetPos, 0.4f);
        if (Vector3.Distance(model.localPosition, modelTargetPos) < 0.01f)
        {
            modelTargetPos = Vector3.zero;
        }
    }

    // 后座力效果
    private void SetRecoilEffect()
    {
        Gun gun = (Gun) currentWeapon;
        Vector2 rand = Random.insideUnitCircle;
        modelTargetPos = Vector3.back * gun.recoilForce + new Vector3(rand.x, 0, rand.y) * gun.shakeForce;
        if (aiming)
            modelTargetPos /= 3;
    }

    #endregion

    #region changingWeapon

    private bool changingWeapon;
    private int currentWeaponIndex;
    private Weapon currentWeapon;
    [HideInInspector] public bool isGun;
    [HideInInspector] public bool isAuto;

    IEnumerator ChangeWeapon(bool forward)
    {
        Debug.Log("StartChageWeapon");
        changingWeapon = true;

        AudioManager.Instance.Play(changeWeaponAudio, 0.2f);
        animator.SetTrigger(AnimParamChangeWeapon);
        // 等待放下
        yield return new WaitForSeconds(0.9f);

        // 换武器
        weapons[currentWeaponIndex].gameObject.SetActive(false);
        currentWeaponIndex += (forward ? 1 : -1) + weapons.Length;
        currentWeaponIndex %= weapons.Length;
        SetCurrentWeapon();

        // 换层
        animator.SetLayerWeight(1, isGun ? 0 : 1);

        // 等待拿起
        yield return new WaitForSeconds(0.9f);

        changingWeapon = false;
    }

    private void HideOtherWeapon()
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].gameObject.SetActive(false);
        }

        SetCurrentWeapon();
    }

    private void SetCurrentWeapon()
    {
        currentWeapon = weapons[currentWeaponIndex];
        currentWeapon.gameObject.SetActive(true);
        isGun = currentWeapon is Gun;
        isAuto = (currentWeapon as Gun)?.auto == true;
    }

    #endregion

    #region aim and move camera

    [Header("Aiming")] public Camera mCamera;
    public Transform aimingCameraPos;
    public float aimFieldOfView = 40;
    public float cameraLerp = 0.1f;
    [HideInInspector] public bool aiming;
    private float oldFieldOfView;

    private void CheckAimState()
    {
        if (aiming && playerMovement.isRunning) // 被奔跑打断
        {
            aiming = false;
            animator.SetBool(AnimParamAiming, false);
        }
    }

    private void Aim()
    {
        playerMovement.isRunning = false; // 打断奔跑状态

        Vector3 targetPos = mCamera.transform.parent.InverseTransformPoint(aimingCameraPos.position);
        StartCoroutine(Aiming(true, targetPos, aimFieldOfView));
    }

    private void UnAim()
    {
        StartCoroutine(Aiming(false, Vector3.zero, oldFieldOfView));
    }

    // 移动摄像机 改变视野大小
    IEnumerator Aiming(bool aim, Vector3 targetPos, float fieldOfView)
    {
        aiming = aim;
        animator.SetBool(AnimParamAiming, aiming);
        while (aiming == aim)
        {
            mCamera.transform.localPosition = Vector3.Lerp(mCamera.transform.localPosition, targetPos, cameraLerp);
            mCamera.fieldOfView = Mathf.Lerp(mCamera.fieldOfView, fieldOfView, cameraLerp);
            yield return null;

            if ((mCamera.transform.localPosition - targetPos).sqrMagnitude < 0.000001f)
            {
                mCamera.transform.localPosition = targetPos;
                mCamera.fieldOfView = fieldOfView;
                Debug.Log($"aim {aim} end");
                yield break;
            }
        }

        Debug.Log($"aim {aim} end");
    }

    #endregion
}