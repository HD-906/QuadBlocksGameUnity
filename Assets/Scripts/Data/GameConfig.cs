using UnityEngine;

[CreateAssetMenu(menuName = "Tetris/Game Config")]
public class GameConfig : ScriptableObject
{
    public GameMode mode = GameMode.Classic;

    [Header("Controls")]
    public KeyCode moveLeft = KeyCode.LeftArrow;
    public KeyCode moveRight = KeyCode.RightArrow;
    public KeyCode softDrop = KeyCode.DownArrow;
    public KeyCode hardDrop = KeyCode.UpArrow;
    public KeyCode rotateRight = KeyCode.S;
    public KeyCode rotateLeft = KeyCode.A;
    public KeyCode hold = KeyCode.Space;

    public KeyCode restart = KeyCode.R;
    public KeyCode forfeit = KeyCode.Escape;

    [Header("Handling")]
    public float gravity = 1.0f;
    public float arr = 2.0f;
    public float das = 10.0f;
    public float dcd = 1.0f;
    public int sdf = 6;

    public float frameRate = 60f;
}
