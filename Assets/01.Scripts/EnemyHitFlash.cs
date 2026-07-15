using UnityEngine;
using System.Collections;

public class EnemyHitFlash : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    private Coroutine _flashCoroutine;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void PlayFlash(float duration = 0.2f)
    {
        if(_flashCoroutine != null)
        {
            StopCoroutine(_flashCoroutine);
        }
        _flashCoroutine = StartCoroutine(HitFlashRoutine(duration));
    }

    private IEnumerator HitFlashRoutine(float duration)
    {
        _spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(duration);
        _spriteRenderer.color = Color.white;
    }
}
