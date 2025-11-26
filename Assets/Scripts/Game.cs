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
    WarFactory warFactory = default;

    [SerializeField]
    GameScenario scenario = default;

    [SerializeField,Range(0,100)]
    int startingPlayerHealth = 10;

    [SerializeField,Range(1f,10f)]
    float playSpeed = 1f;

    const float pausedTimeScale = 0f;

    int playerHealth;
    GameScenario.State activeScenario;

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

    public static Explosion SpawnExplosion()
    {
        Explosion explosion = instance.warFactory.Explosion;
        instance.nonEnemies.Add(explosion);
        return explosion;
    }
    private void Awake()
    {
        playerHealth = startingPlayerHealth;
        board.Initialize(boardSize, tileContentFactory);
        board.ShowGrid = true;
        activeScenario = scenario.Begin();
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

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Time.timeScale = Time.timeScale > pausedTimeScale ? pausedTimeScale : playSpeed;
        }
        else if (Time.timeScale > pausedTimeScale)
        {
            Time.timeScale = playSpeed;
        }

        //开始游戏
        if (Input.GetKeyDown(KeyCode.B))
        {
            BeginNewGame();
        }

        if(playerHealth<=0 && startingPlayerHealth > 0)
        {
            Debug.Log("Defeat!");
            BeginNewGame();
        }
        if (!activeScenario.Progress() && enemies.IsEmpty)
        {
            Debug.Log("Victory!");
            BeginNewGame();
            activeScenario.Progress();
        }

        activeScenario.Progress();//在最外层调用update

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

    void BeginNewGame()
    {
        playerHealth = startingPlayerHealth;
        enemies.Clear();
        nonEnemies.Clear();
        board.Clear();
        activeScenario = scenario.Begin();
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

    public static void SpawnEnemy(EnemyFactory factory,EnemyType type)
    {
        GameTile spawnPoint =instance.board.GetSpawnPoint(
            Random.Range(0, instance.board.SpawnPointCount));
        Enemy enemy = factory.Get(type);
        enemy.SpawnOn(spawnPoint);
        instance.enemies.Add(enemy);
    }

    public static void EnemyReachedDestination()
    {
        instance.playerHealth -= 1;
    }
}
