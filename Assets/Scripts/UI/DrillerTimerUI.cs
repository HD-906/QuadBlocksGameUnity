using System.Collections.Generic;
using UnityEngine;

public class DrillerTimerUI : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GameObject previewUnitPrefab;
    private readonly List<GameObject> pool = new();

    public event System.Action<int> OnChanged;
    public int MaxTimer { get; } = GameConsts.TopOutHeight;
    private int timerCount = 0;
    public int TimerCount
    {
        get => timerCount;
        set
        {
            value = Mathf.Clamp(value, 0, MaxTimer);
            if (value == timerCount) return;
            timerCount = value;
            OnChanged?.Invoke(value);
        }
    }

    private void OnEnable()
    {
        OnChanged += Render;
        EnsurePool();
    }

    private void OnDisable()
    {
        OnChanged -= Render;
    }

    private void EnsurePool()
    {
        int count = 0;
        while (pool.Count < GameConsts.TopOutHeight)
        {
            var go = Instantiate
                (
                    previewUnitPrefab,
                    leftField(!gameManager.is_2P, count++),
                    Quaternion.identity
                );
            var goSR = go.GetComponent<SpriteRenderer>();
            if (goSR != null)
            {
                goSR.color = new Color(0.75f, 1.0f - count / 20f, 0.1f);
            }
            go.SetActive(false);
            pool.Add(go);
        }
    }

    private void Render(int current)
    {
        EnsurePool();

        for (int i = 0; i < pool.Count; i++)
        {
            pool[i].SetActive(i < current);
        }
    }

    private Vector3 leftField(bool isLeft, int count)
    {
        return isLeft
            ? gameManager.CellToWorld(new Vector2Int(0, count)) + GameConsts.LeftGarbageUIStart
            : gameManager.CellToWorld(new Vector2Int(9, count)) + GameConsts.RightGarbageUIStart;
    }
}
