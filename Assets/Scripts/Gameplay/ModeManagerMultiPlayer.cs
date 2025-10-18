using UnityEngine;

public class ModeManagerMultiPlayer : ModeManager
{
    private int totalLinesCleared = 0;
    private int currentLevel = 1;
    [SerializeField] GameManager gameManager_1;
    [SerializeField] GameManager gameManager_2;

    private bool started = false;

    private System.Action<int> updateAction;

    private void Awake()
    {
        gameManager_1.is_2P = false;
        gameManager_1.modeManager = this;

        gameManager_2.is_2P = true;
        gameManager_2.modeManager = this;
    }

    void Start()
    {
    }

    private void Update()
    {
        if (gameManager_1.started && !started)
        {
            started = true;
        }

        if (!gameManager_1.started)
        {
            return;
        }
    }

    public override void AddLinesCleared(int linesCleared)
    {
        totalLinesCleared += linesCleared;
    }

    public override void GetGarbagePerfectClear(bool is_2P)
    {
        if (is_2P)
        {
            gameManager_1.AddGarbageToQueue(10);
        }
        else
        {
            gameManager_2.AddGarbageToQueue(10);
        }
    }

    public override int GetGarbage(bool is_2P, int linesCleared, int tSpinStatus, bool backToBack, int combo)
    { // To be finished
        int garbage = 0;
        if (tSpinStatus < 2)
        {
            if (linesCleared < 4)
            {
                garbage = linesCleared - 1;
            }
            else
            {
                garbage = 4;
            }

            if (backToBack && tSpinStatus == 1)
            {
                garbage++;
            }
        }
        else // proper t-spin
        {
            garbage = linesCleared * (backToBack ? 3 : 2);
        }

        garbage += GameConsts.GetLinesCombo(combo);

        if (is_2P)
        {
            HandleGarbage(gameManager_2, gameManager_1, garbage);
        }
        else
        {
            HandleGarbage(gameManager_1, gameManager_2, garbage);
        }

        return garbage;
    }

    private void HandleGarbage(GameManager sender, GameManager receiver, int garbage)
    {
        garbage = sender.RemoveGarbageFromQueue(garbage);
        receiver.AddGarbageToQueue(garbage);
    }
}
