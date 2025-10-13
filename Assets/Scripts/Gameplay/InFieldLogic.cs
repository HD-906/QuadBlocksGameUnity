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

    private KeyCode moveLeft;
    private KeyCode moveRight;
    private KeyCode softDrop;
    private KeyCode hardDrop;
    private KeyCode rotateRight;
    private KeyCode rotateLeft;
    private KeyCode hold;

    private bool dasTriggeredLeft = false;
    private bool dasTriggeredRight = false;
    private bool cancelLeft = false;
    private bool cancelRight = false;
    private bool firstLeftDown = false;
    private bool firstRightDown = false;
    private float fallTime = 0.5f;
    private GameObject ghostGO;
    private GhostPiece ghost;

    private float previousTime = -1;
    private float previousLeftDownTime = -1;
    private float previousRightDownTime = -1;
    private float previousLeftArrTime = -1;
    private float previousRightArrTime = -1;
    private int lowestY = 25;
    private int movementCount = 0;
    Tetromino tetr;

    public GameManager gameManager;

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
        arr = cfg.arr / cfg.frameRate;
        das = cfg.das / cfg.frameRate;
        dcd = cfg.dcd / cfg.frameRate;
        sdf = cfg.sdf;

        multiplier = gravity;

        moveLeft = cfg.moveLeft;
        moveRight = cfg.moveRight;
        softDrop = cfg.softDrop;
        hardDrop = cfg.hardDrop;
        rotateRight = cfg.rotateRight;
        rotateLeft = cfg.rotateLeft;
        hold = cfg.hold;
    }

    void OnEnable()
    {
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

        if (Input.GetKey(softDrop))
        {
            multiplier = gravity * sdf;
        }
        else
        {
            multiplier = gravity;
        }

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
                gameManager.UpdateLastMovementStatus(cancelLeft, cancelRight);
                tetr.LockTetromino();
                previousTime = Time.time;
            }
        }

        if (!(Input.GetKey(moveLeft) && Input.GetKey(moveRight)))
        {
            SetCancelStatus(false, false);
        }
        else if (Input.GetKeyDown(moveLeft) && Input.GetKey(moveRight))
        {
            SetCancelStatus(false, true);
        }
        else if (Input.GetKey(moveLeft) && Input.GetKeyDown(moveRight))
        {
            SetCancelStatus(true, false);
        }

        HandleHorizontalArr
        (
            moveLeft,
            ref previousLeftDownTime,
            ref previousLeftArrTime,
            ref dasTriggeredLeft,
            ref firstLeftDown,
            Vector3.left,
            cancelLeft
        );

        HandleHorizontalArr
        (
            moveRight,
            ref previousRightDownTime, 
            ref previousRightArrTime, 
            ref dasTriggeredRight, 
            ref firstRightDown,
            Vector3.right,
            cancelRight
        );

        if (Input.GetKeyDown(rotateLeft))
        {
            bool rotated = tetr.Rotate(1);
            if (rotated)
            {
                PostRotations();
            }
        }

        if (Input.GetKeyDown(rotateRight))
        {
            bool rotated = tetr.Rotate(-1);
            if (rotated)
            {
                PostRotations();
            }
        }

        if (Input.GetKeyDown(hardDrop))
        {
            gameManager.UpdateLastMovementStatus(cancelLeft, cancelRight);
            tetr.HardDropAndLock();
        }

        if (Input.GetKeyDown(hold))
        {
            gameManager.UpdateLastMovementStatus(cancelLeft, cancelRight);
            tetr.Hold();
        }
    }

    private void HandleHorizontalArr(KeyCode key, ref float previousDownTime, ref float previousArrTime, ref bool dasTriggered, ref bool firstDown, Vector3 v, bool cancel)
    {
        if (Input.GetKeyDown(key))
        {
            previousDownTime = Time.time;
            MoveHorizontal(v);
            firstDown = true;
        }

        if (Input.GetKey(key))
        {
            if (previousDownTime < 0) // To Avoid Unexpected bug
            {
                previousDownTime = Time.time;
            }

            if (previousArrTime < 0) // To Avoid Unexpected bug
            {
                previousArrTime = Time.time;
            }

            float deltaTime = Time.time - previousDownTime;
            if (deltaTime > (firstDown ? das : dcd))
            {
                if (!dasTriggered)
                {
                    if (!cancel)
                    {
                        MoveHorizontal(v);
                    }
                    dasTriggered = true;
                    previousArrTime = Time.time;
                }
                else if (Time.time - previousArrTime > arr)
                {
                    if (!cancel)
                    {
                        MoveHorizontal(v);
                    }
                    previousArrTime = Time.time;
                }
            }
        }

        if (Input.GetKeyUp(key))
        {
            previousDownTime = -1;
            previousArrTime = -1;
            dasTriggered = false;
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

    public void SetCancelStatus(bool statusLeft, bool statusRight)
    {
        cancelLeft = statusLeft;
        cancelRight = statusRight;
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
