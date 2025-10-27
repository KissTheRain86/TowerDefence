using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    EnemyFactory originFactory;

    public EnemyFactory OriginFactory
    {
        get => originFactory;

        set
        {
            Debug.Assert(originFactory == null, "重复定义了origin factory");
            originFactory = value;
        } 
    }

    GameTile tileFrom, tileTo;
    Vector3 positionFrom, positionTo;
    float progress;

    public void SpawnOn(GameTile tile)
    {
        Debug.Assert(tile.NextTileOnPath != null, "Nowhere to go", this);
        tileFrom = tile;
        tileTo = tile.NextTileOnPath;
        positionFrom = tileFrom.transform.localPosition;
        positionTo = tileTo.transform.localPosition;
        transform.localPosition = tile.transform.localPosition;
        progress = 0;
    }
    public bool GameUpdate()
    {
        progress += Time.deltaTime;
        while (progress >= 1f)
        {
            //向下一个目标前进
            tileFrom = tileTo;
            tileTo = tileFrom.NextTileOnPath;
            if (tileTo == null)
            {
                OriginFactory.Reclaim(this);
                return false;
            }
            positionFrom = positionTo;
            positionTo = tileTo.transform.localPosition;
            progress -= 1;
        }
        transform.localPosition = Vector3.LerpUnclamped(positionFrom, positionTo, progress);
        return true;
    }
}
