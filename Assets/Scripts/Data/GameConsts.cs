using UnityEngine;

[System.Serializable]
public struct ControlBindings
{
    public KeyCode moveLeft, moveRight, softDrop, hardDrop, rotateLeft, rotateRight, hold, restart;
}

public static class GameConsts
{
    [Header("GameData")]
    public const int GridWidth = 10;
    public const int GridHeight = 60;
    public const int TopOutHeight = 20;
    public static readonly Vector2Int SpawnCell = new(4, 18);
    public const float CellSize = 1f;

    public const int sprintLines = 40;
    public static readonly int[] drillingLines = {
        50, 100, 200, 500
    };
    public const int blitzTimeCentiSec = 12000;
    public const float softdropSpeed = 20f;
    public const int maxDelayMovement = 15;
    public const int startCountdown = 3;
    public const float harddropEnableTime = 0.1f;
    public const float lockDelayTime = 0.5f;

    public const int maxGarbageSpawn = 10;
    public const int maxGarbage = 40;
    public const int perfectClearGarbage = 10;
    public const float garbageDelay = 0.3f;

    public const KeyCode forfeit = KeyCode.Escape;

    public static Color configLabelColorDefault = Color.black;
    public static Color configLabelColorConflicted = Color.red;

    public enum TetrominoType
    {
        I, O, T, J, L, S, Z
    }

    [Header("UI data")]

    public static Vector3 LeftGarbageUIStart = Vector3.left * 0.75f;
    public static Vector3 RightGarbageUIStart = Vector3.right * 0.85f;

    [Header("Defaults Controls")]
    public static readonly ControlBindings DefaultsSingle = new ControlBindings
    {
        moveLeft = KeyCode.LeftArrow,
        moveRight = KeyCode.RightArrow,
        softDrop = KeyCode.DownArrow,
        hardDrop = KeyCode.UpArrow,
        rotateLeft = KeyCode.Z,
        rotateRight = KeyCode.X,
        hold = KeyCode.Space,
        restart = KeyCode.R
    };

    public static readonly ControlBindings DefaultsMP1 = new ControlBindings
    {
        moveLeft = KeyCode.D,
        moveRight = KeyCode.G,
        softDrop = KeyCode.F,
        hardDrop = KeyCode.R,
        rotateLeft = KeyCode.Z,
        rotateRight = KeyCode.X,
        hold = KeyCode.Space,
        restart = KeyCode.I
    };

    public static readonly ControlBindings DefaultsMP2 = new ControlBindings
    {
        moveLeft = KeyCode.Keypad4,
        moveRight = KeyCode.Keypad6,
        softDrop = KeyCode.Keypad5,
        hardDrop = KeyCode.Keypad8,
        rotateLeft = KeyCode.Period,
        rotateRight = KeyCode.Slash,
        hold = KeyCode.RightControl,
        restart = KeyCode.Keypad0
    };

    public static readonly ControlBindings None = new ControlBindings
    {
        moveLeft = KeyCode.None,
        moveRight = KeyCode.None,
        softDrop = KeyCode.None,
        hardDrop = KeyCode.None,
        rotateLeft = KeyCode.None,
        rotateRight = KeyCode.None,
        hold = KeyCode.None,
        restart = KeyCode.None
    };

    public const float gravity = 1.0f;
    public const float arr = 2.0f;
    public const float das = 10.0f;
    public const float dcd = 1.0f;
    public const int sdf = 20;
    public const float frameRate = 60f;

    public const float holdTime = 2f;

    [Header("Names")]
    public const string modeSprint  = "Sprint";
    public const string modeBlitz = "Blitz";
    public const string modeMarathon = "Marathon";
    public const string modeDrilling = "Drilling";
    public const string modeBattle = "Battle";

    [Header("Race Mode Interval")]
    public static readonly int[] DrillingGarbageInterval = { 
        20, 12, 7, 4 
    };

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
