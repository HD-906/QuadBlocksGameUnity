using System;
using TMPro;
using UnityEngine;

public class ModeManagerSinglePlayer : ModeManager
{
    [SerializeField] private TMP_Text modeName;
    [SerializeField] private TMP_Text condition;
    [SerializeField] private int condValue;
    [SerializeField] private int InitSprintLines;
    private int totalLinesCleared = 0;
    private int currentLevel = 1;
    private int timeValueInitial;
    [SerializeField] private int timeValue;
    [SerializeField] public GameManager gameManager;
    private bool started = false;
    private float timeStart = 0;

    private System.Action<int> updateAction;

    void Start()
    {
        if (SceneData.selectedMode == "")
        {
            SceneData.selectedMode = GameConsts.modeBlitz;
        }
        modeName.text = SceneData.selectedMode;
        InitCondition();
        timeStart = Time.time;
    }

    private void Update()
    {
        if (gameManager.started && !started)
        {
            timeStart = Time.time;
            started = true;
        }

        if (!gameManager.started)
        {
            return;
        }

        UpdateCondition();
    }

    private void InitCondition()
    {
        switch (modeName.text)
        {
            case GameConsts.modeSprint:
                InitSprintLines = condValue = GameConsts.sprintLines;
                timeValue = timeValueInitial = 0;
                condition.text = $"{TimeToString()}\nRemaining: {condValue}";
                updateAction = UpdateConditionSprint;
                break;
            case GameConsts.modeBlitz:
                condValue = gameManager.level;
                timeValue = timeValueInitial = GameConsts.blitzTimeCentiSec;
                condition.text = $"Level {condValue}\n{TimeToString()}";
                updateAction = UpdateConditionBlitz;
                break;
            case GameConsts.modeMarathon:
                condValue = gameManager.level;
                timeValue = timeValueInitial = 0;
                condition.text = $"Level {condValue}\n{TimeToString()}";
                updateAction = UpdateConditionMarathon;
                break;
            default:
                break;
        }
    }

    private void UpdateCondition()
    {
        int timePassed = (int)((Time.time - timeStart) * 100f);
        updateAction?.Invoke(timePassed);
    }

    private void UpdateConditionSprint(int timePassed)
    {
        condValue = Mathf.Max(0, InitSprintLines - totalLinesCleared);
        timeValue = timeValueInitial + timePassed;
        condition.text = $"{TimeToString()}\nRemaining: {condValue}";
        if (condValue <= 0)
        {
            gameManager.GameCleared(TimeToString());
        }
    }

    private void UpdateConditionBlitz(int timePassed)
    {
        condValue = gameManager.level;
        timeValue = Mathf.Max(0, timeValueInitial - timePassed);
        condition.text = $"Level {condValue}\n{TimeToString()}";
        if (timeValue <= 0)
        {
            gameManager.GameCleared();
        }

        int increment = (currentLevel <= 10) ? 0 : 1;
        while (totalLinesCleared > 2 * currentLevel + increment)
        {
            totalLinesCleared -= 2 * currentLevel + 1 + increment;
            currentLevel++;
            increment = (currentLevel <= 10) ? 0 : 1;
        }
        gameManager.level = currentLevel;
    }

    private void UpdateConditionMarathon(int timePassed)
    {
        condValue = gameManager.level;
        timeValue = timeValueInitial + timePassed;
        condition.text = $"Level {condValue}\n{TimeToString()}";

        currentLevel = totalLinesCleared / 10 + 1;
        gameManager.level = currentLevel;
    }

    public override void AddLinesCleared(int linesCleared)
    {
        totalLinesCleared += linesCleared;
    }

    public override int GetGarbage(bool is_2P, int linesCleared, int tSpinStatus, bool backToBack, int combo)
    {
        return 0; // unused in Single Player
    }

    public override void GetGarbagePerfectClear(bool is_2P) { }  // unused in Single Player

    private string TimeToString()
    {
        Func<int, string> ConvertUnderHr = 
            v => $"{v / 6000:00}:{v % 6000 / 100:00}.{v % 100:00}";

        if (timeValue < 360000)
        {
            return ConvertUnderHr(timeValue);
        }

        return $"{timeValue / 360000}:{ConvertUnderHr(timeValue % 360000)}";
    }
}
