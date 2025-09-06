using UnityEngine;

[RequireComponent(typeof(Tetromino))]
public class InFieldLogic : MonoBehaviour
{
    [SerializeField] float fallTime = 2f;
    float previousTime;
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
    }

    void Update()
    {
        if (GameManager.isGameOver)
        {
            return;
        }

        //gravity
        if (Time.time - previousTime > fallTime)
        {
            tetr.LockIfPossible();
            previousTime = Time.time;
        }

        // shift left
        if (Input.GetKeyDown(KeyCode.LeftArrow)) 
        {
            bool ground = tetr.CheckGround();
            bool moved = tetr.Move(Vector3.left);
            if (moved && (ground || tetr.CheckGround()))
                previousTime = Time.time;
        }

        // shift right
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            bool ground = tetr.CheckGround();
            bool moved = tetr.Move(Vector3.right);
            if (moved && (ground || tetr.CheckGround()))
                previousTime = Time.time;
        }

        // rotate left
        if (Input.GetKeyDown(KeyCode.A))
        {
            bool rotated = tetr.Rotate(1);
            if (rotated)
                previousTime = Time.time;
        }

        // rotate right
        if (Input.GetKeyDown(KeyCode.S))
        {
            bool rotated = tetr.Rotate(-1);
            if (rotated)
                previousTime = Time.time;
        }

        // softdrop
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (tetr.Move(Vector3.down))
                previousTime = Time.time;
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
}
