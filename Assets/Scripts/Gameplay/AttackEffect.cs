using System.Collections;
using UnityEngine;

public class AttackEffect : MonoBehaviour
{
    [SerializeField] GameObject attackEffectRefPoint;
    [SerializeField] GameObject attackPrefab;

    public void SpawnAttack(int num)
    {
        StartCoroutine(SpawnAttackCoroutine(num));
    }

    private IEnumerator SpawnAttackCoroutine(int num)
    {
        var refTransform = attackEffectRefPoint.transform;
        for (; num > 0; num--)
        {
            Quaternion rot = refTransform.rotation * Quaternion.Euler(0f, 0f, Random.value * 5f);
            Instantiate(attackPrefab, refTransform.position, rot);
            yield return new WaitForSeconds(0.016f);
        }
    }
}
