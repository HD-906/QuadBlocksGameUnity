using UnityEngine;

[RequireComponent(typeof(Tetromino))]
public class TetroLogic : MonoBehaviour
{
    [SerializeField] private GameObject ghostPrefab;
    private float accumulator = 0;
    private const float STEP = 1 / GameConsts.frameRate;
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
    private float lockDelayTime;
    private float harddropEnableTime = GameConsts.harddropEnableTime;
    private float startTime;
    private GameObject ghostGO;
    private GhostPiece ghost;

    private float previousTime = -1;
    private float previousLeftDownTime = -1;
    private float previousRightDownTime = -1;
    private float previousLeftArrTime = -1;
    private float previousRightArrTime = -1;
    private float softDropTime = -1;
    private int lowestY = 25;
    private int movementCount = 0;
    Tetromino tetr;

    private bool lastRotated = false;

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
        ControlConfig ctrlCfg = gameManager.ctrlCfg;

        gravity = 1 / getIntervalFromLevel();
        lockDelayTime = GameConsts.lockDelayTime;
        if (gameManager.level >= 20)
        {
            lockDelayTime -= (gameManager.level - 19f) / 30f;
        }
        arr = ctrlCfg.arr / ctrlCfg.frameRate;
        das = ctrlCfg.das / ctrlCfg.frameRate;
        dcd = ctrlCfg.dcd / ctrlCfg.frameRate;
        sdf = ctrlCfg.sdf;

        multiplier = gravity;

        moveLeft = ctrlCfg.moveLeft;
        moveRight = ctrlCfg.moveRight;
        softDrop = ctrlCfg.softDrop;
        hardDrop = ctrlCfg.hardDrop;
        rotateRight = ctrlCfg.rotateRight;
        rotateLeft = ctrlCfg.rotateLeft;
        hold = ctrlCfg.hold;

        startTime = Time.time;
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
        if (GameManager.gameEnded)
        {
            return;
        }

        accumulator += Time.deltaTime;

        if (Input.GetKeyDown(softDrop))
        {
            softDropTime = Time.time;
        }

        if (Input.GetKeyUp(softDrop))
        {
            softDropTime = -1;
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

        if (!(Input.GetKey(moveLeft) && Input.GetKey(moveRight)))
        {
            SetLeftRightCancelStatus(false, false);
        }
        else if (Input.GetKeyDown(moveLeft) && Input.GetKey(moveRight))
        {
            SetLeftRightCancelStatus(false, true);
        }
        else if (Input.GetKey(moveLeft) && Input.GetKeyDown(moveRight))
        {
            SetLeftRightCancelStatus(true, false);
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
            bool groundedPrev = tetr.CheckGround();
            bool rotated = tetr.Rotate(1);
            if (rotated)
            {
                bool grounded = PostRotations();
                if (!grounded && groundedPrev)
                {
                    previousTime = Time.time;
                }
            }
        }

        if (Input.GetKeyDown(rotateRight))
        {
            bool groundedPrev = tetr.CheckGround();
            bool rotated = tetr.Rotate(-1);
            if (rotated)
            {
                bool grounded = PostRotations();
                if (!grounded && groundedPrev)
                {
                    previousTime = Time.time;
                }
            }
        }

        if 
        (
            Input.GetKeyDown(hardDrop) && 
            (gameManager.lastPieceHarddroped || (Time.time - startTime > harddropEnableTime))
        )
        {
            gameManager.UpdateLastMovementStatus(cancelLeft, cancelRight);
            int score = tetr.HardDropAndLock(lastRotated) * 2;
            gameManager.AddScore(score);
        }

        if (Input.GetKeyDown(hold))
        {
            gameManager.UpdateLastMovementStatus(cancelLeft, cancelRight);
            tetr.Hold();
        }

        while (accumulator >= STEP)
        {
            UpdateFrame();
            accumulator -= STEP;

            if (accumulator > 20 * STEP)
            {
                accumulator = 0;
                break;
            }
        }

        tetr.Locked();
    }

    private void UpdateFrame()
    {
        float deltaTime = Time.time - previousTime;

        float fallTimeCurrent = fallTime / multiplier;
        float deltaTimeSoft = softDropTime > previousTime 
            ? (softDropTime - previousTime) / multiplier + Time.time - softDropTime
            : deltaTime;

        bool dropped = false;
        if (deltaTimeSoft > fallTimeCurrent)
        {
            int toFallInit = (int) (deltaTimeSoft / fallTimeCurrent);
            int toFall = toFallInit;
            dropped = true;
            while (dropped && toFall-- > 0)
            {
                dropped = tetr.Move(Vector3.down);
            }

            if (dropped || (toFallInit - toFall > 1))
            {
                previousTime = Time.time;
                lastRotated = false;
                if (tetr.transform.localPosition.y < lowestY)
                {
                    lowestY = (int)tetr.transform.localPosition.y;
                }
                if (multiplier > gravity)
                {
                    gameManager.AddScore(1);
                }
            }
        }

        if (!dropped && deltaTime > lockDelayTime && tetr.CheckGround())
        {
            gameManager.UpdateLastMovementStatus(cancelLeft, cancelRight);
            tetr.LockTetromino(lastRotated);
            previousTime = Time.time;
            return;
        }
    }

    private void HandleHorizontalArr
        (
            KeyCode key, 
            ref float previousDownTime, 
            ref float previousArrTime, 
            ref bool dasTriggered, 
            ref bool firstDown, 
            Vector3 v, 
            bool cancel
        )
    {
        bool groundedPrev = tetr.CheckGround();

        if (Input.GetKeyDown(key))
        {
            previousDownTime = Time.time;
            MoveHorizontal(v, groundedPrev);
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
                        MoveHorizontal(v, groundedPrev);
                    }
                    dasTriggered = true;
                    previousArrTime = Time.time;
                }
                else if (Time.time - previousArrTime > arr)
                {
                    if (!cancel)
                    {
                        MoveHorizontal(v, groundedPrev);
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

    private void MoveHorizontal(Vector3 v, bool groundedPrev)
    {
        bool moved = tetr.Move(v);
        if (moved)
        {
            bool grounded = PostActions();
            if (!grounded && groundedPrev)
            {
                previousTime = Time.time;
            }
        }
    }

    private bool PostRotations()
    {
        bool grounded = PostActions();
        if (grounded)
        {
            lastRotated = true;
        }
        if (lowestY > tetr.transform.position.y)
        {
            lowestY = (int)tetr.transform.position.y;
            movementCount = 0;
        }
        else if (movementCount >= GameConsts.maxDelayMovement && grounded)
        {
            tetr.LockTetromino(lastRotated);
        }
        return grounded;
    }

    private bool PostActions()
    {
        movementCount++;
        lastRotated = false;
        bool grounded = tetr.CheckGround();
        if (grounded)
        {
            previousTime = Time.time;
        }
        return grounded;
    }

    public void SetLeftRightCancelStatus(bool statusLeft, bool statusRight)
    {
        cancelLeft = statusLeft;
        cancelRight = statusRight;
    }

    private float getIntervalFromLevel()
    {
        return Mathf.Pow((float)(0.8 - (gameManager.level - 1) * 0.007), gameManager.level - 1);
    }

    void CreateGhost()
    {
        if (ghostPrefab == null || ghostGO != null)
        {
            return;
        }

        ghostGO = Instantiate(ghostPrefab, transform.position, transform.rotation);
        ghost = ghostGO.GetComponent<GhostPiece>();
        if (ghost != null)
        {
            ghost.Init(gameManager, tetr);
        }
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
