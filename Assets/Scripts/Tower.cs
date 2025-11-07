using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : GameTileContent
{
    [SerializeField, Range(1.5f, 10.5f)]
    float targetingRange = 1.5f;

    [SerializeField, Range(1f, 100f)]
    float damagePerSecond = 10f;

    [SerializeField]
    Transform turret = default,laserBeam = default;

    const int enemyLayerMask = 1 << 9;//enemy的layer

    TargetPoint target;

    Vector3 laserBeamScale;

    private void Awake()
    {
        laserBeamScale = laserBeam.localScale;
    }
    public override void GameUpdate()
    {
        base.GameUpdate();
        //有目标的话 锁定 直到出范围 重新检测
        if (TrackTarget() || AquireTaget())
        {
            //Debug.Log("Searching for target...");
            Shoot();
        }
        else
        {
            laserBeam.localScale = Vector3.zero;
        }
    }

    //搜寻目标
    static Collider[] targetsBuffer = new Collider[100];
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
            target = targetsBuffer[Random.Range(0,hits)].GetComponent<TargetPoint>();
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


    void Shoot()
    {
        Vector3 point = target.Position;
        turret.LookAt(point);
        laserBeam.localRotation = turret.localRotation;

        float d = Vector3.Distance(turret.position, point);
        laserBeamScale.z = d;
        laserBeam.localScale = laserBeamScale;
        laserBeam.localPosition = turret.localPosition + 0.5f * d * laserBeam.forward;

        target.Enemy.ApplyDamage(damagePerSecond * Time.deltaTime);
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
