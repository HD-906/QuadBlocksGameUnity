using UnityEngine;

public class GarbageHandler : MonoBehaviour
{
    [SerializeField] private GameObject blockPrefab;
    [SerializeField] private GameManager gameManager;
    private int garbageQueue = 0;
    public int MaxGarbage { get; } = GameConsts.maxGarbage;
    public int MaxGarbageSpawn { get; } = GameConsts.maxGarbageSpawn;
    public int currentTop = 0;
    public event System.Action<int> OnChanged;
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

    public const int gridWidth = GameConsts.GridWidth;
    public const int gridHeight = GameConsts.GridHeight;

    public void AddGarbageToQueue(int toAdd)
    {
        GarbageQueue += Mathf.Max(toAdd, 0);
    }

    public int RemoveGarbageFromQueue(int toRemove)
    {
        if (toRemove < 0)
        {
            return 0;
        }
        int removed = Mathf.Min(toRemove, GarbageQueue);
        GarbageQueue -= removed;
        return toRemove - removed;
    }

    public void RaiseGarbage(bool sticky)
    {
        if (GarbageQueue > 0)
        {
            int garbageSpawned = Mathf.Min(GarbageQueue, MaxGarbageSpawn);
            RaiseGarbage(garbageSpawned, sticky);
            GarbageQueue -= garbageSpawned;
        }
    }

    private int RaiseGarbage(int lines, bool sticky)
    {
        lines = Mathf.Clamp(lines, 0, MaxGarbageSpawn);

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

        int holeCol = PlaceGarbageLineRand(0);
        for (int y = 1; y < lines; y++)
        {
            holeCol = sticky 
                ? PlaceGarbageLineRandSticky(y, holeCol)
                : PlaceGarbageLineRand(y);
        }

        return lines;
    }

    private int PlaceGarbageLineRand(int y)
    {
        int holeCol = Random.Range(0, gridWidth - 1);
        PlaceGarbageLine(y, holeCol);
        return holeCol;
    }

    private int PlaceGarbageLineRandSticky(int y, int holeCol)
    {
        holeCol = Random.value < 0.7 ? holeCol : Random.Range(0, gridWidth - 1);
        PlaceGarbageLine(y, holeCol);
        return holeCol;
    }

    private void PlaceGarbageLine(int y, int holeColumn)
    {
        var grid = gameManager.grid;
        for (int x = 0; x < gridWidth; x++)
        {
            if (x == holeColumn)
            {
                Destroy(grid[x, y]?.gameObject);
                grid[x, y] = null;
                continue;
            }

            GameObject block = Instantiate(blockPrefab, gameManager.CellToWorld(new Vector2Int(x, y)), Quaternion.identity);
            grid[x, y] = block.transform;
        }
    }
}
