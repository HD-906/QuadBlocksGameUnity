using UnityEngine;

public class PauseMenuInput : MonoBehaviour
{
    void Update()
    {
        GameManager manager = FindAnyObjectByType<GameManager>();
        if (manager == null || !GameManager.gameFinished)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            manager.RestartGame();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            manager.ForfeitGame();
        }
    }
}
