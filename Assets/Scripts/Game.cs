using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeField]
    Vector2Int boardSize = new Vector2Int(11, 11);

    [SerializeField]
    GameBoard board = default;

    [SerializeField]
    GameTileContentFactory tileContentFactory = default;

    [SerializeField]
    EnemyFactory enemyFactory = default;

    [SerializeField]
    WarFactory warFactory = default;

    [SerializeField,Range(0.1f,10f)]
    float spawnSpeed = 1f;

    float spawnProgress;

    GameBehaviorCollection enemies = new();
    GameBehaviorCollection nonEnemies = new();

    Ray TouchRay => Camera.main.ScreenPointToRay(Input.mousePosition);

    TowerType selectTowerType;

    static Game instance;

    public static Shell SpawnShell()
    {
        Shell shell = instance.warFactory.Shell;
        instance.nonEnemies.Add(shell);
        return shell;
    }
    private void Awake()
    {
        board.Initialize(boardSize, tileContentFactory);
        board.ShowGrid = true;
    }

    private void OnEnable()
    {
        instance = this;
    }

    private void Update()
    {
        //左键
        if (Input.GetMouseButtonDown(0))
        {
            HandleTouch();
        }else if (Input.GetMouseButtonDown(1))
        {
            //右键
            HandleAlternativeTouch();
        }

        if(Input.GetKeyDown(KeyCode.V))
        {
            board.ShowPaths = !board.ShowPaths;
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            board.ShowGrid = !board.ShowGrid;
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            selectTowerType = TowerType.Laser;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            selectTowerType = TowerType.Mortar;
        }

        spawnProgress += spawnSpeed * Time.deltaTime;
        while (spawnProgress >= 1f)
        {
            spawnProgress -= 1f;
            SpawnEnemy();
        }

        enemies.GameUpdate();
        Physics.SyncTransforms();
        board.GameUpdate();
        nonEnemies.GameUpdate();
    }

    private void OnValidate()
    {
        if (boardSize.x < 2)
        {
            boardSize.x = 2;
        }
        if (boardSize.y < 2)
        {
            boardSize.y = 2;
        }
    }

    private void HandleAlternativeTouch()
    {
        GameTile tile = board.GetTile(TouchRay);
        if (tile != null)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                board.ToggleDestination(tile);
            }
            else
            {
                board.ToggleSpawnPoint(tile);
            }
            
        }
    }

    void HandleTouch()
    {
        GameTile tile = board.GetTile(TouchRay);
        if (tile != null)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                board.ToggleTower(tile,selectTowerType);
            }
            else
            {
                //tile.Content = tileContentFactory.Get(GameTileContentType.Destination);
                board.ToggleWall(tile);
            }
            
        }
    }

    void SpawnEnemy()
    {
        GameTile spawnPoint = board.GetSpawnPoint(Random.Range(0, board.SpawnPointCount));
        Enemy enemy = enemyFactory.Get();
        enemy.SpawnOn(spawnPoint);
        enemies.Add(enemy);
    }

}
