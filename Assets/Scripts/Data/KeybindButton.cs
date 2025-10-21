using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class KeybindButton : MonoBehaviour
{
    private Button keyButton;
    private TMP_Text keyLabel;

    private ControlConfig configs;
    private string internalName;

    private bool binding;
    private RectTransform buttonRect;

    public void Init(ControlConfig cfg, Button btn, string name)
    {
        configs = cfg;
        keyButton = btn;
        keyLabel = btn.GetComponentInChildren<TMP_Text>();
        internalName = name;
        buttonRect = keyButton.transform as RectTransform;
        
        keyButton.onClick.RemoveAllListeners();
        keyButton.onClick.AddListener(StartBinding);

        RefreshLabel();
    }

    void RefreshLabel()
    {
        var field = configs.GetType().GetField(internalName);
        var key = (KeyCode)(field?.GetValue(configs) ?? KeyCode.None);
        keyLabel.text = key.ToString();
        keyLabel.fontSize = 14;
        keyLabel.font = Resources.Load<TMP_FontAsset>("Electronic Highway Sign SDF");
    }

    void StartBinding()
    {
        if (binding)
        {
            return;
        }

        binding = true;
        keyLabel.text = "Press a key to bind...";
        StartCoroutine(CaptureKey());
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
                        ApplyKey(code);
                        yield break;
                    }
                }
            }
            yield return null;
        }
    }

    void ApplyKey(KeyCode code)
    {
        var f = configs.GetType().GetField(internalName);
        if (f != null && f.FieldType == typeof(KeyCode))
        {
            f.SetValue(configs, code);
        }
        binding = false;
        RefreshLabel();
    }

    void CancelBinding()
    {
        binding = false;
        RefreshLabel();
    }
}
