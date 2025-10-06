using Unity.VisualScripting;
using UnityEngine;

public class GhostPiece : MonoBehaviour
{
    private Tetromino target;

    public void Initialize(Tetromino tetromino)
    {
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
            Vector2 pos = Round(block.position + direction);
            if (!InsideGrid(pos) || GameManager.grid[(int)pos.x, (int)pos.y] != null)
            {
                return false;
            }
        }
        return true;
    }

    private Vector2 Round(Vector2 pos)
    {
        return new Vector2(Mathf.Round(pos.x), Mathf.Round(pos.y));
    }

    private bool InsideGrid(Vector2 pos)
    {
        return ((int)pos.x >= 0 && (int)pos.x < GameManager.width &&
                (int)pos.y >= 0 && (int)pos.y < GameManager.height);
    }    
}
