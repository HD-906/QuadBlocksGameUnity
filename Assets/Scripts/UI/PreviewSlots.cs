using UnityEngine;
using TMPro;

public class PreviewSlots : MonoBehaviour
{
    [SerializeField] private Transform anchor;
    [SerializeField] private TMP_Text label;

    private GameObject current;

    public void SetLabel(string text)
    {
        if (label != null)
        {
            label.text = text;
        }
    }

    public void Show(GameObject prefab)
    {
        Clear();

        if (prefab == null || anchor == null)
        {
            return;
        }

        current = Instantiate(prefab, anchor.position, Quaternion.identity, anchor);

        var logic = current.GetComponent<TetroLogic>();
        if (logic)
        {
            logic.enabled = false;
        }

        current.transform.localScale = Vector3.one;
        current.transform.rotation = Quaternion.identity;
    }

    public void Clear()
    {
        if (current != null)
        {
            Destroy(current);
            current = null;
        }
    }
}
