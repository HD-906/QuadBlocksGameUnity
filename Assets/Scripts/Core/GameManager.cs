using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> currentBag = new List<GameObject>();
    [SerializeField] private GameObject[] tetrominoPrefabs;
    [SerializeField] private GameObject current;
    [SerializeField] private GameObject onHold;
    [SerializeField] public bool holdLocked = false;
    [SerializeField] public Transform[,] grid;
    [SerializeField] public static bool isGameOver = false;
    [SerializeField] private PreviewController preview;
    [SerializeField] Transform fieldOrigin;
    [SerializeField] float cellSize = GameConsts.CellSize;
    [SerializeField] public int level = 1;
    [SerializeField] private InFieldStatus fieldStatus;

    private Vector2Int spawnPosition = GameConsts.SpawnCell;

    public const int width = GameConsts.GridWidth;
    public const int height = GameConsts.GridHeight;

    private bool[] lastPieceMovementStatus = { false, false };
    private float holdTime = GameConsts.holdTime;

    private float previousRTime;
    private float previousFTime;
    private KeyCode restart;
    private KeyCode forfeit;

    void Awake()
    {
        if (fieldOrigin == null)
        {
            fieldOrigin = transform;
        }

        grid = new Transform[width, height];
        FillBag();
        SpawnNextTetromino();
        preview.ShowNext(currentBag);
        preview.ShowHold(onHold);

        restart = Bootstrap.I.config.restart;
        forfeit = Bootstrap.I.config.forfeit;
    }

    void Update()
    {
        ExecuteWhenHeld(restart, ref previousRTime, RestartGame);
        ExecuteWhenHeld(forfeit, ref previousFTime, ForfeitGame);
    }

    private void ExecuteWhenHeld(KeyCode key, ref float previousTime, System.Action act)
    {
        if (Input.GetKeyDown(key))
        {
            previousTime = Time.time;
        }

        if (Input.GetKey(key) && Time.time - previousTime > holdTime)
        {
            act();
        }
    }

    private void FillBag()
    {
        List<GameObject> tempBag = new List<GameObject>(tetrominoPrefabs);
        for (int i = 0; i < tempBag.Count; i++) // Fisher–Yates Shuffle
        {
            int randomIndex = Random.Range(i, tempBag.Count);
            (tempBag[i], tempBag[randomIndex]) = (tempBag[randomIndex], tempBag[i]);
        }
        currentBag.AddRange(tempBag);
    }

    private void InstantiateNewMino(GameObject prefabObj)
    {
        var pos = CellToWorld(spawnPosition);
        var newMino = Instantiate(prefabObj, pos, Quaternion.identity);
        Tetromino tetr = newMino.GetComponent<Tetromino>();

        newMino.GetComponent<Tetromino>().gameManager = this;
        newMino.transform.SetParent(fieldOrigin, true);


        var logic = newMino.GetComponent<InFieldLogic>();
        if (logic != null)
        {
            logic.gameManager = this;
            logic.SetCancelStatus(lastPieceMovementStatus[0], lastPieceMovementStatus[1]);
            logic.enabled = true;
        }

        int tryCount = 0;
        while (!tetr.IsValidMove() && tryCount++ < 2)
        {
            newMino.transform.position += Vector3.up;
        }

        if (tryCount >= 3)
        {
            GameOver();
        }
    }

    public void SpawnNextTetromino()
    {
        holdLocked = false;

        if (isGameOver)
            return;

        if (currentBag.Count < 7)
            FillBag();

        current = currentBag[0];
        currentBag.RemoveAt(0);

        preview.ShowNext(currentBag);

        InstantiateNewMino(current);
    }

    public bool HoldCurrent()
    {
        if (holdLocked)
        {
            return false;
        }

        if (onHold == null)
        {
            onHold = current;
            SpawnNextTetromino();
        }
        else
        {
            (current, onHold) = (onHold, current);
            InstantiateNewMino(current);
        }

        holdLocked = true;
        preview.ShowHold(onHold);
        return true;
    }

    public void UpdateLastMovementStatus(bool statusLeft, bool statusRight)
    {
        lastPieceMovementStatus[0] = statusLeft;
        lastPieceMovementStatus[1] = statusRight;
    }

    public int ClearFullLines()
    {
        int cleared = 0;
        int upperBound = height;
        List<int> toClearList = new List<int>();
        for (int y = 0; y < height; y++)
        {
            bool full = true;
            bool empty = true;
            for (int x = 0; x < width; x++)
            {
                if (grid[x, y] == null)
                {
                    full = false;
                    if (!empty) break;
                }
                else
                {
                    empty = false;
                    if (!full) break;
                }
            }

            if (empty)
            {
                upperBound = y;
                break;
            }

            if (full)
            {
                toClearList.Add(y);
                cleared++;
            }
        }

        if (cleared == 0)
        {
            return 0;
        }

        if (cleared == upperBound)
        {
            fieldStatus.AddScorePerfectClear(cleared, level);
        }

        int yToFill = toClearList[0];
        int yFiller = yToFill + 1;

        for (int y = yToFill; y < upperBound; y++)
        {
            while (toClearList.Contains(yFiller))
            {
                if (yFiller < upperBound)
                {
                    yFiller++;
                }
            }

            for (int x = 0; x < width; x++)
            {
                Destroy(grid[x, y]?.gameObject);
                grid[x, y] = grid[x, yFiller];
                grid[x, yFiller] = null;
                if (grid[x, y] != null)
                {
                    grid[x, y].position += Vector3.down * (yFiller - y);
                }
            }

            if (yFiller < upperBound)
            {
                yFiller++;
            }
        }
        return cleared;
    }

    public void AddScore(int toAdd)
    {
        fieldStatus.AddScore(toAdd);
    }

    public void AddScoreBonus(int linesCleared, int tSpinStatus)
    {
        fieldStatus.AddScoreBonus(linesCleared, tSpinStatus, level);
    }

    public void GameOver()
    {
        Debug.Log("GAME OVER!");
        Time.timeScale = 0f;
        isGameOver = true;
    }

    public void RestartGame()
    {
        Scene current = SceneManager.GetActiveScene();
        SceneManager.LoadScene(current.name);
    }

    public void ForfeitGame()
    {
        SceneManager.LoadScene(SceneNames.MainMenu);
    }

    public Vector3 CellToWorld(Vector2Int cell)
    {
        return fieldOrigin.TransformPoint(new Vector3(cell.x, cell.y, 0) * cellSize);
    }

    public Vector2Int WorldToCell(Vector3 world)
    {
        Vector3 local = fieldOrigin.InverseTransformPoint(world) / cellSize;
        return Vector2Int.RoundToInt(new Vector2(local.x, local.y));
    }
}
