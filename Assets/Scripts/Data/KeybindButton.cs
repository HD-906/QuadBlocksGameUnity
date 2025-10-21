using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class KeybindButton : MonoBehaviour
{
    private Button keyButton;
    private TMP_Text keyLabel;

    private ControlConfig configs;
    private string internalName;

    private bool binding;
    private RectTransform buttonRect;

    private KeybindColumn column;

    public void Init(KeybindColumn parent, ControlConfig cfg, Button btn, string name)
    {
        SetMainFields(parent, cfg, btn, name);

        keyButton.onClick.RemoveAllListeners();
        keyButton.onClick.AddListener(StartBinding);

        RefreshLabel();
    }

    public void Init(KeybindColumn parent, ControlConfig cfg, Button btn, string name, UnityAction resetAct) // Reset
    {
        SetMainFields(parent, cfg, btn, name);

        keyButton.onClick.RemoveAllListeners();
        keyButton.onClick.AddListener(resetAct);
    }

    private void SetMainFields(KeybindColumn parent, ControlConfig cfg, Button btn, string name)
    {
        column = parent;
        configs = cfg;
        keyButton = btn;
        keyLabel = btn.GetComponentInChildren<TMP_Text>();
        internalName = name;
        buttonRect = keyButton.transform as RectTransform;
    }


    public void RefreshLabel()
    {
        var field = configs.GetType().GetField(internalName);
        var key = (KeyCode)(field?.GetValue(configs) ?? KeyCode.None);
        keyLabel.text = key.ToString();
        keyLabel.fontSize = 14;
    }

    void StartBinding()
    {
        if (binding)
        {
            return;
        }

        binding = true;
        //keyLabel.text = "Press a key to bind...";
        StartCoroutine(CaptureKey());
    }

    void SetConflictColour(bool conflict)
    {
        keyLabel.color = conflict ? GameConsts.configLabelColorConflicted : GameConsts.configLabelColorDefault;
    }

    IEnumerator CaptureKey()
    {
        // Consume initial click
        yield return null;

        while (binding)
        {
            // ESC to cancel
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                CancelBinding();
                yield break;
            }

            // Cancel if user clicks somewhere else
            else if (Input.GetMouseButtonDown(0))
            {
                // If the click is NOT on our button, cancel
                if (!RectTransformUtility.RectangleContainsScreenPoint(
                        buttonRect, Input.mousePosition, null))
                {
                    CancelBinding();
                    yield break;
                }
            }

            // Detect any key
            else if (Input.anyKeyDown)
            {
                foreach (KeyCode code in Enum.GetValues(typeof(KeyCode)))
                {
                    if (Input.GetKeyDown(code))
                    {
                        ApplyKeyUnique(code);
                        yield break;
                    }
                }
            }
            yield return null;
        }
    }

    void ApplyKeyUnique(KeyCode keyCode)
    {
        if (keyCode.ToString() == keyLabel.text)
        {
            binding = false;
            return;
        }
        bool conflict = column.FindAndTurnConflictRed(keyCode, keyLabel.text);
        SetConflictColour(conflict);
        if (conflict)
        {
            keyLabel.text = keyCode.ToString();
            binding = false;
            return;
        }
        ApplyKey(keyCode);
    }

    public void ApplyKey()
    {
        ApplyKey(GetKeyCodeFromLabel());
    }

    void ApplyKey(KeyCode keyCode)
    {
        var f = configs.GetType().GetField(internalName);
        if (f != null && f.FieldType == typeof(KeyCode))
        {
            f.SetValue(configs, keyCode);
        }
        binding = false;
        RefreshLabel();
    }

    void CancelBinding()
    {
        binding = false;
        RefreshLabel();
    }

    private KeyCode GetKeyCodeFromLabel()
    {
        if (System.Enum.TryParse(keyLabel.text, out KeyCode code))
        {
            return code;
        }
        else
        {
            Debug.LogWarning("Invalid KeyCode string: " + keyLabel.text);
            return KeyCode.None;
        }
    }
}
