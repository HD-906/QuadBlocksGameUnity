using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [Header("Buttons in display order (top to bottom)")]
    [SerializeField] private Button[] buttons; // size = 4
    [SerializeField] private Button backButton;

    private bool escLock = false;
    
    void Start()
    {
        var target = Bootstrap.I ? Bootstrap.I.nextMenuPage : MenuPage.Root;

        switch (target)
        {
            case MenuPage.SinglePlayer:
                ShowSinglePlayer();
                escLock = true;
                break;
            case MenuPage.MultiPlayer:
                ShowMultiPlayer();
                escLock = true;
                break;
            case MenuPage.Settings:
                ShowConfig();
                escLock = true;
                break;
            case MenuPage.Root: // falls down to default
            default:
                ShowMain();
                break;
        }

        backButton.onClick.AddListener(ShowMain);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !escLock)
        {
            ShowMain();
        }

        if (Input.GetKeyUp(KeyCode.Escape))
        {
            escLock = false;
        }
    }

    // ---------------- Main Screen ----------------
    public void ShowMain()
    {
        ClearAll();

        Set(0, "Single Player", ShowSinglePlayer);
        Set(1, "Multi Player",  ShowMultiPlayer);
        Set(2, "Config",        ShowConfig);
        buttons[3].gameObject.SetActive(false);

        backButton.gameObject.SetActive(false);

        if (Bootstrap.I)
        {
            Bootstrap.I.nextMenuPage = MenuPage.Root;
            Debug.Log("Set to root");
        }
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

        if (Bootstrap.I)
        {
            Bootstrap.I.nextMenuPage = MenuPage.SinglePlayer;
            Debug.Log("Set to sp");
        }

        backButton.gameObject.SetActive(true);
    }

    // --------------- Callbacks -------------------
    void ShowMultiPlayer()
    {
        if (Bootstrap.I)
        {
            Bootstrap.I.nextMenuPage = MenuPage.MultiPlayer;
            Debug.Log("Set to mp");
        }
    }

    void ShowConfig()
    {
        // SceneManager.LoadScene("Settings");

        if (Bootstrap.I)
        {
            Bootstrap.I.nextMenuPage = MenuPage.Settings;
            Debug.Log("Set to s");
        }
    }

    void OnCustom()
    {
        Debug.Log("Custom clicked");
    }

    void StartMode(string sceneName)
    {
        SceneManager.LoadScene(SceneNames.PlayfieldSingle);
    }

    // --------------- Helpers ---------------------
    void Set(int index, string label, UnityEngine.Events.UnityAction onClickAction)
    {
        var btn = buttons[index];
        btn.gameObject.SetActive(true);

        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(onClickAction);

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
