using System.Net.Sockets;
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

        Set(0, "Sprint", () => StartMode(SceneNames.PlayfieldSingle, GameConsts.modeSprint));
        Set(1, "Blitz", () => StartMode(SceneNames.PlayfieldSingle, GameConsts.modeBlitz));
        Set(2, "Marathon", () => StartMode(SceneNames.PlayfieldSingle, GameConsts.modeMarathon));
        Set(3, "Custom", OnCustom);

        if (Bootstrap.I)
        {
            Bootstrap.I.nextMenuPage = MenuPage.SinglePlayer;
        }

        backButton.gameObject.SetActive(true);
    }

    // --------------- Callbacks -------------------
    void ShowMultiPlayer()
    {
        StartMode(SceneNames.PlayfieldMulti, "Game_M_Modern");
        //ClearAll();

        //buttons[2].gameObject.SetActive(true);

        //Set(0, "Classic", () => StartMode(SceneNames.PlayfieldMulti, "Game_M_Classic"));
        //Set(1, "Modern", () => StartMode(SceneNames.PlayfieldMulti, "Game_M_Modern"));

        //if (Bootstrap.I)
        //{
        //    Bootstrap.I.nextMenuPage = MenuPage.MultiPlayer;
        //}

        //backButton.gameObject.SetActive(true);
    }

    void ShowConfig()
    {
        // SceneManager.LoadScene("Settings");

        if (Bootstrap.I)
        {
            Bootstrap.I.nextMenuPage = MenuPage.Settings;
        }
    }

    void OnCustom()
    {
        Debug.Log("Custom clicked");
    }

    void StartMode(string scene, string sceneName)
    {
        SceneData.selectedMode = sceneName;
        SceneManager.LoadScene(scene);
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
