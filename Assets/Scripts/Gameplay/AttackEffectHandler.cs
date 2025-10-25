using System.Collections;
using TMPro;
using UnityEngine;

public class AttackEffectHandler : MonoBehaviour
{
    [SerializeField] private GameObject attackEffectRefPoint;
    [SerializeField] private GameObject attackPrefab;

    [SerializeField] private GameObject comboObj;
    [SerializeField] private GameObject clearTypeObj;
    [SerializeField] private GameObject backToBackObj;
    [SerializeField] private GameObject perfectClear;

    GeneralMessageEffect comboEffect, clearTypeEffect, backToBackEffect, perfectClearEffect;

    private void Awake()
    {
        comboEffect = EnsureMessageEffect(comboObj);
        clearTypeEffect = EnsureMessageEffect(clearTypeObj);
        backToBackEffect = EnsureMessageEffect(backToBackObj);
        perfectClearEffect = perfectClear.GetComponent<GeneralMessageEffect>()
                          ?? perfectClear.AddComponent<GeneralMessageEffect>();
    }

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

    public void EnableUI(int lineClear, int tSpinStatus, bool backToBack, int comboNum, bool perfectClr)
    {
        if (perfectClr)
        {
            perfectClearEffect.enabled = true;
        }

        if (comboNum > 0)
        {
            comboObj.SetActive(true);
        }
    }

    private GeneralMessageEffect EnsureMessageEffect(GameObject go)
    {
        return go.GetComponent<GeneralMessageEffect>() 
            ?? go.AddComponent<GeneralMessageEffect>();
    }
}
