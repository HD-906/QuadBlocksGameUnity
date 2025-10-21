using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class KeybindMenu : MonoBehaviour
{
    [System.Serializable]
    public struct BindDef
    {
        public string actionName;    // label shown to player
        public string internalName;   // must match ControlConfig field (e.g., "moveLeft")
    }

    [SerializeField] private Button resetButton;

    [Header("UI")]
    [SerializeField] private KeybindColumn columnSingle;
    [SerializeField] private KeybindColumn columnMP1;
    [SerializeField] private KeybindColumn columnMP2;

    void Awake()
    {
        if (resetButton)
        {
            resetButton.onClick.AddListener(ResetToDefaults);
        }
    }

    void ResetToDefaults()
    {
        columnSingle.ResetDefault();
        columnMP1.ResetDefault();
        columnMP2.ResetDefault();
    }
}
