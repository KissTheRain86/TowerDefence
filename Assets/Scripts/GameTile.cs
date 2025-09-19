using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTile : MonoBehaviour
{
    [SerializeField]
    Transform arrow = default;

    //�ĸ����ڵ�
    GameTile north, east, south, west, nextOnPath;

    //��Ŀ�ĵ�֮��ľ���
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

    //��ʶ���ߺ����ߵ����������ڵ�
    public static void MakeEastWestNeigbors(GameTile east,GameTile west)
    {
        Debug.Assert(west.east == null && east.west == null, "�ظ��������ھ� east west");
        west.east = east;
        east.west = west;
    }

    //��ʶ���ߺ��ϱߵ����������ڵ�
    public static void MakeNorthSouthNeigbors(GameTile north, GameTile south)
    {
        Debug.Assert(south.north == null && north.south == null, "�ظ��������ھ� north south");
        south.north = north;
        north.south = south;
    }
}
