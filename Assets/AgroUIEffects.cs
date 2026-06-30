using UnityEngine;
using System.Collections;

public class AgroUIEffects : MonoBehaviour
{
    [Header("Futuristic Laser Scan Line")]
    public RectTransform scanLine;
    public float scanSpeed = 300f;
    public float topYAnchor = 400f;
    public float bottomYAnchor = -400f;

    private bool movingDown = true;

    void Update()
    {
        if (scanLine == null) return;

        // Moves a glowing horizontal laser bar up and down the viewfinder smoothly
        float step = scanSpeed * Time.deltaTime;
        Vector3 currentPos = scanLine.localPosition;

        if (movingDown)
        {
            currentPos.y -= step;
            if (currentPos.y <= bottomYAnchor) movingDown = false;
        }
        else
        {
            currentPos.y += step;
            if (currentPos.y >= topYAnchor) movingDown = true;
        }

        scanLine.localPosition = currentPos;
    }

    // Call this method via UI Buttons to cleanly transition panels without sudden pops
    public void CrossFadePanel(CanvasGroup panelToFade, float targetAlpha, float duration)
    {
        StartCoroutine(FadeRoutine(panelToFade, targetAlpha, duration));
    }

    private IEnumerator FadeRoutine(CanvasGroup group, float target, float time)
    {
        float start = group.alpha;
        float elapsed = 0f;
        while (elapsed < time)
        {
            elapsed += Time.deltaTime;
            group.alpha = Mathf.Lerp(start, target, elapsed / time);
            yield return null;
        }
        group.alpha = target;
    }
}