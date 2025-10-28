using UnityEngine;
using UnityEngine.UI;

public class ToggleBGM : MonoBehaviour
{
    [SerializeField] private Toggle BGMToggle;
    [SerializeField] private BGMConfig config;

    void Awake()
    {
        BGMToggle.isOn = config.enabledBGM;
        BGMToggle.onValueChanged.AddListener(OnToggleChanged);
    }

    private void OnToggleChanged(bool value)
    {
        config.enabledBGM = value;
    }

    void OnDestroy()
    {
        BGMToggle.onValueChanged.RemoveListener(OnToggleChanged);
    }
}
