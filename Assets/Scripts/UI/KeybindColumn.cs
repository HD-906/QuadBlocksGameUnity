using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class KeybindColumn : MonoBehaviour
{
    [SerializeField] private KeybindColumn multiOtrKC; // null for single player
    [SerializeField] private ControlConfig controlConfig;
    [SerializeField] private int controlType; // 0, 1, 2 for SinglePlayer, MP1 and MP2 respectively
    private ControlBindings defaultBindings;
    // private Button[] buttons;
    private TMP_Text[] buttonLabels;

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
                refreshAll += () => kb.SetConflictColour(false);
            }
            else // is Reset
            {
                UnityAction resetAct = () =>
                {
                    controlConfig.Apply(defaultBindings);
                    refreshAll?.Invoke();
                    if (multiOtrKC != null)
                    {
                        foreach (TMP_Text label in multiOtrKC.buttonLabels)
                        {
                            multiOtrKC.FindAndToggleConflict(label.text, label);
                        }
                    }
                };
                kb.Init(this, controlConfig, btn, id, resetAct);
            }
            i++;
        }
        buttonLabels = buttons
            .Take(buttons.Length - 1)
            .Select(b => b.GetComponentInChildren<TMP_Text>(true))
            .ToArray();
    }

    public bool FindAndToggleConflict(KeyCode newKey, TMP_Text keyLabelTMP)
    {
        return FindAndToggleConflict(newKey.ToString(), keyLabelTMP);
    }

    public bool FindAndToggleConflict(string newLabel, TMP_Text keyLabelTMP)
    {
        string originalLabel = keyLabelTMP.text;
        bool found = false;
        TMP_Text onlyConflicted = null;
        bool wasUnique = true;

        FindInArray(ref found, newLabel, originalLabel, ref onlyConflicted, ref wasUnique, keyLabelTMP);
        multiOtrKC?
            .FindInArray(ref found, newLabel, originalLabel, ref onlyConflicted, ref wasUnique, keyLabelTMP);

        if (onlyConflicted != null)
        {
            onlyConflicted.color = GameConsts.configLabelColorDefault;
            KeybindButton kb = onlyConflicted.transform.parent.GetComponent<KeybindButton>();
            kb.ApplyKey();
        }

        keyLabelTMP.color = found ? GameConsts.configLabelColorConflicted : GameConsts.configLabelColorDefault;

        return found;
    }

    public void FindInArray(ref bool found, string newLabel, string originalLabel, ref TMP_Text onlyConflicted, ref bool wasUnique, TMP_Text keyLabelTMP)
    {
        foreach (TMP_Text label in buttonLabels)
        {
            if (label == keyLabelTMP)
            {
                continue;
            }

            if (label?.text == newLabel)
            {
                label.color = GameConsts.configLabelColorConflicted;
                found = true;
                continue;
            }

            if (label?.text == originalLabel)
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
    }
}
