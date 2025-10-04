using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class MainMenuController : MonoBehaviour
{
    [Header("Buttons in display order (top to bottom)")]
    [SerializeField] private Button[] buttons; // size = 4
    [SerializeField] private Button backButton;
    
    void Start()
    {
        ShowMain();
        backButton.onClick.AddListener(ShowMain);
        backButton.gameObject.SetActive(false); // hidden on root menu
    }

    // ---------------- Main Screen ----------------
    public void ShowMain()
    {
        ClearAll();

        Set(0, "Single Player", ShowSinglePlayer);
        Set(1, "Multi Player",  OnMultiPlayer);
        Set(2, "Config",        OnConfig);
        buttons[3].gameObject.SetActive(false);

        backButton.gameObject.SetActive(false);
    }

    // --------------- Single Player ---------------
    void ShowSinglePlayer()
    {
        ClearAll();

        buttons[3].gameObject.SetActive(true);

        Set(0, "Sprint", () => StartMode("Game_Sprint"));
        Set(1, "Bliz", () => StartMode("Game_Bliz"));
        Set(2, "Endless", () => StartMode("Game_Endless"));
        Set(3, "Custom", OnCustom);

        backButton.gameObject.SetActive(true);
    }

    // --------------- Callbacks -------------------
    void OnMultiPlayer()
    {
        Debug.Log("MultiPlayer");
    }

    void OnConfig()
    {
        // SceneManager.LoadScene("Settings");
        Debug.Log("Settings");
    }

    void OnCustom()
    {
        Debug.Log("Custom clicked");
    }

    void StartMode(string sceneName)
    {
        // If you use Bootstrap/GameConfig, set mode here before loading.
        // Bootstrap.I.config.mode = GameMode.Sprint; etc.
        SceneManager.LoadScene(SceneNames.PlayfieldSingle);
        Debug.Log($"StartMode: {sceneName}");
    }

    // --------------- Helpers ---------------------
    void Set(int index, string label, UnityEngine.Events.UnityAction onClick)
    {
        var btn = buttons[index];
        btn.gameObject.SetActive(true);

        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(onClick);

        // Update TMP label
        var tmp = btn.GetComponentInChildren<TMP_Text>(true);
        if (tmp)
        {
            tmp.text = label;
        }
    }

    void ClearAll()
    {
        foreach (var b in buttons)
        {
            b.onClick.RemoveAllListeners();
        }
    }
}
