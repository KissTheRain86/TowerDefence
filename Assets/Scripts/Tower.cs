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
        if (AquireTaget())
        {
            Debug.Log("Searching for target...");
        }
    }

    //搜寻目标
    private bool AquireTaget()
    {
        Collider[] targets = Physics.OverlapSphere(
                transform.localPosition, targetingRange,enemyLayerMask
            );
        if (targets.Length > 0)
        {
            target = targets[0].GetComponent<TargetPoint>();
            Debug.Assert(target != null, "Targeted non-enemy", targets[0]);
            return true;
        }
        target = null;
        return false;
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
