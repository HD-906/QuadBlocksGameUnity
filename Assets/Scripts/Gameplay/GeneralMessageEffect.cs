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

    Color clr1 = Color.white, clr2 = Color.cyan, clr3 = Color.green;
    Color clr4 = new Color(1f, 0.5f, 0f, 1f), clr5 = Color.magenta;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        textTMP = GetComponent<TMP_Text>() ?? GetComponentInChildren<TMP_Text>();
        gameObject.SetActive(false);
    }

    public void ShowCombo(int comboNum)
    {
        if (comboNum == 0)
        {
            spriteRenderer.color = Color.yellow;
            return;
        }

        textTMP.text = $"{comboNum}\nCombo";
        spriteRenderer.color = IncrCombo(spriteRenderer.color);
        Show();
    }

    public void ShowClear(int lineCleared, int tSpinStatus)
    {
        (string text, Color color) result = (lineCleared, tSpinStatus) switch
        {
            (1, 0) => ("SINGLE", clr1),
            (2, 0) => ("DOUBLE", clr2),
            (3, 0) => ("TRIPLE", clr3),
            (4, 0) => ("QUADRUPLE", clr4),

            (1, 1) => ("T-SPIN\nMINI", clr1),
            (2, 1) => ("T-SPIN\nDOUBLE MINI", clr2),

            (1, 2) => ("T-SPIN\nSINGLE", clr3),
            (2, 2) => ("T-SPIN\nDOUBLE", clr4),
            (3, 2) => ("T-SPIN\nTRIPLE", clr5),

            _ => ("", Color.white)
        };

        textTMP.text = result.text;
        spriteRenderer.color = result.color;
        Show();
    }

    public void Show()
    {
        if (co != null)
        {
            StopCoroutine(co);
        }
        gameObject.SetActive(true);

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
        textTMP.color = ChangeAlpha(textTMP.color, 1);
        if (spriteRenderer != null)
        {
            spriteRenderer.color = ChangeAlpha(spriteRenderer.color, 1);
        }
    }

    public static Color ChangeAlpha(Color color, float a)
    {
        color.a = a;
        return color;
    }

    public static Color IncrCombo(Color color)
    {
        if (color.g >= 0.1f)
        {
            color.g -= 0.1f;
            return color;
        }

        if (color.b <= 0.9f)
        {
            color.b += 0.1f;
        }

        return color;
    }
}
