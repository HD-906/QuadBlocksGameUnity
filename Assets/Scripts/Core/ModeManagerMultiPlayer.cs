using UnityEngine;

public class ModeManagerMultiPlayer : ModeManager
{
    [SerializeField] GameManager gameManager_1, gameManager_2;
    private int currentLevel = 1;
    private float startTime;

    private bool started = false;

    private void Awake()
    {
        gameManager_1.is_2P = false;
        gameManager_1.modeManager = this;

        gameManager_2.is_2P = true;
        gameManager_2.modeManager = this;
    }

    void Start()
    {
        startTime = Time.time;
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

        currentLevel = Mathf.Clamp((int)((Time.time - startTime) / 60f) + 1, 1, 20);
        gameManager_1.level = gameManager_2.level = currentLevel;
    }

    public override void AddLinesCleared(int linesCleared) { } // used in Single Player mode

    public override void GetGarbagePerfectClear(bool is_2P)
    {
        if (is_2P)
        {
            gameManager_1.AddGarbageToQueueDelayed(10);
        }
        else
        {
            gameManager_2.AddGarbageToQueueDelayed(10);
        }
    }

    public override int GetGarbage(bool is_2P, int linesCleared, int tSpinStatus, bool backToBack, int combo)
    {
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
        receiver.AddGarbageToQueueDelayed(garbage);
    }
}
