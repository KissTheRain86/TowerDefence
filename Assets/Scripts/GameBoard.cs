﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoard : MonoBehaviour
{
    [SerializeField]
    Transform ground = default;

    [SerializeField]
    GameTile tilePrefab = default;

    [SerializeField]
    Texture2D gridTexture = default;

    Vector2Int size;

    GameTile[] tiles;

    Queue<GameTile> searchFrontier = new Queue<GameTile>();
    List<GameTile> spawnPoints = new();
    GameTileContentFactory contentFactory;

    bool showGrid,showPaths;

    public bool ShowPaths
    {
        get => showPaths;
        set
        {
            showPaths = value;
            if (showPaths)
            {
                foreach(var tile in tiles)
                {
                    tile.ShowPath();
                }
            }
            else
            {
                foreach(var tile in tiles)
                {
                    tile.HidePath();
                }
            }
        }
    }

    public bool ShowGrid
    {
        get => showGrid;
        set
        {
            showGrid = value;
            Material m = ground.GetComponent<MeshRenderer>().material;
            if (showGrid)
            {
                m.mainTexture = gridTexture;
                m.SetTextureScale("_BaseMap", size);
            }
            else
            {
                m.mainTexture = null;
            }
        }
    }

    public GameTile GetTile(Ray ray)
    {
        if (Physics.Raycast(ray,out RaycastHit hit))
        {
            int x = (int)(hit.point.x + size.x * 0.5f);
            int y = (int)(hit.point.z + size.y * 0.5f);
            if(x>=0 && x<size.x && y>=0 && y < size.y)
            {
                return tiles[x + y * size.x];
            }
           
        }
        return null;
    }

    public GameTile GetSpawnPoint(int index)
    {
        return spawnPoints[index];
    }

    public int SpawnPointCount => spawnPoints.Count;

    public void Initialize(Vector2Int size,GameTileContentFactory contentFactory)
    {
        this.size = size;
        this.contentFactory = contentFactory;
        ground.localScale = new Vector3(size.x, size.y, 1f);

        Vector2 offset = new Vector2(
            (size.x - 1) * 0.5f, (size.y - 1) * 0.5f
        );
        tiles = new GameTile[size.x * size.y];
        for (int i = 0, y = 0; y < size.y; y++)
        {
            for (int x = 0; x < size.x; x++,i++)
            {
                GameTile tile = Instantiate(tilePrefab);
                tiles[i] = tile;
                tile.transform.SetParent(transform, false);
                tile.transform.localPosition = new Vector3(
                    x - offset.x, 0f, y - offset.y
                );
                if (x > 0)
                {
                    GameTile.MakeEastWestNeigbors(tile, tiles[i - 1]);
                }
                if (y > 0)
                {
                    GameTile.MakeNorthSouthNeigbors(tile, tiles[i - size.x]);
                }
                tile.IsAlternative = (x & 1) == 0;//为0说明是偶数
                if ((y & 1) == 0)
                {
                    tile.IsAlternative = !tile.IsAlternative;
                }
                tile.Content = contentFactory.Get(GameTileContentType.Empty);
            }
        }
        ToggleDestination(tiles[tiles.Length / 2]);
        ToggleSpawnPoint(tiles[0]);
    }


    public bool FindPaths()
    {
        foreach(GameTile tile in tiles)
        {
            if(tile.Content.Type == GameTileContentType.Destination)
            {
                tile.BecomeDestination();
                searchFrontier.Enqueue(tile);
            }
            else
            {
                tile.ClearPath();
            }
        }
        if(searchFrontier.Count==0) return false;
        //tiles[tiles.Length/2].BecomeDestination();
        //searchFrontier.Enqueue(tiles[tiles.Length/2]); 
        
        while(searchFrontier.Count > 0)
        {
            GameTile tile = searchFrontier.Dequeue();
            if (tile == null) continue;
            if (tile.IsAlternative)
            {
                searchFrontier.Enqueue(tile.GrowPathNorth());
                searchFrontier.Enqueue(tile.GrowPathSouth());
                searchFrontier.Enqueue(tile.GrowPathEast());
                searchFrontier.Enqueue(tile.GrowPathWest());
            }
            else
            {
                searchFrontier.Enqueue(tile.GrowPathWest());
                searchFrontier.Enqueue(tile.GrowPathEast());
                searchFrontier.Enqueue(tile.GrowPathSouth());
                searchFrontier.Enqueue(tile.GrowPathNorth());
            }
           
        }
        
        //如果有一个格子无法到达终点 寻路失败
        foreach(GameTile tile in tiles)
        {
            if(!tile.HasPath) return false;
        }
        //寻路成功 显示结果
        if (showPaths)
        {
            foreach (GameTile tile in tiles)
            {
                tile.ShowPath();
            }
        }
        
        return true;
    }

    /// <summary>
    /// 切换为目的地
    /// </summary>
    /// <param name="tile"></param>
    public void ToggleDestination(GameTile tile)
    {
        if (tile.Content.Type == GameTileContentType.Destination)
        {
            tile.Content = contentFactory.Get(GameTileContentType.Empty);
            if (!FindPaths())
            {
                tile.Content = contentFactory.Get(GameTileContentType.Destination);
                FindPaths();
            }     
        }
        else if(tile.Content.Type == GameTileContentType.Empty)
        {
            tile.Content = contentFactory.Get(GameTileContentType.Destination);
            FindPaths();
        }      
    }     

    //切换为wall
    public void ToggleWall(GameTile tile)
    {
        if(tile.Content.Type == GameTileContentType.Wall)
        {
            tile.Content = contentFactory.Get(GameTileContentType.Empty);
            FindPaths();
        }
        else if(tile.Content.Type == GameTileContentType.Empty)
        {
            tile.Content = contentFactory.Get(GameTileContentType.Wall);
            if (!FindPaths())
            {
                //回退
                tile.Content = contentFactory.Get(GameTileContentType.Empty);
                FindPaths();
            }
        }
    }

   
    public void ToggleSpawnPoint(GameTile tile)
    {
        if(tile.Content.Type == GameTileContentType.SpawnPoint)
        {
            if (spawnPoints.Count > 1)
            {
                spawnPoints.Remove(tile);
                tile.Content = contentFactory.Get(GameTileContentType.Empty);
            }
           
        }
        else if(tile.Content.Type == GameTileContentType.Empty)
        {
            tile.Content = contentFactory.Get(GameTileContentType.SpawnPoint);
            spawnPoints.Add(tile);
        }
    }
}
