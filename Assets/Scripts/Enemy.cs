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

    Direction direction;
    DirectionChange directionChange;
    float directionAngleFrom,directionAngleTo;

    public void SpawnOn(GameTile tile)
    {
        Debug.Assert(tile.NextTileOnPath != null, "Nowhere to go", this);
        tileFrom = tile;
        tileTo = tile.NextTileOnPath;
        
        progress = 0;
        PrepareIntro();
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
            progress -= 1;
            PrepareNextState();
        }

        transform.localPosition = Vector3.LerpUnclamped(positionFrom, positionTo, progress);
        //转向
        if (directionChange != DirectionChange.None)
        {
            float angle = Mathf.LerpUnclamped(
                directionAngleFrom, directionAngleTo, progress
            );
            transform.localRotation = Quaternion.Euler(0f, angle, 0f);
        }
        return true;
    }

    void PrepareIntro()
    {
        positionFrom = tileFrom.transform.localPosition;
        positionTo = tileFrom.ExitPoint;
        direction = tileFrom.PathDirection;
        directionChange = DirectionChange.None;
        directionAngleFrom = directionAngleTo = direction.GetAngle();
        transform.localRotation = direction.GetRotation();
    }

    void PrepareNextState()
    {
        positionFrom = positionTo;
        positionTo = tileFrom.ExitPoint;
        directionChange = direction.GetDirectionChangeTo(tileFrom.PathDirection);
        direction = tileFrom.PathDirection;
        directionAngleFrom = directionAngleTo;
        switch (directionChange)
        {
            case DirectionChange.None: PrepareForward(); break;
            case DirectionChange.TurnRight: PrepareTurnRight();break;
            case DirectionChange.TurnLeft: PrepareTurnLeft();break;
            case DirectionChange.TurnAround: PrepareTurnAround(); break;
        }
    }

    void PrepareForward()
    {
        transform.localRotation = direction.GetRotation();
        directionAngleTo = direction.GetAngle();
    }
    void PrepareTurnRight()
    {
        directionAngleTo = directionAngleFrom + 90f;
    }

    void PrepareTurnLeft()
    {
        directionAngleTo = directionAngleFrom - 90f;
    }

    void PrepareTurnAround()
    {
        directionAngleTo = directionAngleFrom + 180f;
    }
}
