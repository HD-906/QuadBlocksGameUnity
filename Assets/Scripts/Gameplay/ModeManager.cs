using TMPro;
using UnityEngine;

public class ModeManager : MonoBehaviour
{
    [SerializeField] TMP_Text modeName;
    [SerializeField] TMP_Text condition;
    [SerializeField] int condValue;
    [SerializeField] GameManager gameManager;

    void Start()
    {
        if (SceneData.selectedMode == "")
        {
            SceneData.selectedMode = GameConsts.modeBlitz;
        }
        modeName.text = SceneData.selectedMode;
        ConditionInit();
    }

    void ConditionInit()
    {
        switch (modeName.text)
        {
            case GameConsts.modeSprint:
                condValue = GameConsts.sprintLines;
                condition.text = $"Lines Remaining: \n{condValue}";
                break;
            case GameConsts.modeBlitz:
                condValue = GameConsts.blitzTimeCentiSec;
                condition.text = $"Time Remaining: \n{condToTimeCenti()}";
                break;
            case GameConsts.modeMarathon:
                condValue = gameManager.level;
                condition.text = $"Current Level: \n{condValue}";
                break;
            default:
                break;
        }
    }

    private string condToTimeCenti()
    {
        return $"{condValue / 6000:00}:{condValue % 6000 / 100:00}.{condValue % 100:00}";
    }
}
