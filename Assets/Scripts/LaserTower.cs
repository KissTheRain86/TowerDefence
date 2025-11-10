using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserTower : Tower
{

    [SerializeField, Range(1f, 100f)]
    float damagePerSecond = 10f;

    [SerializeField]
    Transform turret = default,laserBeam = default;
    public override TowerType TowerType => TowerType.Laser;

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
        if (TrackTarget(ref target) || AquireTaget(out target))
        {
            //Debug.Log("Searching for target...");
            Shoot();
        }
        else
        {
            laserBeam.localScale = Vector3.zero;
        }
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

  
}
