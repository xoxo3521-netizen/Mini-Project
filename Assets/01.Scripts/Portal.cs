using UnityEngine;
using UnityEngine.SceneManagement;

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
            else
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
                Debug.Log("다음 층으로 이동합니다.");
            }
        }
    }
}
