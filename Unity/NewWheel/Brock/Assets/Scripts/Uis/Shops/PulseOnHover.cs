using UnityEngine;

public class PulseOnHover : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Coroutine pulseCoroutine;

    [Range(0f, 1f)] public float minAlpha = 0.3f;
    [Range(0f, 1f)] public float maxAlpha = 1f;
    public float pulseSpeed = 2f;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        SetAlpha(minAlpha); // Optional: start semi-transparent
    }

    private void OnMouseEnter()
    {
        if (WeaponUiManager.Instance.currentlyDragging != null)
        {
            StartPulse();
            WeaponUiManager.Instance.SetHoverTarget(this);
        }
    }

    private void OnMouseExit()
    {
        StopPulse();
        SetAlpha(minAlpha); // Reset to resting transparency
        WeaponUiManager.Instance.ClearHoverTarget(this);
    }

    private void StartPulse()
    {
        if (pulseCoroutine != null)
            StopCoroutine(pulseCoroutine);

        pulseCoroutine = StartCoroutine(PulseAlpha());
    }

    public void StopPulse()
    {
        if (pulseCoroutine != null)
        {
            StopCoroutine(pulseCoroutine);
            pulseCoroutine = null;
        }
    }

    private System.Collections.IEnumerator PulseAlpha()
    {
        float t = 0f;
        bool increasing = true;

        while (true)
        {
            float alpha = Mathf.Lerp(minAlpha, maxAlpha, t);
            SetAlpha(alpha);

            t += (increasing ? 1 : -1) * Time.deltaTime * pulseSpeed;

            if (t >= 1f)
            {
                t = 1f;
                increasing = false;
            }
            else if (t <= 0f)
            {
                t = 0f;
                increasing = true;
            }

            yield return null;
        }
    }

    private void SetAlpha(float alpha)
    {
        Color c = spriteRenderer.color;
        c.a = alpha;
        spriteRenderer.color = c;
    }
}
