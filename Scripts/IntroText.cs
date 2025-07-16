using UnityEngine;

public class IntroText : MonoBehaviour
{
    public SpriteRenderer[] targets;
    public float blinkSpeed = 1f;
    public float minAlpha = 0f;
    public float maxAlpha = 1f;

    private float timer = 0f;
    private bool fadingOut = true;

    void Update()
    {
        timer += Time.deltaTime * blinkSpeed;

        float alpha;
        if (fadingOut)
        {
            alpha = Mathf.Lerp(maxAlpha, minAlpha, timer);
            if (alpha <= minAlpha)
            {
                alpha = minAlpha;
                fadingOut = false;
                timer = 0f;
            }
        }
        else
        {
            alpha = Mathf.Lerp(minAlpha, maxAlpha, timer);
            if (alpha >= maxAlpha)
            {
                alpha = maxAlpha;
                fadingOut = true;
                timer = 0f;
            }
        }

        foreach (var sprite in targets)
        {
            if (sprite != null)
            {
                Color color = sprite.color;
                color.a = alpha;
                sprite.color = color;
            }
        }
    }
}

