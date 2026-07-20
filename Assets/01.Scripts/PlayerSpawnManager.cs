using UnityEngine;

public class PlayerSpawnManager : MonoBehaviour
{
    [Header("플레이어 스폰 설정")]
    [SerializeField] private Transform playerSpawnPoint;

    private void Start()
    {
        GameObject player = GameObject.FindWithTag("Player");

        if(player != null && playerSpawnPoint != null)
        {
            player.transform.position = playerSpawnPoint.position;
            Debug.Log("플레이어가 3층 스폰 위치에 이동되었습니다.");
        }
        else
        {
            Debug.Log("플레이어나 스폰 포인트를 찾지 못했습니다.");
        }
    }
}
