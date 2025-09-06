using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> currentBag = new List<GameObject>();
    [SerializeField] private GameObject[] tetrominoPrefabs;
    [SerializeField] private GameObject current;
    [SerializeField] private GameObject onHold;
    [SerializeField] public bool holdLocked = false;
    [SerializeField] public static Transform[,] grid;
    [SerializeField] public static bool isGameOver = false;

    private Vector2 spawnPosition = new Vector2(4, 18);

    public const int width = 10;
    public const int height = 25;

    void Start()
    {
        grid = new Transform[width, height];
        FillBag();
        SpawnNextTetromino();
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
        GameObject newMino = Instantiate(prefabObj, spawnPosition, Quaternion.identity);
        newMino.GetComponent<Tetromino>().gameManager = this;

        Tetromino tetroScript = newMino.GetComponent<Tetromino>();

        var logic = newMino.GetComponent<InFieldLogic>();
        if (logic != null)
        {
            logic.enabled = true;
        }

        int tryCount = 0;
        while (!tetroScript.IsValidMove())
        {
            if (tryCount >= 2)
            {
                GameOver();
            }
            newMino.transform.position += Vector3.up;
            tryCount++;
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
        return true;
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

    public void GameOver()
    {
        Debug.Log("GAME OVER!");
        Time.timeScale = 0f;
        isGameOver = true;
    }
}
