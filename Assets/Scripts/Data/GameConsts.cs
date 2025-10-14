using UnityEngine;

public static class GameConsts
{
    [Header("GameData")]
    public const int GridWidth = 10;
    public const int GridHeight = 25;
    public static readonly Vector2Int SpawnCell = new(4, 18);
    public const float CellSize = 1f;

    public const int sprintLines = 40;
    public const int blitzTimeCentiSec = 12000;
    public const float softdropSpeed = 20f;
    public const int maxDelayMovement = 15;
    public const int startCountdown = 3;

    [Header("Defaults")]
    public const float gravity = 1.0f;
    public const float arr = 2.0f;
    public const float das = 10.0f;
    public const float dcd = 1.0f;
    public const int sdf = 6;
    public const float frameRate = 60f;

    public const float holdTime = 2f;

    [Header("Names")]
    public const string modeSprint  = "Sprint";
    public const string modeBlitz = "Blitz";
    public const string modeMarathon = "Marathon";
}
