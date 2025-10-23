using UnityEngine;
using UnityEngine.UI;

public class PopupPanelController : MonoBehaviour
{
    [SerializeField] private GameObject popupPanel;
    [SerializeField] private MainMenuController mainController;
    [SerializeField] private Button positive;
    [SerializeField] private Button negative;

    [SerializeField] private 

    void Awake()
    {
        positive.onClick.RemoveAllListeners();
        negative.onClick.RemoveAllListeners();

        positive.onClick.AddListener(RevertSettings);
        negative.onClick.AddListener(() => popupPanel.SetActive(false));
    }

    void RevertSettings()
    {
        mainController.RevertSettingsAndBack();
    }
}
