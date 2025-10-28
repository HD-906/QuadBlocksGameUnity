using UnityEngine;
using TMPro;

public class PreviewSlots : MonoBehaviour
{
    [SerializeField] private Transform anchor;
    [SerializeField] private TMP_Text label;
    [SerializeField] private Vector3 slotBias = Vector3.zero;

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

        Vector3 biasPos = prefab.GetComponent<Tetromino>().Type switch
        {
            GameConsts.TetrominoType.O => slotBias + Vector3.left * 0.3f,
            GameConsts.TetrominoType.I => slotBias + (Vector3.up + Vector3.left) * 0.3f,
            _ => Vector3.zero
        };
        current = Instantiate(prefab, anchor.position + biasPos, Quaternion.identity, anchor);

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
