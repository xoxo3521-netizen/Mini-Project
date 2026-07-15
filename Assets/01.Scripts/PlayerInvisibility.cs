using UnityEngine;
using System.Collections;

public class PlayerInvisibility : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    private bool _isInvincible;

    public bool IsInvincible => _isInvincible;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void TriggerInvincibility(float duration = 1.0f, float flashInterval = 0.1f)
    {
        StartCoroutine(InvincibilityRoutine(duration, flashInterval));
    }

    private IEnumerator InvincibilityRoutine(float duration, float flashInterval)
    {
        _isInvincible = true;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            _spriteRenderer.color = new Color(1f, 1f, 1f, 0.3f);
            yield return new WaitForSeconds(flashInterval);
            _spriteRenderer.color = new Color(1f, 1f, 1f, 1f);
            yield return new WaitForSeconds(flashInterval);
            elapsed += (flashInterval * 2);
        }

        _spriteRenderer.color = Color.white;
        _isInvincible = false;
    }
}
