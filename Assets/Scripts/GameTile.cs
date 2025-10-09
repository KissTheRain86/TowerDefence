using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTile : MonoBehaviour
{
    [SerializeField]
    Transform arrow = default;

    //四个相邻的
    GameTile north, east, south, west, nextOnPath;

    //与目的地之间的距离
    int distance;

    public void ClearPath()
    {
        distance = int.MaxValue;
        nextOnPath = null;
    }

    public void SetDestination()
    {
        distance = 0;
        nextOnPath = null;
    }

    public bool HasPath => distance != int.MaxValue;

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

    public GameTile GrowPathNorth() => GrowPathTo(north);
    public GameTile GrowPathEast()=> GrowPathTo(east);
    public GameTile GrowPathSouth() => GrowPathTo(south);
    public GameTile GrowPathWest() => GrowPathTo(west);

    public bool IsAlternative { get; set; }

    private GameTile GrowPathTo(GameTile neighbor)
    {
        Debug.Assert(HasPath, "No Path!");
        //保证邻居不为空 而且还没有路径
        if (neighbor == null || neighbor.HasPath) return null;
        neighbor.distance = distance + 1;
        neighbor.nextOnPath = this;
        return neighbor;
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
}
