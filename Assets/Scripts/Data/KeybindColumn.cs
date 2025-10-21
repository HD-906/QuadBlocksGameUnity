using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class KeybindColumn : MonoBehaviour
{
    [SerializeField] private KeybindColumn multiOtrKC; // null for single player
    [SerializeField] private ControlConfig controlConfig;
    [SerializeField] private int controlType; // 0, 1, 2 for SinglePlayer, MP1 and MP2 respectively
    private ControlBindings defaultBindings;
    TMP_Text[] buttonLabels;

    UnityAction refreshAll;

    private void Awake()
    {
        switch (controlType)
        {
            case 1:
                defaultBindings = GameConsts.DefaultsMP1;
                break;
            case 2:
                defaultBindings = GameConsts.DefaultsMP2;
                break;
            case 0:
            default:
                defaultBindings = GameConsts.DefaultsSingle;
                break;
        }
    }

    private void Start()
    {
        Button[] buttons = GetComponentsInChildren<Button>(true);
        int i = 0;

        refreshAll = null;
        foreach (Button btn in buttons)
        {
            string id = btn.name.Replace("btn_", "");
            KeybindButton kb = btn.gameObject.GetComponent<KeybindButton>()
                            ?? btn.gameObject.AddComponent<KeybindButton>();

            if (i != buttons.Length - 1)
            {
                kb.Init(this, controlConfig, btn, id);
                refreshAll += kb.RefreshLabel;
            }
            else // is Reset
            {
                UnityAction resetAct = () =>
                {
                    controlConfig.Apply(defaultBindings);
                    refreshAll?.Invoke();
                };
                kb.Init(this, controlConfig, btn, id, resetAct);
            }
            i++;
        }
        buttonLabels = buttons.Select(b => b.GetComponentInChildren<TMP_Text>(true)).ToArray();
    }

    public bool FindAndTurnConflictRed(KeyCode key, string original)
    {
        bool found = false;
        string keyLabel = key.ToString();
        TMP_Text onlyConflicted = null;
        bool wasUnique = true;

        foreach (TMP_Text label in buttonLabels)
        {
            if (label?.text == keyLabel)
            {
                label.color = GameConsts.configLabelColorConflicted;
                found = true;
            }
            if (label?.text == original)
            {
                if (wasUnique)
                {
                    onlyConflicted = label;
                    wasUnique = false;
                }
                else
                {
                    onlyConflicted = null;
                }
            }
        }

        if (onlyConflicted != null)
        {
            onlyConflicted.color = GameConsts.configLabelColorDefault;
            KeybindButton kb = onlyConflicted.transform.parent.GetComponent<KeybindButton>();
            kb.ApplyKey();
        }
        
        return found;
    }
}
