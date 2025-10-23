using UnityEngine;

public class EffectAtkMotion : MonoBehaviour
{
    float moveSpeed = 10f;
    float acceleration = 170f;

    [SerializeField] GameObject SpawnPointObj;


    private void Start()
    {
        float angle = transform.eulerAngles.z;
        if (angle > 180f)
        {
            angle -= 360f;
        }
        angle *= Mathf.Deg2Rad;
        float multiplier = 1 / Mathf.Max(Mathf.Abs(Mathf.Sin(angle)), 0.01f);

        moveSpeed *= multiplier;
        acceleration *= multiplier;

        Destroy(gameObject, GameConsts.garbageDelay);
    }

    void Update()
    {
        transform.Translate(Vector2.up * moveSpeed * Time.deltaTime);
        moveSpeed += acceleration * Time.deltaTime;
    }
}
