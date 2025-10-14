using System;
using System.Collections.Generic;
using UnityEngine;

public class Tetromino : MonoBehaviour
{
    int orient = 0;
    public GameObject blockPrefab;
    private bool rotatedFifthKick = false;
    private bool locking = false;

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

        int selectedCount = 0;
        foreach (var offset in selectedList)
        {
            transform.position = originalPosition + offset.ToVector2();

            if (IsValidMove())
            {
                orient = nextOrient;
                rotatedFifthKick = (selectedCount == 4);
                return true;
            }
            selectedCount++;
        }

        transform.position = originalPosition;
        transform.Rotate(0, 0, -90 * dir);
        return false;
    }

    public int HardDropAndLock(bool lastRotated)
    {
        bool moved = true;
        int score = -1;
        while (moved)
        {
            moved = Move(Vector3.down);
            score++;
            if (score >= 30) // escape if bugged
            {
                break;
            }
        }
        if (score > 0)
        {
            lastRotated = false;
        }
        LockTetromino(lastRotated);

        return score;
    }

    public void LockTetromino(bool lastRotated)
    {
        if (locking)
        {
            return;
        }
        locking = true;
        int tSpin = 0;
        if (type == TetrominoType.T && lastRotated)
        {
            tSpin = CheckTspin();
        } // 0, 1, 2 -> No T-spin, T-spin mini, full T-spin
         
        AddToGrid();
        int linesCleared = gameManager.ClearFullLines();
        gameManager.AddScoreBonus(linesCleared, tSpin);
        gameManager.SpawnNextTetromino();
        enabled = false;
    }

    public void Locked()
    {
        locking = false;
    }

    public void Hold()
    {
        bool holdSuccess = gameManager.HoldCheck();
        if (holdSuccess)
        {
            Destroy(gameObject);
            gameManager.HoldExecute();
        }
    }

    public bool IsValidMove()
    {
        foreach (Transform block in transform)
        {
            Vector2Int cell = gameManager.WorldToCell(block.position);
            if (!GridEmpty(cell))
            {
                return false;
            }
        }
        return true;
    }

    bool GridEmpty(Vector2Int cell)
    {
        return GridValid(cell) && !GridOccupied(cell);
    }

    bool GridValid(Vector2Int cell)
    {
        return (cell.x >= 0 && cell.x < GameManager.width && 
                cell.y >= 0 && cell.y < GameManager.height);
    }

    bool GridOccupied(Vector2Int cell)
    {
        return gameManager.grid[cell.x, cell.y] != null;
    }

    void AddToGrid()
    {
        bool overflow = true;

        foreach (Transform child in transform)
        {
            Vector2Int cell = gameManager.WorldToCell(child.position);
            Vector3 world = gameManager.CellToWorld(cell);
            GameObject block = Instantiate(blockPrefab, world, Quaternion.identity);

            // Copy colour
            var srcSR = child.GetComponent<SpriteRenderer>();
            var dstSR = block.GetComponent<SpriteRenderer>();
            if (srcSR != null && dstSR != null)
            {
                dstSR.color = srcSR.color;
            }

            gameManager.grid[cell.x, cell.y] = block.transform;

            if (cell.y <= 19)
            {
                overflow = false;
            }
        }

        if (overflow)
        {
            gameManager.GameOver();
        }

        Destroy(gameObject);
    }

    int CheckTspin() // 0, 1, 2 -> No T-spin, T-spin mini, full T-spin
    {
        Vector2Int mainCell = Vector2Int.zero;
        foreach (Transform child in transform)
        {
            Vector2Int cell = gameManager.WorldToCell(child.position);
            mainCell = cell;
            break;
        }

        Vector2Int orientDir = Vector2Int.zero;
        Vector2Int sideDir = Vector2Int.zero;
        switch (orient)
        {
            case 0:
                orientDir = Vector2Int.up;
                sideDir = Vector2Int.left;
                break;
            case 1:
                orientDir = Vector2Int.right;
                sideDir = Vector2Int.up;
                break;
            case 2:
                orientDir = Vector2Int.down;
                sideDir = Vector2Int.left;
                break;
            case 3:
            default:
                orientDir = Vector2Int.left;
                sideDir = Vector2Int.up;
                break;
        }

        int frontCheck = 
            Convert.ToInt32(!GridEmpty(mainCell + orientDir + sideDir)) + 
            Convert.ToInt32(!GridEmpty(mainCell + orientDir - sideDir));
        int backCheck = 
            Convert.ToInt32(!GridEmpty(mainCell - orientDir + sideDir)) + 
            Convert.ToInt32(!GridEmpty(mainCell - orientDir - sideDir));

        if (frontCheck + backCheck < 3)
        {
            return 0;
        }
        
        if (!rotatedFifthKick && frontCheck == 1)
        {
            return 1;
        }

        return 2;
    }
}

public static class VectorExtensions
{
    public static Vector2 ToVector2(this Vector2Int v)
    {
        return new Vector2(v.x, v.y);
    }
}
