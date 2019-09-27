using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float destroyTime = 10f;

    private Rigidbody rBody;
    private float damage;
    private bool collided; // 是否发生了碰撞

    private Vector3 lastDir; // 存储每一帧的方向， 在OnCollisionEnter中访问相当于上一帧
    private Vector3 lastPos; // 上一帧位置；

    private int lunchLayer;

    // Start is called before the first frame update
    void Awake()
    {
        rBody = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        collided = false;
        StartCoroutine(DelayRecycle(destroyTime));
    }

    private void OnDisable()
    {
        rBody.velocity = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        if (collided) return;
        transform.forward = rBody.velocity; // 子弹头朝向移动方向
        lastDir = transform.forward;
        lastPos = transform.position;
    }


    public void Set(float damage, Transform position, float speed, int lunchLayer)
    {
        this.damage = damage;
        transform.position = position.position;
        transform.rotation = position.rotation;
        rBody.velocity = transform.forward * speed;
        //todo 速度每次都不一样
        lastDir = transform.forward;
        lastPos = transform.position;
        this.lunchLayer = lunchLayer;
    }

    private void OnCollisionEnter(Collision other) // 先于update
    {
        GameCharacter character = other.gameObject.GetComponent<GameCharacter>();
        // 碰撞到角色
        if (character != null)
        {
            character.Damage(damage);
            PoolsManager.Instance.Recycle(gameObject);

            return;
        }

        if (collided) return;
        collided = true;

        lastPos -= lastDir; // 射线起点向后移动一个单位
        Debug.DrawRay(lastPos, lastDir * 10, Color.black);
        int layrMask = ~(1 << lunchLayer | LayerMask.GetMask("Bullet"));
        // 碰撞到其他物体,  碰撞点的法线与速度的夹角 < 60
        if (Physics.Raycast(new Ray(lastPos, lastDir), out var hit, 100, layrMask))
        {
            Debug.DrawRay(hit.point, -hit.normal * 10, Color.red);

            if (Vector3.Angle(lastDir, -hit.normal) < 60)
            {
                // 若小于60度 则销毁子弹， 生成弹孔
                Transform hole = PoolsManager.Instance.Get("BulletHole").transform;
                hole.position = hit.point + hit.normal * 0.0001f;
                hole.up = hit.normal;

                PoolsManager.Instance.Recycle(gameObject);
                return;
            }
        }

        // 射线没有判断到 或 大于45度 则等待一段时间后销毁
        StartCoroutine(DelayRecycle(2));


        // 碰撞顶点位置不准确
//        else if (other.contactCount > 0) // 碰撞到其他物体,  判断 碰撞点的法线与速度的夹角
//        {
//            Debug.DrawRay(other.contacts[0].point, -other.contacts[0].normal * 10, Color.red);
//            Debug.DrawRay(transform.position, lastDir * 10, Color.black);
//            if (Vector3.Angle(lastDir, -other.contacts[0].normal) < 45)
//            {
//                // 若小于45度 则销毁子弹， 生成弹孔
//                Debug.Log("生成弹孔");
//                Transform hole = PoolManager.Instance.Get("BulletHole").transform;
//                hole.position = other.contacts[0].point;
//                hole.up = other.contacts[0].normal;
//
//                PoolManager.Instance.Recycle(gameObject);
//            }
//            else
//            {
//                // 若大于45度 则等待一段时间后销毁
//                StartCoroutine(DelayRecycle(2));
//            }
//        }    
    }

    private IEnumerator DelayRecycle(float time)
    {
        yield return new WaitForSeconds(time);
        if (gameObject.activeSelf)
            PoolsManager.Instance.Recycle(gameObject);
    }
}