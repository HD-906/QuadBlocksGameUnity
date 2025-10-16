using UnityEngine;

[CreateAssetMenu(menuName = "Tetris/Control Config", fileName = "ControlConfig")]
public class ControlConfig : ScriptableObject
{
    [Header("Keys")]
    public KeyCode moveLeft;
    public KeyCode moveRight;
    public KeyCode softDrop;
    public KeyCode hardDrop;
    public KeyCode rotateLeft;
    public KeyCode rotateRight;
    public KeyCode hold;

    public KeyCode restart;
    public KeyCode forfeit;

    [Header("Handling")]
    public float arr = 2f;
    public float das = 10f;
    public float dcd = 1f;
    public int sdf = 6;
    public float frameRate = 60f;
}
