using UnityEngine;

[CreateAssetMenu(menuName = "Tetris/Game Config")]
public class GameConfig : ScriptableObject
{
    public GameMode mode = GameMode.Classic;

    [Header("Gameplay")]
    public int sprintLines = 40;
    public float gravitySecs = 1.0f;

    [Header("Audio")]
    [Range(0f, 1f)] public float masterVolume = 1f;
}
