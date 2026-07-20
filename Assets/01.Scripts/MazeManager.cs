using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;

public class MazeManager : MonoBehaviour
{
    public static MazeManager Instance { get; private set; }

    [Header("포탈 설정")]
    [SerializeField] private GameObject portalPrefab;
    [SerializeField] private List<Transform> portalSpawnPoints;

    [Header("UI")]
    [SerializeField] private TMP_Text missionText;
    [SerializeField] private GameObject gameOverPanel;

    [Header("시작 위치")]
    [SerializeField] private Transform startPoint;


    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        if(missionText != null )
        {
            missionText.text = "Complete the mission and go to the boss room on the 3rd floor.";
        }
 
        SpawnRandomPortal();
    }

    void SpawnRandomPortal()
    {
        if(portalPrefab == null || portalSpawnPoints == null || portalSpawnPoints.Count == 0)
        {
            Debug.LogWarning("포탈 프리팹 또는 스폰 위치가 설정되지 않았습니다.");
            return;
        }

        int randomIndex = Random.Range(0, portalSpawnPoints.Count);
        Transform targetPoint = portalSpawnPoints[randomIndex];

        GameObject portal = Instantiate(portalPrefab, targetPoint.position, Quaternion.identity);
        Debug.Log($"랜덤 포탈이 {targetPoint.name} 위치에 생성되었습니다.");
    }

    public void TriggerGameOver()
    {
        if(gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
        Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;

        if(PlayerController.Instance != null)
        {
            Destroy(PlayerController.Instance.gameObject);
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex-1);
    }
}
