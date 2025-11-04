using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : GameTileContent
{
    [SerializeField, Range(1.5f, 10.5f)]
    float targetingRange = 1.5f;

    const int enemyLayerMask = 1 << 9;//enemy的layer

    TargetPoint target;
    public override void GameUpdate()
    {
        base.GameUpdate();
        //有目标的话 锁定 直到出范围 重新检测
        if (TrackTarget() || AquireTaget())
        {
            Debug.Log("Searching for target...");
        }
    }

    //搜寻目标
    static Collider[] targetsBuffer = new Collider[1];
    private bool AquireTaget()
    {
        Vector3 a = transform.localPosition;
        Vector3 b = a;
        b.y += 3f;
        int hits =  Physics.OverlapCapsuleNonAlloc(
               a,b,targetingRange,targetsBuffer,enemyLayerMask
            );
        if (hits > 0)
        {
            target = targetsBuffer[0].GetComponent<TargetPoint>();
            Debug.Assert(target != null, "Targeted non-enemy", targetsBuffer[0]);
            return true;
        }
        target = null;
        return false;
    }

    bool TrackTarget()
    {
        if(target==null) return false;
        Vector3 a = transform.localPosition;
        Vector3 b = target.Position;
        if (Vector3.Distance(a, b) > targetingRange+0.125f*target.Enemy.Scale)
        {
            target = null;
            return false;
        }
        return true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 position = transform.localPosition;
        position.y += 0.01f;
        Gizmos.DrawWireSphere(position, targetingRange);
        //绘制target
        if (target != null)
        {
            Gizmos.DrawLine(position, target.Position);
        }
    }
}
