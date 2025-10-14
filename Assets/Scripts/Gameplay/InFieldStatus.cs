using TMPro;
using UnityEngine;

public class InFieldStatus : MonoBehaviour
{
    [SerializeField] TMP_Text scoreField;
    private int score = 0;
    private bool backToBack = false;
    private int combo = 0;

    void Update()
    {
        scoreField.text = score > 0 ? score.ToString("#,#") : "0";
    }

    public void AddScore(int toAdd)
    {
        score += toAdd;
    }

    public void AddScoreBonus(int linesCleared, int tSpinStatus, int level)
    {
        int scoreInc = 0;
        if (tSpinStatus < 2)
        {
            switch (linesCleared)
            {
                case 4:
                    scoreInc += 800;
                    break;
                case 3:
                    scoreInc += 500;
                    break;
                case 2:
                    scoreInc += 300;
                    break;
                case 1:
                    scoreInc += 100;
                    break;
                default:
                    break;
            }

            scoreInc += tSpinStatus * 100;
        }
        else
        {
            switch (linesCleared)
            {
                case 3:
                    scoreInc += 1600;
                    break;
                case 2:
                    scoreInc += 1200;
                    break;
                case 1:
                    scoreInc += 800;
                    break;
                case 0:
                    scoreInc += 400;
                    break;
                default:
                    break;
            }
        }

        if (linesCleared * tSpinStatus > 0 || linesCleared == 4)
        {
            if (backToBack)
            {
                scoreInc += scoreInc / 2;
            }
            backToBack = true;
        }
        else if (linesCleared > 0)
        {
            backToBack = false;
        }

        if (linesCleared > 0)
        {
            scoreInc += 50 * combo++;
        }
        else
        {
            combo = 0;
        }

        score += scoreInc * level;
    }

    public void AddScorePerfectClear(int linesCleared, int level)
    {
        int scoreInc = 0;
        switch (linesCleared)
        {
            case 4:
                scoreInc += backToBack ? 3200 : 2000;
                break;
            case 3:
                scoreInc += 1800;
                break;
            case 2:
                scoreInc += 1200;
                break;
            case 1:
                scoreInc += 800;
                break;
            default:
                break;
        }

        score += scoreInc * level;
    }

    public int GetScore()
    { 
        return score;
    }
}
