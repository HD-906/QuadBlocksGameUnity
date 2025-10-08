using UnityEngine;

[CreateAssetMenu(menuName = "Tetris/Game Config")]
public class GameConfig : ScriptableObject
{
    public GameMode mode = GameMode.Classic;

    [Header("Gameplay")]
    public float gravity = 1.0f;
    public float arr = 2.0f;
    public float das = 10.0f;
    public float dcd = 1.0f;
    public int sdf = 6;

    [Header("Audio")]
    [Range(0f, 1f)] public float masterVolume = 1f;
}
