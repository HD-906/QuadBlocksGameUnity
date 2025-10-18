using UnityEngine;

public class GarbageHandler : MonoBehaviour
{
    private GameObject blockPrefab;
    private GameManager gameManager;
    private Transform[,] grid;
    public int garbageQueue = 0;
    public int currentTop = 0;

    public const int gridWidth = GameConsts.GridWidth;
    public const int gridHeight = GameConsts.GridHeight;

    public GarbageHandler(GameManager gameManager)
    {
        this.gameManager = gameManager;
        grid = gameManager.grid;
        blockPrefab = gameManager.BlockPrefab;
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
        lines = Mathf.Clamp(lines, 0, GameConsts.maxGarbageSpawn);

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

            GameObject block = Instantiate(blockPrefab, gameManager.CellToWorld(new Vector2Int(x, y)), Quaternion.identity);
            grid[x, y] = block.transform;
        }
    }
}
