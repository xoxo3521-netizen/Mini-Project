using UnityEngine;
using System.Collections;
using TMPro;


public class TextTimer : MonoBehaviour
{
    private TMP_Text textComponet;

    [Header("시간 설정")]
    [SerializeField] private float showDuration = 3f;
    [SerializeField] private float hideDuration = 15f;

    private void Awake()
    {
        textComponet = GetComponent<TMP_Text>();
    }

    private void Start()
    {
        StartCoroutine(ToggleTextRoutine());
    }

    IEnumerator ToggleTextRoutine()
    {
        textComponet.enabled = true;

        while(true)
        {
            yield return new WaitForSeconds(hideDuration);
            textComponet.enabled = true;
            yield return new WaitForSeconds(showDuration);
            textComponet.enabled = false;
        }
    }
}
