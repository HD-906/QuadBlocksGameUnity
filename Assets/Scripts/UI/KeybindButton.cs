using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class KeybindButton : MonoBehaviour
{
    private Button keyButton;
    private TMP_Text keyLabel;
    public KeyCode KeyCode { get; private set; }

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
        UpdateKeyCode();
        buttonRect = keyButton.transform as RectTransform;
    }


    public void RefreshLabel()
    {
        UpdateKeyCode();
        keyLabel.text = KeyCode.ToString();
        keyLabel.fontSize = 14;
    }

    private void UpdateKeyCode()
    {
        KeyCode = (KeyCode)(configs.GetType().GetField(internalName)?.GetValue(configs) ?? KeyCode.None);
    }

    private void StartBinding()
    {
        if (binding)
        {
            return;
        }

        binding = true;
        //keyLabel.text = "Press a key to bind...";
        StartCoroutine(CaptureKey());
    }

    public void SetConflictColour(bool conflict)
    {
        keyLabel.color = conflict ? GameConsts.configLabelColorConflicted : GameConsts.configLabelColorDefault;
    }

    public bool HasConflict()
    {
        return keyLabel.color == GameConsts.configLabelColorConflicted;
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

    private void ApplyKeyUnique(KeyCode newKeyCode)
    {
        if (newKeyCode.ToString() == keyLabel.text) // set to the same key
        {
            binding = false;
            return;
        }
        bool conflict = column.FindAndToggleConflict(newKeyCode, keyLabel);

        keyLabel.text = newKeyCode.ToString();
        KeyCode = newKeyCode;
        if (conflict)
        {
            binding = false;
            return;
        }
        ApplyKey();
    }

    public void ApplyKey()
    {
        var f = configs.GetType().GetField(internalName);
        if (f != null && f.FieldType == typeof(KeyCode))
        {
            f.SetValue(configs, KeyCode);
        }
        binding = false;
        RefreshLabel();
    }

    private void CancelBinding()
    {
        binding = false;
        RefreshLabel();
    }
}
