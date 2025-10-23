using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [Header("Buttons in display order (top to bottom)")]
    [SerializeField] private Button[] buttons; // size = 4
    [SerializeField] private Button backButton;
    [SerializeField] private GameObject menuRoot;
    [SerializeField] private GameObject configRoot;
    [SerializeField] private GameObject popupPanel;

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
            case MenuPage.Config:
                ShowConfig();
                escLock = true;
                break;
            case MenuPage.Drilling:
                ShowDrilling();
                escLock = true;
                break;
            case MenuPage.Root: // falls down to default
            default:
                ShowMain();
                break;
        }

        backButton.onClick.AddListener(Back);
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
        configRoot.SetActive(false);
        popupPanel.SetActive(false);
        menuRoot.SetActive(true);

        Set(0, "Single Player", ShowSinglePlayer);
        Set(1, "Multi Player",  ShowMultiPlayer);
        Set(2, "Config",        ShowConfig);
        buttons[3].gameObject.SetActive(false);

        backButton.gameObject.SetActive(false);

        if (Bootstrap.I)
        {
            Bootstrap.I.nextMenuPage = MenuPage.Root;
        }
    }

    // --------------- Single Player ---------------
    void ShowSinglePlayer()
    {
        GameManager.gameEnded = false;
        ClearAll();
        configRoot.SetActive(false);
        popupPanel.SetActive(false);
        menuRoot.SetActive(true);

        buttons[3].gameObject.SetActive(true);

        Set(0, "Sprint",   () => StartMode(SceneNames.PlayfieldSingle, GameConsts.modeSprint));
        Set(1, "Blitz",    () => StartMode(SceneNames.PlayfieldSingle, GameConsts.modeBlitz));
        Set(2, "Marathon", () => StartMode(SceneNames.PlayfieldSingle, GameConsts.modeMarathon));
        Set(3, "Drilling", ShowDrilling);

        if (Bootstrap.I)
        {
            Bootstrap.I.nextMenuPage = MenuPage.SinglePlayer;
        }

        backButton.gameObject.SetActive(true);
    }

    void ShowDrilling()
    {
        GameManager.gameEnded = false;
        ClearAll();
        configRoot.SetActive(false);
        popupPanel.SetActive(false);
        menuRoot.SetActive(true);

        buttons[3].gameObject.SetActive(true);

        Set(0, "Easy",      () => StartMode(SceneNames.PlayfieldSingle, GameConsts.modeDrilling, 0));
        Set(1, "Normal",    () => StartMode(SceneNames.PlayfieldSingle, GameConsts.modeDrilling, 1));
        Set(2, "Hard",      () => StartMode(SceneNames.PlayfieldSingle, GameConsts.modeDrilling, 2));
        Set(3, "Very Hard", () => StartMode(SceneNames.PlayfieldSingle, GameConsts.modeDrilling, 3));

        if (Bootstrap.I)
        {
            Bootstrap.I.nextMenuPage = MenuPage.Drilling;
        }

        backButton.gameObject.SetActive(true);
    }

    // --------------- Callbacks -------------------
    void ShowMultiPlayer()
    {
        GameManager.gameEnded = false;
        StartMode(SceneNames.PlayfieldMulti, GameConsts.modeBattle);
    }

    void ShowConfig()
    {
        ClearAll(); 
        menuRoot.SetActive(false);
        configRoot.SetActive(true);

        if (Bootstrap.I)
        {
            Bootstrap.I.nextMenuPage = MenuPage.Config;
        }

        backButton.gameObject.SetActive(true);
    }

    void Back()
    {
        if (Bootstrap.I.nextMenuPage != MenuPage.Config)
        {
            if (Bootstrap.I.nextMenuPage == MenuPage.Drilling)
            {
                ShowSinglePlayer();
                return;
            }
            ShowMain();
            return;
        }

        KeybindRoot root = configRoot.GetComponent<KeybindRoot>();
        bool hasConflict = root.keybindColumns.Any(e => e.HasConflict());
        if (!hasConflict)
        {
            ShowMain();
            return;
        }

        popupPanel.SetActive(true);
    }

    public void RevertSettingsAndBack()
    {
        KeybindRoot root = configRoot.GetComponent<KeybindRoot>();
        foreach (KeybindColumn column in root.keybindColumns)
        {
            column.RevertSettings();
        }
        ShowMain();
    }

    void StartMode(string scene, string sceneName, int difficulty)
    {
        SceneData.selectedMode = sceneName;
        SceneData.difficulty = difficulty;
        SceneManager.LoadScene(scene);
    }

    void StartMode(string scene, string sceneName)
    {
        SceneData.selectedMode = sceneName;
        SceneData.difficulty = -1;
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
