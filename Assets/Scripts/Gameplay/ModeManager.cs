using System;
using System.Threading;
using TMPro;
using UnityEngine;

public class ModeManager : MonoBehaviour
{
    [SerializeField] TMP_Text modeName;
    [SerializeField] TMP_Text condition;
    [SerializeField] public int condValue;
    [SerializeField] public int InitSprintLines;
    private int totalLinesCleared = 0;
    private int currentLevel = 1;
    private int timeValueInitial;
    [SerializeField] int timeValue;
    [SerializeField] GameManager gameManager;
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

    void InitCondition()
    {
        switch (modeName.text)
        {
            case GameConsts.modeSprint:
                InitSprintLines = condValue = GameConsts.sprintLines;
                timeValue = timeValueInitial = 0;
                condition.text = $"{timeToString()}\nRemaining: {condValue}";
                updateAction = UpdateConditionSprint;
                break;
            case GameConsts.modeBlitz:
                condValue = gameManager.level;
                timeValue = timeValueInitial = GameConsts.blitzTimeCentiSec;
                condition.text = $"Level {condValue}\n{timeToString()}";
                updateAction = UpdateConditionBlitz;
                break;
            case GameConsts.modeMarathon:
                condValue = gameManager.level;
                timeValue = timeValueInitial = 0;
                condition.text = $"Level {condValue}\n{timeToString()}";
                updateAction = UpdateConditionMarathon;
                break;
            default:
                break;
        }
    }

    void UpdateCondition()
    {
        int timePassed = (int)((Time.time - timeStart) * 100);
        updateAction(timePassed);
    }

    void UpdateConditionSprint(int timePassed)
    {
        condValue = Mathf.Max(0, InitSprintLines - totalLinesCleared);
        timeValue = timeValueInitial + timePassed;
        condition.text = $"{timeToString()}\nRemaining: {condValue}";
        if (condValue <= 0)
        {
            gameManager.GameCleared(timeToString());
        }
    }

    void UpdateConditionBlitz(int timePassed)
    {
        condValue = gameManager.level;
        timeValue = Mathf.Max(0, timeValueInitial - timePassed);
        condition.text = $"Level {condValue}\n{timeToString()}";
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

    void UpdateConditionMarathon(int timePassed)
    {
        condValue = gameManager.level;
        timeValue = timeValueInitial + timePassed;
        condition.text = $"Level {condValue}\n{timeToString()}";

        currentLevel = totalLinesCleared / 10 + 1;
        gameManager.level = currentLevel;
    }

    public void clearLines(int linesCleared)
    {
        totalLinesCleared += linesCleared;
    }

    private string timeToString()
    {
        Func<int, string> ConvertUnderHr = 
            value => $"{timeValue / 6000:00}:{timeValue % 6000 / 100:00}.{timeValue % 100:00}";

        if (timeValue < 360000)
        {
            return ConvertUnderHr(timeValue);
        }

        return $"{timeValue / 360000}:{ConvertUnderHr(timeValue % 360000)}";
    }
}
