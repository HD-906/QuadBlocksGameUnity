using UnityEngine;
using System.Collections.Generic;
using System.Net;

public class Tetromino : MonoBehaviour
{
    int orient = 0;
    public GameObject blockPrefab;

    public enum TetrominoType
    {
        I, O, T, J, L, S, Z
    }

    public TetrominoType type;

    private List<Vector2Int> RotationListA = new() {
        new (0, 0), new (-1, 0), new (-1, 1), new (0, -2), new (-1, -2)
    };

    private List<Vector2Int> RotationListB = new() {
        new (0, 0), new (1, 0), new (1, -1), new (0, 2), new (1, 2)
    };

    private List<Vector2Int> RotationListC = new() {
        new (0, 0), new (1, 0), new (1, 1), new (0, -2), new (1, -2)
    };

    private List<Vector2Int> RotationListD = new() {
        new (0, 0), new (-1, 0), new (-1, -1), new (0, 2), new (-1, 2)
    };

    private List<Vector2Int> RotationListIA = new() {
        new (1, 0), new (-1, 0), new (2, 0), new (-1, -1), new (2, 2)
    };

    private List<Vector2Int> RotationListIB = new() {
        new (-1, 0), new (1, 0), new (-2, 0), new (1, 1), new (-2, -2)
    };

    private List<Vector2Int> RotationListIC = new() {
        new (0, -1), new (-1, -1), new (2, -1), new (-1, 1), new (2, -2)
    };

    private List<Vector2Int> RotationListID = new() {
        new (0, 1), new (1, 1), new (-2, 1), new (1, -1), new (-2, 2)
    };

    public GameManager gameManager; 

    public bool CheckGround()
    {
        transform.position += Vector3.down;
        bool ground = !IsValidMove();
        transform.position -= Vector3.down;
        return ground;
    }

    public bool Move(Vector3 direction)
    {
        transform.position += direction;
        if (!IsValidMove())
        {
            transform.position -= direction;
            return false;
        }
        return true;
    }

    public bool Rotate(int dir)
    {
        if (type == TetrominoType.O)
            return true;

        Vector2 originalPosition = transform.position;
        transform.Rotate(0, 0, 90 * dir);
        int nextOrient = (orient - dir + 4) % 4;
        List<Vector2Int> selectedList;

        if (type == TetrominoType.I)
        {
            if (orient + nextOrient == 3)
            {
                if (nextOrient >= 2)
                {
                    selectedList = RotationListIC;
                }
                else
                {
                    selectedList = RotationListID;
                }
            }
            else
            {
                if (nextOrient == 1 || nextOrient == 2)
                {
                    selectedList = RotationListIA;
                }
                else
                {
                    selectedList = RotationListIB;
                }
            }
        }
        else
        {
            if (orient % 2 == 0)
            {
                if (nextOrient == 1)
                {
                    selectedList = RotationListA;
                }
                else
                {
                    selectedList = RotationListC;
                }
            }
            else
            {
                if (orient == 1)
                {
                    selectedList = RotationListB;
                }
                else
                {
                    selectedList = RotationListD;
                }
            }
        }

        foreach (var offset in selectedList)
        {
            transform.position = originalPosition + offset.ToVector2();

            if (IsValidMove())
            {
                orient = nextOrient;
                return true;
            }
        }

        transform.position = originalPosition;
        transform.Rotate(0, 0, -90 * dir);
        return false;
    }

    public void HardDropAndLock()
    {
        bool moved = true;
        while (moved)
        {
            moved = Move(Vector3.down);
        }
        LockTetromino();
    }

    public void LockTetromino()
    {
        AddToGrid();
        int linesCleared = gameManager.ClearFullLines();
        gameManager.SpawnNextTetromino();
        enabled = false;
    }

    public void Hold()
    {
        bool holdSuccess = gameManager.HoldCurrent();
        if (holdSuccess)
        {
            Destroy(gameObject);
        }
    }

    public bool IsValidMove()
    {
        foreach (Transform block in transform)
        {
            Vector2 pos = Round(block.position);
            if (!InsideGrid(pos))
            {
                return false;
            }

            if (gameManager.grid[(int)pos.x, (int)pos.y] != null)
            {
                return false; 
            }
        }
        return true;
    }

    Vector2 Round(Vector2 pos)
    {
        return new Vector2(Mathf.Round(pos.x), Mathf.Round(pos.y));
    }

    bool InsideGrid(Vector2 pos)
    {
        return ((int)pos.x >= 0 && (int)pos.x < GameManager.width && 
                (int)pos.y >= 0 && (int)pos.y < GameManager.height);
    }

    void AddToGrid()
    {
        bool overflow = true;
        if (blockPrefab == null)
        {
            Debug.LogError("blockPrefab is not assigned in Tetromino!");
            return;
        }

        foreach (Transform child in transform)
        {
            Vector2Int pos = Vector2Int.RoundToInt(child.position);
            GameObject block = Instantiate(blockPrefab, pos.ToVector2(), Quaternion.identity);

            // Render colour to match the original block
            SpriteRenderer originalSR = child.GetComponent<SpriteRenderer>();
            SpriteRenderer newSR = block.GetComponent<SpriteRenderer>();
            if (originalSR != null && newSR != null)
            {
                newSR.color = originalSR.color;
            }

            gameManager.grid[pos.x, pos.y] = block.transform;

            if (pos.y <= 19)
            {
                overflow = false;
            }
        }

        if (overflow)
        {
            FindFirstObjectByType<GameManager>().GameOver();
        }

        Destroy(gameObject);
    }
}

public static class VectorExtensions
{
    public static Vector2 ToVector2(this Vector2Int v)
    {
        return new Vector2(v.x, v.y);
    }
}
