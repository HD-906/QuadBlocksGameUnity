using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class KeybindColumn : MonoBehaviour
{
    [SerializeField] private ControlConfig controlConfig;
    [SerializeField] private int controlType; // 0, 1, 2 for SinglePlayer, MP1 and MP2 respectively

    private void Start()
    {
        Button[] buttons = GetComponentsInChildren<Button>(true);

        foreach (Button btn in buttons)
        {
            string id = btn.name.Replace("btn_", "");
            KeybindButton kb = btn.gameObject.AddComponent<KeybindButton>();
            kb.Init(controlConfig, btn, id);
        }
    }

    public void ResetDefault()
    {
        switch(controlType)
        {
            case 0:
                controlConfig.Apply(GameConsts.DefaultsSingle);
                break;
            case 1:
                controlConfig.Apply(GameConsts.DefaultsMP1);
                break;
            case 2:
                controlConfig.Apply(GameConsts.DefaultsMP2);
                break;
            default:
                break;
        }
    }
}
