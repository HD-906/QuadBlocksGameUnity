using System.Collections;
using System.Linq;
using UnityEngine;

public class GarbageHandler : MonoBehaviour
{
    [SerializeField] private GameObject blockPrefab;
    [SerializeField] private GameManager gameManager;
    private int garbageQueue = 0;
    public int MaxGarbage { get; } = GameConsts.maxGarbage;
    public int MaxGarbageSpawn { get; } = GameConsts.maxGarbageSpawn;
    public int currentTop = 0;
    public event System.Action<int> OnChanged; // Assigned in GarbageQueueUI.cs
    public int GarbageQueue
    {
        get => garbageQueue;
        private set
        {
            value = Mathf.Clamp(value, 0, MaxGarbage);
            if (value == garbageQueue) return;
            garbageQueue = value;
            OnChanged?.Invoke(value);
        }
    }
    private int garbageBuffer = 0;

    public const int gridWidth = GameConsts.GridWidth, gridHeight = GameConsts.GridHeight;

    public void AddGarbageToQueue(int value)
    {
        GarbageQueue += Mathf.Max(value, 0);
    }

    public void AddGarbageToQueueDelayed(int value)
    {
        garbageBuffer += Mathf.Max(value, 0);
        StartCoroutine(TransferAfterDelay(value));
    }

    private IEnumerator TransferAfterDelay(int value)
    {
        yield return new WaitForSeconds(GameConsts.garbageDelay);

        int transferable = Mathf.Min(value, garbageBuffer);
        if (transferable > 0)
        {
            AddGarbageToQueue(value);
            garbageBuffer -= transferable;
        }
    }

    public int RemoveGarbageFromQueue(int value)
    {
        if (value < 0)
        {
            return 0;
        }
        int removed = Mathf.Min(value, GarbageQueue);
        GarbageQueue -= removed;

        value -= removed;
        removed = Mathf.Min(value, garbageBuffer);
        garbageBuffer -= removed;
        return value - removed; // remaining to be sent
    }

    public void RaiseGarbage(bool drillerMode, bool raiseAll, bool sticky)
    {
        if (GarbageQueue > 0)
        {
            int garbageSpawned = drillerMode || raiseAll
                ? GarbageQueue
                : Mathf.Min(GarbageQueue, MaxGarbageSpawn);
            RaiseGarbage(garbageSpawned, drillerMode, sticky);
            GarbageQueue -= garbageSpawned;
        }
    }

    private int RaiseGarbage(int lines, bool drillerMode, bool sticky)
    {
        if (lines == 0)
        {
            return 0;
        }

        var grid = gameManager.grid;
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

        if (drillerMode)
        {
            for (int y = 0; y < lines; y++)
            {
                PlaceGarbageLineRandDriller(y);
            }
        }
        else
        {
            int holeCol = -1;
            for (int y = 0; y < lines; y++)
            {
                holeCol = PlaceGarbageLineRand(y, holeCol, sticky);
            }
        }

        return lines;
    }

    private int PlaceGarbageLineRand(int y, int holeCol, bool sticky)
    {
        if (!sticky || holeCol < 0 || Random.value > 0.7)
        {
            holeCol = Random.Range(0, gridWidth - 1);
        }
        PlaceGarbageLine(y, holeCol);
        return holeCol;
    }

    private void PlaceGarbageLineRandDriller(int y)
    {
        int[] holeCols = Enumerable.Range(0, 10)
                         .OrderBy(_ => UnityEngine.Random.value)
                         .Take(GameConsts.DrillerGarbageValue)
                         .ToArray();
        PlaceGarbageLine(y, holeCols);
    }

    private void PlaceGarbageLine(int y, params int[] holeColumns)
    {
        var grid = gameManager.grid;
        for (int x = 0; x < gridWidth; x++)
        {
            if (holeColumns.Contains(x))
            {
                Destroy(grid[x, y]?.gameObject);
                grid[x, y] = null;
                continue;
            }

            GameObject block = Instantiate(
                blockPrefab, 
                gameManager.CellToWorld(new Vector2Int(x, y)), 
                Quaternion.identity
            );
            grid[x, y] = block.transform;
        }
    }
}
