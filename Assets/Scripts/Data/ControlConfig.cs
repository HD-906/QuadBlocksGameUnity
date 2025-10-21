using UnityEngine;

[CreateAssetMenu(menuName = "Tetris/Control Config", fileName = "ControlConfig")]
public class ControlConfig : ScriptableObject
{
    public KeyCode moveLeft, moveRight, softDrop, hardDrop, rotateLeft, rotateRight, hold, restart;

    public void Apply(ControlBindings b)
    {
        moveLeft = b.moveLeft; 
        moveRight = b.moveRight;
        softDrop = b.softDrop; 
        hardDrop = b.hardDrop;
        rotateLeft = b.rotateLeft; 
        rotateRight = b.rotateRight;
        hold = b.hold; 
        restart = b.restart;
    }

    [Header("Handling")]
    public float arr = 2f;
    public float das = 10f;
    public float dcd = 1f;
    public int sdf = 6;
    public float frameRate = 60f;
}
