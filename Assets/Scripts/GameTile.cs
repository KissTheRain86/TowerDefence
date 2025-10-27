using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Accessibility;

public class GameTile : MonoBehaviour
{
    [SerializeField]
    Transform arrow = default;

    //四个相邻的
    GameTile north, east, south, west, nextOnPath;

    //与目的地之间的距离
    int distance;

    GameTileContent content;
    public GameTileContent Content
    {
        get => content;
        set
        {
            Debug.Assert(value != null, "Null assigned to content");
            if (content != null) content.Recycle();
            content = value;
            content.transform.localPosition = transform.localPosition;
        }
    }

    public Vector3 ExitPoint { get; private set; }

    public Direction PathDirection { get; private set; }

    public void ClearPath()
    {
        distance = int.MaxValue;
        nextOnPath = null;
    }

    public void BecomeDestination()
    {
        distance = 0;
        nextOnPath = null;
        ExitPoint = transform.localPosition;
    }

    public bool HasPath => distance != int.MaxValue;
    public GameTile NextTileOnPath=> nextOnPath;

    static Quaternion
      northRotation = Quaternion.Euler(90f, 0f, 0f),
      eastRotation = Quaternion.Euler(90f, 90f, 0f),
      southRotation = Quaternion.Euler(90f, 180f, 0f),
      westRotation = Quaternion.Euler(90f, 270f, 0f);


    //标识东边和西边的两块是相邻的
    public static void MakeEastWestNeigbors(GameTile east,GameTile west)
    {
        Debug.Assert(west.east == null && east.west == null, "重复定义了邻居 east west");
        west.east = east;
        east.west = west;
    }

    //标识北边和南边的两块是相邻的
    public static void MakeNorthSouthNeigbors(GameTile north, GameTile south)
    {
        Debug.Assert(south.north == null && north.south == null, "重复定义了邻居 north south");
        south.north = north;
        north.south = south;
    }

    public GameTile GrowPathNorth() => GrowPathTo(north,Direction.South);
    public GameTile GrowPathEast()=> GrowPathTo(east,Direction.West);
    public GameTile GrowPathSouth() => GrowPathTo(south,Direction.North);
    public GameTile GrowPathWest() => GrowPathTo(west,Direction.East);

    public bool IsAlternative { get; set; }

    private GameTile GrowPathTo(GameTile neighbor,Direction direction)
    {
        Debug.Assert(HasPath, "No Path!");
        //保证邻居不为空 而且还没有路径
        if (!HasPath || neighbor == null || neighbor.HasPath) return null;
        neighbor.distance = distance + 1;
        neighbor.nextOnPath = this;
        neighbor.ExitPoint = (neighbor.transform.localPosition + transform.localPosition) * 0.5f;
        neighbor.PathDirection = direction;
        return neighbor.Content.Type!=GameTileContentType.Wall? neighbor:null;
    }

    public void ShowPath()
    {
        if (distance == 0)
        {
            arrow.gameObject.SetActive(false);
            return;
        }
        arrow.gameObject.SetActive(true);
        arrow.localRotation =
            nextOnPath == north ? northRotation :
            nextOnPath == east ? eastRotation :
            nextOnPath == south ? southRotation :
            westRotation;
    }

    public void HidePath()
    {
        arrow.gameObject.SetActive(false);
    }


}

