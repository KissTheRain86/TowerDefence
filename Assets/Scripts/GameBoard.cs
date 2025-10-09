using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoard : MonoBehaviour
{
    [SerializeField]
    Transform ground = default;

    [SerializeField]
    GameTile tilePrefab = default;

    Vector2Int size;

    GameTile[] tiles;

    Queue<GameTile> searchFrontier = new Queue<GameTile>();

  

    public void Initialize(Vector2Int size)
    {
        this.size = size;
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
            }
        }
        FindPaths();
    }


    public void FindPaths()
    {
        foreach(GameTile tile in tiles)
        {
            tile.ClearPath();
        }
        tiles[tiles.Length/2].SetDestination();
        searchFrontier.Enqueue(tiles[tiles.Length/2]); 
        
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
        //show path
        foreach(GameTile tile in tiles)
        {
            tile.ShowPath();
        }
    }

  
}
