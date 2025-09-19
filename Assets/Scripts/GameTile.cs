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
}
