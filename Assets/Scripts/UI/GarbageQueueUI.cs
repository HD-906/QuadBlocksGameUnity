using System.Collections.Generic;
using UnityEngine;

public class GarbageQueueUI : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GameObject unitPrefab;
    private GarbageHandler handler;
    private readonly List<GameObject> pool = new();

    private Vector3 shiftLeft = Vector3.left * 0.75f;
    private Vector3 shiftRight = Vector3.right * 0.85f;

    private void Awake()
    {
        handler = gameManager.garbageHandler;
    }

    private void OnEnable()
    {
        handler.OnChanged += Render;
        Render(handler.GarbageQueue);
    }

    private void OnDisable()
    {
        handler.OnChanged -= Render;
    }

    private void EnsurePool()
    {
        int count = 0;
        while (pool.Count < handler.MaxGarbage)
        {
            var go = Instantiate
                (
                    unitPrefab,
                    leftField(gameManager.is_2P, count++),
                    Quaternion.identity
                );
            if (count <= handler.MaxGarbageSpawn)
            {
                var goSR = go.GetComponent<SpriteRenderer>();
                if (goSR != null)
                {
                    goSR.color = new Color(0.55f, 0f, 0.1f);
                }
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
            ? gameManager.CellToWorld(new Vector2Int(0, count)) + shiftLeft
            : gameManager.CellToWorld(new Vector2Int(9, count)) + shiftRight;
    }
}
