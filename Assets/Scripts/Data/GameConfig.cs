using UnityEngine;

[CreateAssetMenu(menuName = "Tetris/Game Config")]
public class GameConfig : ScriptableObject
{
    public GameMode mode = GameMode.Classic;
}
