using UnityEngine;

public class BossRoomEntrance : MonoBehaviour
{
    [Header("연결 설정")]
    [SerializeField] private BossSpawnController bossManager;

    private bool triggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(!triggered && other.CompareTag("Player"))
        {
            triggered = true;

            if(bossManager != null)
            {
                bossManager.StartBossFight();

                gameObject.SetActive(false);
            }
            else
            {
                Debug.LogWarning("BossSpawnController가 연결되어있지 않습니다.");
            }
        }
    }
}
