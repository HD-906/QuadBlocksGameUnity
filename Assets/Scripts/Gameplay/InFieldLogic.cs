using UnityEngine;

[RequireComponent(typeof(Tetromino))]
public class InFieldLogic : MonoBehaviour
{
    [SerializeField] private GameObject ghostPrefab;
    private float multiplier;
    private float gravity;
    private float arr;
    private float das;
    private float dcd;
    private int sdf;
    private bool dasTriggeredLeft = false;
    private bool dasTriggeredRight = false;
    private float fallTime = 0.5f;
    private GameObject ghostGO;
    private GhostPiece ghost;

    private float previousTime;
    private float previousLeftDownTime;
    private float previousRightDownTime;
    private float previousLeftArrTime;
    private float previousRightArrTime;
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

    void Start()
    {
        var cfg = Bootstrap.I.config;

        gravity = cfg.gravity;
        arr = cfg.arr;
        das = cfg.das;
        dcd = cfg.dcd;
        sdf = cfg.sdf;

        previousTime = -1;
        previousLeftDownTime = -1;
        previousRightDownTime = -1;
        previousLeftArrTime = -1;
        previousRightArrTime = -1;

        multiplier = gravity;
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
            multiplier = gravity * sdf;
        }
        else
        {
            multiplier = gravity;
        }

        //gravity
        if (previousTime < 0)
        {
            previousTime = Time.time;
        }

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
            previousLeftDownTime = Time.time;
            MoveHorizontal(Vector3.left);
        }

        // shift right
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            previousRightDownTime = Time.time;
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

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            if (previousLeftDownTime < 0) // To Avoid Unexpected bug
            {
                previousLeftDownTime = Time.time;
            }

            if (previousLeftArrTime < 0) // To Avoid Unexpected bug
            {
                previousLeftArrTime = Time.time;
            }

            float deltaLeftTime = Time.time - previousLeftDownTime;
            if (deltaLeftTime > das)
            {
                if (!dasTriggeredLeft)
                {
                    MoveHorizontal(Vector3.left);
                    dasTriggeredLeft = true;
                    previousLeftArrTime = Time.time;
                }
                else if (Time.time - previousLeftArrTime > arr)
                {
                    MoveHorizontal(Vector3.left);
                    previousLeftArrTime = Time.time;
                }
            }
        }

        if (Input.GetKeyUp(KeyCode.LeftArrow))
        {
            previousLeftDownTime = -1;
            previousLeftArrTime = -1;
            dasTriggeredLeft = false;
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {

        }
    }

    private void MoveHorizontal(Vector3 v)
    {
        bool moved = tetr.Move(v);
        if (moved)
        {
            PostActions();
        }
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
