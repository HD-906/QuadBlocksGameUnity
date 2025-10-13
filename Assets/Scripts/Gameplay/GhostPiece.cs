using UnityEngine;

public class GhostPiece : MonoBehaviour
{
    private Tetromino target;
    public GameManager gameManager;

    public void Init(GameManager manager, Tetromino tetromino)
    {
        gameManager = manager;
        target = tetromino;
    }

    private void LateUpdate()
    {
        if (target == null)
        {
            return;
        }
        transform.position = target.transform.position;
        transform.rotation = target.transform.rotation;

        while (IsValidMove(Vector3.down))
        {
            transform.position += Vector3.down;
        }
    }


    private bool IsValidMove(Vector3 direction)
    {
        foreach (Transform block in transform)
        {
            Vector2Int cell = gameManager.WorldToCell(block.position + direction);
            if (!InsideGrid(cell) || gameManager.grid[cell.x, cell.y] != null)
            {
                return false;
            }
        }
        return true;
    }

    private bool InsideGrid(Vector2 pos)
    {
        return ((int)pos.x >= 0 && (int)pos.x < GameManager.width &&
                (int)pos.y >= 0 && (int)pos.y < GameManager.height);
    }    
}
