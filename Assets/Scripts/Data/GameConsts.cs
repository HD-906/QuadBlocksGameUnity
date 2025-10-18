using UnityEngine;

public static class GameConsts
{
    [Header("GameData")]
    public const int GridWidth = 10;
    public const int GridHeight = 60;
    public static readonly Vector2Int SpawnCell = new(4, 18);
    public const float CellSize = 1f;

    public const int sprintLines = 40;
    public const int blitzTimeCentiSec = 12000;
    public const float softdropSpeed = 20f;
    public const int maxDelayMovement = 15;
    public const int startCountdown = 3;

    public const int maxGarbageSpawn = 10;
    public const int maxGarbage = 40;

    [Header("Defaults Controls Single")]
    public const KeyCode moveLeft = KeyCode.LeftArrow;
    public const KeyCode moveRight = KeyCode.RightArrow;
    public const KeyCode softDrop = KeyCode.DownArrow;
    public const KeyCode hardDrop = KeyCode.UpArrow;
    public const KeyCode rotateLeft = KeyCode.Z;
    public const KeyCode rotateRight = KeyCode.X;
    public const KeyCode hold = KeyCode.Space;

    public const KeyCode restart = KeyCode.R;
    public const KeyCode forfeit = KeyCode.Escape;

    [Header("Defaults Controls 1P")]
    public const KeyCode moveLeft_1P = KeyCode.D;
    public const KeyCode moveRight_1P = KeyCode.G;
    public const KeyCode softDrop_1P = KeyCode.F;
    public const KeyCode hardDrop_1P = KeyCode.R;
    public const KeyCode rotateLeft_1P = KeyCode.Z;
    public const KeyCode rotateRight_1P = KeyCode.X;
    public const KeyCode hold_1P = KeyCode.Space;

    public const KeyCode restart_1P = KeyCode.I;
    public const KeyCode forfeit_1P = KeyCode.Escape;

    [Header("Defaults Controls 2P")]
    public const KeyCode moveLeft_2P = KeyCode.Keypad4;
    public const KeyCode moveRight_2P = KeyCode.Keypad6;
    public const KeyCode softDrop_2P = KeyCode.Keypad5;
    public const KeyCode hardDrop_2P = KeyCode.Keypad8;
    public const KeyCode rotateLeft_2P = KeyCode.Period;
    public const KeyCode rotateRight_2P = KeyCode.Comma;
    public const KeyCode hold_2P = KeyCode.RightControl;

    public const KeyCode restart_2P = KeyCode.I;
    public const KeyCode forfeit_2P = KeyCode.Escape;

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

    [Header("Combo Garbage List")]

    private static readonly int[] linesByCombo = {
        0, 0, 1, 1, 2, 2, 3, 3, 4, 4, 4, 4, 4, 5
    };

    public static int GetLinesCombo(int combo)
    {
        int i = Mathf.Clamp(combo, 0, linesByCombo.Length - 1);
        return linesByCombo[i];
    }
}
