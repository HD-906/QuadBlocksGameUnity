using UnityEngine;
using System.Collections.Generic;

public class PreviewController : MonoBehaviour
{
    [SerializeField] private PreviewSlots holdSlot;
    [SerializeField] private PreviewSlots[] nextSlots;

    public void ShowHold(GameObject prefab)
    {
        holdSlot.SetLabel("Hold");
        if (prefab)
        {
            holdSlot.Show(prefab);
        }
        else
        {
            holdSlot.Clear();
        }
    }

    public void ShowNext(IReadOnlyList<GameObject> bag)
    {
        for (int i = 0; i < nextSlots.Length; i++)
        {
            nextSlots[i].SetLabel(i == 0 ? "Next" : "");
            if (bag != null && i < bag.Count)
            {
                nextSlots[i].Show(bag[i]);
            }
            else
            {
                nextSlots[i].Clear();
            }
        }
    }
}
