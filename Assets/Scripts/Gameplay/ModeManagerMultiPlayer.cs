using UnityEngine;

public class ModeManagerMultiPlayer : ModeManager
{
    private int totalLinesCleared = 0;
    private int currentLevel = 1;
    [SerializeField] GameManager gameManager_1;
    [SerializeField] GameManager gameManager_2;
    private bool started = false;

    private System.Action<int> updateAction;

    void Start()
    {
        if (SceneData.selectedMode == "")
        {
            SceneData.selectedMode = GameConsts.modeBlitz;
        }
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
}
