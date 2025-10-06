using UnityEngine;

[RequireComponent(typeof(Tetromino))]
public class InFieldLogic : MonoBehaviour
{
    [SerializeField] private GameObject ghostPrefab;
    private float multiplier = 1f;
    private float fallTime = 0.5f;
    private GameObject ghostGO;
    private GhostPiece ghost;

    private float previousTime;
    private int lowestY = 25;
    private int movementCount = 0;
    Tetromino tetr;

    public enum TetrominoType
    {
        I, O, T, J, L, S, Z
    }

    void Awake()
    {
        tetr = GetComponent<Tetromino>();
    }

    void OnEnable()
    {
        previousTime = Time.time;
        CreateGhost();
    }

    void OnDisable()
    {
        DestroyGhost();
    }

    void Update()
    {
        if (GameManager.isGameOver)
        {
            return;
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            multiplier = 20f;
        }
        else
        {
            multiplier = 1f;
        }

        //gravity
        float deltaTime = Time.time - previousTime;
        if (deltaTime > fallTime / multiplier)
        {
            bool dropped = tetr.Move(Vector3.down);
            if (dropped)
            {
                previousTime = Time.time;
                if (tetr.transform.localPosition.y < lowestY)
                {
                    lowestY = (int)tetr.transform.localPosition.y;
                }
            }
            else if (deltaTime > fallTime)
            {
                tetr.LockTetromino();
                previousTime = Time.time;
            }
        }

        // shift left
        if (Input.GetKeyDown(KeyCode.LeftArrow)) 
        {
            bool moved = tetr.Move(Vector3.left);
            if (moved)
            {
                PostActions();
            }
        }

        // shift right
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            bool moved = tetr.Move(Vector3.right);
            if (moved)
            {
                PostActions();
            }
        }

        // rotate left
        if (Input.GetKeyDown(KeyCode.A))
        {
            bool rotated = tetr.Rotate(1);
            if (rotated)
            {
                PostRotations();
            }
        }

        // rotate right
        if (Input.GetKeyDown(KeyCode.S))
        {
            bool rotated = tetr.Rotate(-1);
            if (rotated)
            {
                PostRotations();
            }
        }

        // harddrop
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            tetr.HardDropAndLock();
        }

        // hold
        if (Input.GetKeyDown(KeyCode.Space))
        {
            tetr.Hold();
        }
    }

    private bool PostActions()
    {
        movementCount++;
        bool grounded = tetr.CheckGround();
        if (grounded)
        {
            previousTime = Time.time;
        }
        return grounded;
    }

    private void PostRotations()
    {
        bool grounded = PostActions();
        if (lowestY > tetr.transform.position.y)
        {
            lowestY = (int)tetr.transform.position.y;
            movementCount = 0;
        }
        else if (movementCount >= GameConsts.maxDelayMovement && grounded)
        {
            tetr.LockTetromino();
        }
    }

    void CreateGhost()
    {
        if (ghostPrefab == null || ghostGO != null)
        {
            return;
        }

        ghostGO = Instantiate(ghostPrefab, transform.position, transform.rotation);
        ghost = ghostGO.GetComponent<GhostPiece>();
        if (ghost != null) ghost.Initialize(tetr);
    }

    void DestroyGhost()
    {
        if (ghostGO != null) 
        {
            Destroy(ghostGO);
        }
        ghostGO = null;
        ghost = null;
    }
}
