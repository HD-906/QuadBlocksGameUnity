using UnityEngine;

public abstract class ModeManager : MonoBehaviour
{
    public abstract void AddLinesCleared(int linesCleared);
    public abstract int GetGarbage(bool is_2P, int linesCleared, int tSpinStatus, bool backToBack, int combo);
    public abstract void GetGarbagePerfectClear(bool is_2P);
}
