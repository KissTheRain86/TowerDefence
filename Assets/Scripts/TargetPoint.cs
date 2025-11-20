using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetPoint : MonoBehaviour
{
    const int enemyLayerMask = 1 << 9;
    static Collider[] buffer = new Collider[100];
    public static int BufferCount { get;private set; }
    //获取碰撞个数
    public static bool FillBuffer(Vector3 position,float range)
    {
        Vector3 top = position;
        top.y += 3;
        BufferCount = Physics.OverlapCapsuleNonAlloc(
            position, top, range, buffer, enemyLayerMask);
        return BufferCount > 0;
    }
    //获取碰撞范围内的目标
    public static TargetPoint GetBuffered(int index)
    {
        var target = buffer[index].GetComponent<TargetPoint>();
        Debug.Assert(target != null, "Targeted non-enemy", buffer[0]);
        return target;
    }

    public static TargetPoint RandomBuffered =>
        GetBuffered(Random.Range(0, BufferCount));

    private void Awake()
    {
        Enemy = transform.root.GetComponent<Enemy>();
        Debug.Assert(Enemy != null, "Target point without enemy root",this);
        Debug.Assert(GetComponent<SphereCollider>() != null,
            "Target point without sphere collider",this);
        Debug.Assert(gameObject.layer == 9, "Target point on wrong layer!", this);
    }
    public Enemy Enemy { get; private set; }
    public Vector3 Position => transform.position;  
}
