using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : GameBehavior
{
    [SerializeField]
    Transform model = default;

    EnemyFactory originFactory;
    float pathOffset;
    float speed;

    float Health { get; set; }
    public float Scale { get; private set; }  

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
    float progress, progressFactor;

    Direction direction;
    DirectionChange directionChange;
    float directionAngleFrom,directionAngleTo;

    public void Initialize(float scale,float speed, float pathOffset)
    {
        Scale = scale;
        model.localScale = new Vector3(scale, scale, scale);
        this.speed = speed;
        this.pathOffset = pathOffset;
        Health = 100f * scale;
    }

    public void SpawnOn(GameTile tile)
    {
        Debug.Assert(tile.NextTileOnPath != null, "Nowhere to go", this);
        tileFrom = tile;
        tileTo = tile.NextTileOnPath;
        
        progress = 0;
        PrepareIntro();
    }

    public void ApplyDamage(float damage)
    {
        Debug.Assert(damage >= 0, "伤害为负数");
        Health -= damage;
    }

    public override bool GameUpdate()
    {
        if (Health <= 0)
        {
            originFactory.Reclaim(this);
            return false;
        }
        progress += Time.deltaTime * progressFactor;
        while (progress >= 1f)
        {  
            if (tileTo == null)
            {
                OriginFactory.Reclaim(this);
                return false;
            }
            progress = (progress-1f)/progressFactor;
            PrepareNextState();
            progress *= progressFactor;
        }
        if(directionChange == DirectionChange.None)
        {
            transform.localPosition = Vector3.LerpUnclamped(positionFrom, positionTo, progress);
        }
        else
        {
            float angle = Mathf.LerpUnclamped(
               directionAngleFrom, directionAngleTo, progress
           );
            transform.localRotation = Quaternion.Euler(0f, angle, 0f);
        }
        return true;
    }

  

    void PrepareNextState()
    {
        tileFrom = tileTo;
        tileTo = tileTo.NextTileOnPath;
        positionFrom = positionTo;
        if(tileTo == null)
        {
            PrepareOutro();
            return;
        }
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
        model.localPosition = new Vector3(pathOffset,0f);
        progressFactor = speed;
    }
    void PrepareTurnRight()
    {
        directionAngleTo = directionAngleFrom + 90f;
        //模拟扇形转向
        model.localPosition = new Vector3(pathOffset - 0.5f, 0f);
        transform.localPosition = positionFrom + direction.GetHafVector();
        progressFactor = speed / (Mathf.PI * 0.5f * (0.5f - pathOffset));
    }

    void PrepareTurnLeft()
    {
        directionAngleTo = directionAngleFrom - 90f;
        model.localPosition = new Vector3(pathOffset + 0.5f, 0f);
        transform.localPosition = positionFrom + direction.GetHafVector();
        progressFactor = speed / (Mathf.PI * 0.5f * (0.5f + pathOffset));
    }

    void PrepareTurnAround()
    {
        directionAngleTo = directionAngleFrom + (pathOffset<0f? 180f:-180f);
        model.localPosition = new Vector3(pathOffset,0f);
        transform.localPosition = positionFrom;
        progressFactor = speed / (Mathf.PI * Mathf.Max(Mathf.Abs(pathOffset), 0.2f));
    }

    void PrepareIntro()
    {
        positionFrom = tileFrom.transform.localPosition;
        positionTo = tileFrom.ExitPoint;
        direction = tileFrom.PathDirection;
        directionChange = DirectionChange.None;
        directionAngleFrom = directionAngleTo = direction.GetAngle();
        model.localPosition = new Vector3(pathOffset, 0f);
        transform.localRotation = direction.GetRotation();
        progressFactor = 2f * speed;
    }

    void PrepareOutro()
    {
        positionTo = tileFrom.transform.localPosition;
        directionChange = DirectionChange.None;
        directionAngleTo = direction.GetAngle();
        model.localPosition = new Vector3(pathOffset,0f);
        transform.localRotation = direction.GetRotation();
        progressFactor = 2f *  speed;
    }
}
