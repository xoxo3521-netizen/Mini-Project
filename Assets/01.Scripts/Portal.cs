using UnityEngine;

public class Portal : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            Debug.Log("플레이어가 포탈에 진입했습니다.");
            if(GameManager.Instance != null )
            {
                GameManager.Instance.GoToNextFloor();
            }
        }
    }
}
