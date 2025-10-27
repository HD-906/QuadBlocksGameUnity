using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> currentBag = new List<GameObject>();
    [SerializeField] private GameObject[] tetrominoPrefabs;
    [SerializeField] private GameObject garbageQueuePrefab;
    [SerializeField] private GameObject current, onHold;
    [SerializeField] private bool holdLocked = false;
    [SerializeField] public Transform[,] grid;
    [SerializeField] public bool gameOverTriggered = false;
    [SerializeField] private PreviewController preview;
    [SerializeField] Transform fieldOrigin;
    [SerializeField] float cellSize = GameConsts.CellSize;
    [SerializeField] public int level = 1;
    [SerializeField] private InFieldStatus fieldStatus;
    [SerializeField] TMP_Text centralText, centralText1, centralText2;
    [SerializeField] TMP_Text bottomInfo;
    [SerializeField] AttackEffectHandler attackEffect;

    [HideInInspector] public ModeManager modeManager; // Set in ModeManager.Awake()
    [HideInInspector] public bool is_2P;

    public static bool gameEnded = false;
    private Vector2Int spawnPosition = GameConsts.SpawnCell;

    public const int gridWidth = GameConsts.GridWidth, gridHeight = GameConsts.GridHeight;

    private bool[] lastPieceMovementStatus = { false, false };
    public bool lastPieceHarddroped = true;
    private float holdTime = GameConsts.holdTime;

    private float previousRTime = -1f, previousFTime = -1f;
    private KeyCode restart, forfeit = GameConsts.forfeit;

    private float countDown = GameConsts.startCountdown;
    public bool started = false;

    public GarbageHandler garbageHandler;
    public bool sticky = true;
    private bool perfectClear = false;

    [SerializeField] public ControlConfig ctrlCfg;

    void Awake()
    {
        gameEnded = false;
        if (fieldOrigin == null)
        {
            fieldOrigin = transform;
        }

        grid = new Transform[gridWidth, gridHeight];
        FillBag();
        preview.ShowNext(currentBag);
        preview.ShowHold(onHold);

        restart = ctrlCfg.restart;
    }

    private void Start()
    {
        centralText.fontSize = 42;
        centralText.gameObject.SetActive(true);
        centralText2.gameObject.SetActive(false);
        Time.timeScale = 1f;
        gameOverTriggered = false;
        bottomInfo.text = $"Press and hold {restart} to restart\n" +
                          $"Press and hold esc to end game";
    }

    void Update()
    {
        if (!started)
        {
            countDown -= Time.deltaTime;
            centralText.text = $"{(int)(countDown + 1)}";
            if (countDown <= 0)
            {
                SpawnNextTetromino();
                started = true;
                centralText.text = "";
                centralText.gameObject.SetActive(false);
            }
        }

        if (gameEnded && !gameOverTriggered)
        {
            centralText.fontSize = 14;
            centralText.text = "YOU WIN!";
            centralText.gameObject.SetActive(true);
            Time.timeScale = 0f;
        }

        if (gameEnded)
        {
            bottomInfo.text = $"Press R to restart\n" +
                              $"Press esc to end game";
            return;
        }

        ExecuteWhenHeld(restart, ref previousRTime, RestartGame, "Restarting", true);
        ExecuteWhenHeld(forfeit, ref previousFTime, ForfeitGame, "Exiting", false);
    }

    private void ExecuteWhenHeld(KeyCode key, ref float previousTime, System.Action act, string str, bool first)
    {
        TMP_Text tmpText = first ? centralText1 : centralText2;

        if (gameEnded)
        {
            tmpText.gameObject.SetActive(false);
            return;
        }

        if (Input.GetKeyDown(key))
        {
            previousTime = Time.time;
            tmpText.text = str;
            tmpText.fontSize = 8;
            tmpText.gameObject.SetActive(true);
        }

        if (previousTime < 0)
        {
            return;
        }

        if (Input.GetKey(key))
        {
            float timeHeld = Time.time - previousTime;
            if (timeHeld > holdTime)
            {
                act();
                tmpText.text = "";
                tmpText.fontSize = 8;
                tmpText.gameObject.SetActive(false);
            }
            else
            {
                tmpText.fontSize = 6 + 10 * timeHeld / holdTime;
            }
        }

        if (Input.GetKeyUp(key))
        {
            tmpText.text = "";
            tmpText.fontSize = 8;
            tmpText.gameObject.SetActive(false);
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
        level = Mathf.Clamp(level, 1, 30);
        var pos = CellToWorld(spawnPosition);
        var newMino = Instantiate(prefabObj, pos, Quaternion.identity);
        Tetromino tetr = newMino.GetComponent<Tetromino>();

        tetr.gameManager = this;
        newMino.transform.SetParent(fieldOrigin, true);


        var logic = newMino.GetComponent<TetroLogic>();
        if (logic != null)
        {
            logic.gameManager = this;
            logic.SetLeftRightCancelStatus(lastPieceMovementStatus[0], lastPieceMovementStatus[1]);
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

        if (gameEnded)
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

        garbageHandler.currentTop = upperBound - cleared;

        if (cleared == 0)
        {
            return 0;
        }

        if (cleared == upperBound) // ================= Perfect Clear
        {
            fieldStatus.AddScorePerfectClear(cleared, level);
            perfectClear = true; // Wait 
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

    public int RemoveGarbageFromQueue(int toRemove)
    {
        int toSend = garbageHandler.RemoveGarbageFromQueue(toRemove);
        if (toSend > 0)
        {
            attackEffect.SpawnAttack(toSend);
        }
        return toSend;
    }

    public void AddGarbageToQueue(int toAdd)
    {
        garbageHandler.AddGarbageToQueue(toAdd);
    }

    public void AddGarbageToQueueDelayed(int toAdd)
    {
        garbageHandler.AddGarbageToQueueDelayed(toAdd);
    }

    public void RaiseGarbage()
    {
        garbageHandler.RaiseGarbage(sticky);
    }

    public void EnableMessageEffect(int lineClear, int tSpinStatus, bool backToBack, int comboNum, bool perfectClr)
    {
        attackEffect.EnableMessageEffect(lineClear, tSpinStatus, backToBack, comboNum, perfectClr);
    }

    public void AddScore(int toAdd)
    {
        fieldStatus.AddScore(toAdd);
    }

    public void LineClearHandling(int linesCleared, int tSpinStatus)
    {
        bool backToBack = fieldStatus.BackToBack;
        int combo = fieldStatus.Combo;
        fieldStatus.AddScoreBonus(linesCleared, tSpinStatus, level); // Will also update B2B status in fieldStatus
        modeManager.GetGarbage(is_2P, linesCleared, tSpinStatus, backToBack && fieldStatus.BackToBack, combo, perfectClear);
        perfectClear = false;
    }

    public void GameOver()
    {
        centralText.fontSize = 14;
        centralText.text = "GAME OVER";
        centralText.gameObject.SetActive(true);
        Time.timeScale = 0f;
        gameOverTriggered = true;
        gameEnded = true;
    }

    public void GameCleared(string message) // shows message (time)
    {
        centralText.fontSize = 14;
        centralText.text = $"GAME CLEARED\n{message}";
        centralText.gameObject.SetActive(true);
        Time.timeScale = 0f;
        gameOverTriggered = true;
        gameEnded = true;
    }

    public void GameCleared() // shows score
    {
        centralText.fontSize = 14;
        centralText.text = $"GAME SCORE\n{fieldStatus.GetScore().ToString("#,#")}";
        centralText.gameObject.SetActive(true);
        Time.timeScale = 0f;
        gameOverTriggered = true;
        gameEnded = true;
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
