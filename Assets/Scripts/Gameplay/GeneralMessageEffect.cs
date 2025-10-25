using System.Collections;
using TMPro;
using UnityEngine;

public class GeneralMessageEffect : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    TMP_Text textTMP;
    float holdDuration = 0.25f;
    float fadeDuration = 0.75f;
    Coroutine co;


    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        textTMP = GetComponent<TMP_Text>() ?? GetComponentInChildren<TMP_Text>();
        gameObject.SetActive(false);
    }

    public void Show()
    {
        if (co != null)
        {
            StopCoroutine(co);
        }
        gameObject.SetActive(true);

        //if (spriteRenderer) spriteRenderer.color = new Color(1, 1, 1, 1);
        //if (textTMP) textTMP.color = new Color(1, 1, 1, 1);
        co = StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        yield return new WaitForSeconds(holdDuration);
        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float a = 1f - Mathf.Clamp01(t / fadeDuration);

            if (spriteRenderer)
            {
                spriteRenderer.color = ChangeAlpha(spriteRenderer.color, a);
            }

            if (textTMP)
            {
                textTMP.color = ChangeAlpha(textTMP.color, a);
            }

            yield return null;
        }

        gameObject.SetActive(false);
    }

    public static Color ChangeAlpha(Color color, float a)
    {
        color.a = a;
        return color;
    }
}
