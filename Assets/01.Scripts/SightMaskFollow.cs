using Unity.VisualScripting;
using UnityEngine;

public class SightMaskFollow : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    private RectTransform rectTransform;
    private Camera mainCamera;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        mainCamera = Camera.main;
    }

    private void LateUpdate()
    {
        if(playerTransform != null&& mainCamera != null)
        {
            Vector2 screenPos = mainCamera.WorldToScreenPoint(playerTransform.position);
            rectTransform.position = screenPos;
        }
    }
}
