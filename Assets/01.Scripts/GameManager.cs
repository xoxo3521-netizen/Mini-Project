using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance {  get; private set; }

    [Header("1층 미션 설정")]
    public int targetKillCount = 10;
    public int currentKillCount = 0;

    [Header("포탈 설정")]
    public GameObject portalPrefab;
    public Transform portalSpawnPoint;

    private bool isMissionComplete = false;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddKill()
    {
        if(isMissionComplete)
        {
            return;
        }

        currentKillCount++;
        Debug.Log($"슬라임 처치! 현재 기록 : {currentKillCount} / {targetKillCount}");

        if(currentKillCount >= targetKillCount)
        {
            CompleteFloor1();
        }
    }

    void CompleteFloor1()
    {
        isMissionComplete = true;
        Debug.Log("1층 미션 클리어! 다음 층으로 가는 포탈이 열립니다! ");

        if(portalPrefab != null)
        {
            Vector3 spawnPos = portalSpawnPoint != null ? portalSpawnPoint.position : Vector3.zero;
            Instantiate(portalPrefab, spawnPos, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("포탈 프리팹이 없어 바로 다음 층으로 이동합니다.");
            GoToNextFloor();
        }
    }

    public void GoToNextFloor()
    {
        Debug.Log("2층으로 이동합니다");

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
