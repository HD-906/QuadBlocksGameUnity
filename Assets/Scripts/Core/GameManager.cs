using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> currentBag = new List<GameObject>();
    [SerializeField] private GameObject[] tetrominoPrefabs;
    [SerializeField] private GameObject blockPrefab;
    [SerializeField] private GameObject current;
    [SerializeField] private GameObject onHold;
    [SerializeField] private bool holdLocked = false;
    [SerializeField] public Transform[,] grid;
    [SerializeField] public static bool gameFinished = false;
    [SerializeField] private PreviewController preview;
    [SerializeField] Transform fieldOrigin;
    [SerializeField] float cellSize = GameConsts.CellSize;
    [SerializeField] public int level = 1;
    [SerializeField] private InFieldStatus fieldStatus;
    [SerializeField] TMP_Text countDownText;

    [HideInInspector] public ModeManager modeManager;
    [HideInInspector] public bool is_2P;

    private Vector2Int spawnPosition = GameConsts.SpawnCell;

    public const int gridWidth = GameConsts.GridWidth;
    public const int gridHeight = GameConsts.GridHeight;

    private bool[] lastPieceMovementStatus = { false, false };
    private float holdTime = GameConsts.holdTime;

    private float previousRTime;
    private float previousFTime;
    private KeyCode restart;
    private KeyCode forfeit;
    private int currentTop = 0;

    private float countDown = GameConsts.startCountdown;
    public bool started = false;

    private int garbageQueue = 0;

    [SerializeField] public ControlConfig ctrlCfg;

    void Awake()
    {
        if (fieldOrigin == null)
        {
            fieldOrigin = transform;
        }

        grid = new Transform[gridWidth, gridHeight];
        FillBag();
        preview.ShowNext(currentBag);
        preview.ShowHold(onHold);

        restart = ctrlCfg.restart;
        forfeit = ctrlCfg.forfeit;
    }

    private void Start()
    {
        countDownText.fontSize = 42;
        countDownText.gameObject.SetActive(true);
        Time.timeScale = 1f;
        gameFinished = false;
    }

    void Update()
    {
        if (!started)
        {
            countDown -= Time.deltaTime;
            countDownText.text = $"{(int)(countDown + 1)}";
            if (countDown <= 0)
            {
                SpawnNextTetromino();
                started = true;
                countDownText.text = "";
                countDownText.fontSize = 14;
                countDownText.gameObject.SetActive(false);
            }
        }
        ExecuteWhenHeld(restart, ref previousRTime, RestartGame);
        ExecuteWhenHeld(forfeit, ref previousFTime, ForfeitGame);

        Testing(); // for debug testing
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

        tetr.gameManager = this;
        newMino.transform.SetParent(fieldOrigin, true);


        var logic = newMino.GetComponent<TetroLogic>();
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

        if (level >=19)
        {
            bool moved = true;
            int counter = 0;
            while (moved)
            {
                moved = tetr.Move(Vector3.down);
                if (counter++ >= 30) // escape if bugged
                {
                    break;
                }
            }
        }
    }

    public void SpawnNextTetromino()
    {
        holdLocked = false;

        if (gameFinished)
            return;

        if (currentBag.Count < 7)
            FillBag();

        current = currentBag[0];
        currentBag.RemoveAt(0);

        preview.ShowNext(currentBag);

        InstantiateNewMino(current);
    }

    public bool HoldCheck()
    {
        return !holdLocked;
    }

    public void HoldExecute()
    {
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
    }

    public void UpdateLastMovementStatus(bool statusLeft, bool statusRight)
    {
        lastPieceMovementStatus[0] = statusLeft;
        lastPieceMovementStatus[1] = statusRight;
    }

    public int ClearFullLines()
    {
        int cleared = 0;
        int upperBound = gridHeight;
        List<int> toClearList = new List<int>();
        for (int y = 0; y < gridHeight; y++)
        {
            bool full = true;
            bool empty = true;
            for (int x = 0; x < gridWidth; x++)
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

        currentTop = upperBound - cleared;

        if (cleared == 0)
        {
            return 0;
        }

        if (cleared == upperBound)
        {
            fieldStatus.AddScorePerfectClear(cleared, level);
            modeManager.GetGarbagePerfectClear(is_2P);
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

            for (int x = 0; x < gridWidth; x++)
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
        modeManager.AddLinesCleared(cleared);

        return cleared;
    }

    public void AddGarbageToQueue(int toAdd)
    {
        garbageQueue += Mathf.Max(toAdd, 0);
    }

    public int RemoveGarbageFromQueue(int toRemove)
    {
        if (toRemove < 0)
        {
            return 0;
        }    
        int removed = Mathf.Min(toRemove, garbageQueue);
        garbageQueue -= removed;
        return toRemove - removed;
    }

    public void RaiseGarbage()
    {
        if (garbageQueue > 0)
        {
            int garbageSpawned = Mathf.Min(garbageQueue, GameConsts.maxGarbageSpawn);
            RaiseGarbage(garbageSpawned);
            garbageQueue -= garbageSpawned;
        }
    }

    private int RaiseGarbage(int lines)
    {
        lines = Mathf.Max(Mathf.Min(lines, GameConsts.maxGarbageSpawn), 0);

        if (lines == 0)
        {  
            return 0; 
        }

        for (int y = currentTop - 1; y >= 0; y--)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                grid[x, y + lines] = grid[x, y];
                grid[x, y] = null;
                if (grid[x, y + lines] != null)
                {
                    grid[x, y + lines].position += Vector3.up * lines;
                }
            }
        }
        currentTop += lines;

        int holeCol = PlaceGarbageLineRand();
        for (int y = 1; y < lines; y++)
        {
            holeCol = PlaceGarbageLineRand(y, holeCol);
        }

        return lines;
    }

    private int PlaceGarbageLineRand(int y, int holeCol)
    {
        holeCol = Random.value < 0.7 ? holeCol : Random.Range(0, gridWidth - 1);
        PlaceGarbageLine(y, holeCol);
        return holeCol;
    }

    private int PlaceGarbageLineRand()
    {
        int holeCol = Random.Range(0, gridWidth - 1);
        PlaceGarbageLine(0, holeCol);
        return holeCol;
    }

    private void PlaceGarbageLine(int y, int holeColumn)
    {
        for (int x = 0; x < gridWidth; x++)
        {
            if (x == holeColumn)
            {
                Destroy(grid[x, y]?.gameObject);
                grid[x, y] = null;
                continue;
            }

            GameObject block = Instantiate(blockPrefab, CellToWorld(new Vector2Int(x, y)), Quaternion.identity);
            grid[x, y] = block.transform;
        }
    }

    private void Testing() // for debug testing
    {
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            Debug.Log("Adding Garbage");
            garbageQueue++;
        }
    }

    public void AddScore(int toAdd)
    {
        fieldStatus.AddScore(toAdd);
    }

    public void LineClearHandling(int linesCleared, int tSpinStatus)
    {
        bool backToBack = fieldStatus.BackToBack;
        int combo = fieldStatus.Combo;
        fieldStatus.AddScoreBonus(linesCleared, tSpinStatus, level);
        modeManager.GetGarbage(is_2P, linesCleared, tSpinStatus, backToBack, combo);
    }

    public void GameOver()
    {
        countDownText.text = "GAME OVER";
        countDownText.gameObject.SetActive(true);
        Time.timeScale = 0f;
        gameFinished = true;
    }

    public void GameCleared(string message) // shows message (time)
    {
        countDownText.text = $"GAME CLEARED\n{message}";
        countDownText.gameObject.SetActive(true);
        Time.timeScale = 0f;
        gameFinished = true;
    }

    public void GameCleared() // shows score
    {
        countDownText.text = $"GAME SCORE\n{fieldStatus.GetScore().ToString("#,#")}";
        countDownText.gameObject.SetActive(true);
        Time.timeScale = 0f;
        gameFinished = true;
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
