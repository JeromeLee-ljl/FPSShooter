using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerMovement : MonoBehaviour
{
    [Header("Move")] public float jumpForce = 300;
    public float walkSpeed = 3;
    public float runSpeed = 5;
    public Camera minMapCamera;
    private PlayerState playerState = PlayerState.Controlled;
    private NavMeshAgent navMeshAgent;
    private Animator animator;
    private Rigidbody rb;
    private PlayerWeaponController playerWeaponController;
    

    private enum PlayerState
    {
        AutoNav, //自动寻路状态
        Controlled, //键盘控制状态
    }

    #region Life Circle

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        playerWeaponController = GetComponent<PlayerWeaponController>();
        ChangeStateTo(PlayerState.Controlled);
    }

    void Update()
    {
        // 若有输入 转为控制状态   、  鼠标锁定 为控制状态
        if (KeyboardInput() || Cursor.lockState == CursorLockMode.Locked)
            ChangeStateTo(PlayerState.Controlled);

        ClickMinMapLogic();
    }

    private void FixedUpdate()
    {
        if (playerState == PlayerState.Controlled)
        {
            MoveLogic();
            JumpLogic();
        }
    }

    #endregion

    #region Main Logic

    [HideInInspector] public bool isRunning; // 跑步可以被攻击、瞄准 打断
    private static readonly int AnimParamSpeed = Animator.StringToHash("Speed");
    private Vector3 inputDir = Vector3.zero;
    private Vector3 horizontalVelocity;
    private bool jump;

    // 改变状态，设置rigidbody 和 navmeshagent
    private void ChangeStateTo(PlayerState state)
    {
        if (playerState == state) return;
        playerState = state;
        navMeshAgent.enabled = playerState == PlayerState.AutoNav;
        rb.isKinematic = playerState == PlayerState.AutoNav;
    }

    // 返回是否有输入
    private bool KeyboardInput()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        bool lShiftDown = Input.GetKeyDown(KeyCode.LeftShift);
        bool lShiftUp = Input.GetKeyUp(KeyCode.LeftShift);
        bool space = Input.GetKeyDown(KeyCode.Space);

        inputDir.Set(horizontal, 0, vertical);
        inputDir.Normalize();
        if (onTheGround && space) jump = true;
        if (lShiftDown) isRunning = true;
        if (lShiftUp) isRunning = false;

        return Math.Abs(horizontal) > 0.1f || Math.Abs(vertical) > 0.1f ||
               lShiftDown || lShiftUp || space;
    }


    private void MoveLogic()
    {
        float maxSpeed = isRunning ? runSpeed : walkSpeed;
        Vector3 targetVelocity = Vector3.zero;
        if (onTheGround)
        {
            targetVelocity = transform.TransformVector(inputDir * maxSpeed);
//            转向移动时horizontalVelocity会降低，因为它是通过插值获得的
//            horizontalVelocity = Vector3.Lerp(horizontalVelocity, targetVelocity, 0.1f);
            // 使用球形插值 防止速度因为转向而减小
            horizontalVelocity = Vector3.Slerp(horizontalVelocity, targetVelocity, 0.3f);
            rb.velocity = new Vector3(horizontalVelocity.x, rb.velocity.y, horizontalVelocity.z);
        }
        else // 在空中时targetVelocity=0, horizontalVelocity 逐渐变为 0 , 但rb不变
        {
            horizontalVelocity = Vector3.Lerp(horizontalVelocity, targetVelocity, 0.3f);
        }

        // 处理声音和动画    跳起时声音立即消失  动画逐渐idle
        float speed = horizontalVelocity.magnitude;
        PlayMoveAudioBySpeed(onTheGround ? speed : 0);
        animator.SetFloat(AnimParamSpeed, speed);
    }

    private void JumpLogic()
    {
        if (!jump) return;
        jump = false;
        AudioManager.Instance.Play(jumpAudio, pitchRange);
        rb.AddForce(transform.up * jumpForce);
    }

    // 点击小地图自动寻路
    private void ClickMinMapLogic()
    {
        // 在没有锁定鼠标的情况下点击左键
        if (Cursor.lockState == CursorLockMode.None && Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = minMapCamera.ScreenToViewportPoint(Input.mousePosition);
            // 判断鼠标是否在小地图内
            if (mousePos.x < 0 || mousePos.x > 1 || mousePos.y < 0 || mousePos.y > 1) return;
            Ray ray = minMapCamera.ViewportPointToRay(mousePos);
            if (Physics.Raycast(ray, out var hit, 1000, LayerMask.GetMask("Floor")))
            {
                ChangeStateTo(PlayerState.AutoNav);
                navMeshAgent.SetDestination(hit.point);
            }
        }
    }

    #endregion

    #region check if on the ground

    private List<ContactPoint> contactPoints = new List<ContactPoint>();
    private bool onTheGround;

    // 碰撞事件参数可以省略，触发器事件不可以
    private void OnCollisionExit() => onTheGround = false;

    void OnCollisionStay(Collision collisionInfo)
    {
        int count = collisionInfo.GetContacts(contactPoints);
        // Debug-draw all contact points and normals
        for (int i = 0; i < count; i++)
        {
            var contact = contactPoints[i];
            if (Vector3.Angle(contact.normal, Vector3.up) < 60)
            {
                onTheGround = true;
                break;
            }
        }
    }

    #endregion

    #region audio 

    [Header("audio")] [Range(0, 0.5f)] public float pitchRange = 0.2f;
    public AudioSource jumpAudio;
    public AudioSource moveAudio;
    public AudioClip walkSound;
    public AudioClip runSound;

    private void PlayMoveAudioBySpeed(float speed)
    {
        if (speed > Mathf.Lerp(walkSpeed, runSpeed, 0.5f))
            AudioManager.Instance.Play(moveAudio, runSound, pitchRange);
        else if (speed > 0.1f)
            AudioManager.Instance.Play(moveAudio, walkSound, pitchRange);
        else if (moveAudio.isPlaying)
            moveAudio.Stop();
    }

    #endregion
}