using UnityEngine;
using System.Collections;

public class EnemyFadeOut : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void PlayFadeOut(float duration, System.Action onComplete)
    {
        StartCoroutine(FadeOutRoutine(duration, onComplete));
    }

    private IEnumerator FadeOutRoutine(float duration, System.Action onComplete)
    {
        float elapsed = 0f;
        Color currentColor = _spriteRenderer.color;

        while(elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f,0f, elapsed/duration);
            _spriteRenderer.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
            yield return null;
        }

        if(onComplete != null)
        {
            onComplete();
        }
    }
}
