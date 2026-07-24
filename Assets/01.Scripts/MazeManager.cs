using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

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

    [Header("사운드 설정")]
    [SerializeField] private AudioClip respawnCountSound;
    private AudioSource _audioSource;

    private void Awake()
    {
        if (Instance == null)
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
        _audioSource = GetComponent<AudioSource>();

        if (missionText != null)
        {
            missionText.text = "Complete the mission and go to the boss room on the 3rd floor.";
        }

        SpawnRandomPortal();
    }

    void SpawnRandomPortal()
    {
        if (portalPrefab == null || portalSpawnPoints == null || portalSpawnPoints.Count == 0)
        {
            Debug.LogWarning("포탈 프리팹 또는 스폰 위치가 설정되지 않았습니다.");
            return;
        }

        int randomIndex = Random.Range(0, portalSpawnPoints.Count);
        Transform targetPoint = portalSpawnPoints[randomIndex];

        Instantiate(portalPrefab, targetPoint.position, Quaternion.identity);
        Debug.Log($"랜덤 포탈이 {targetPoint.name} 위치에 생성되었습니다.");
    }

    public void TriggerGameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
        Time.timeScale = 0f;
    }

    public IEnumerator NormalRespawnRoutine()
    {
        int count = 3;

        if (respawnCountSound != null && _audioSource != null)
        {
            _audioSource.PlayOneShot(respawnCountSound);
        }

        while (count > 0)
        {
            Debug.Log($"리스폰 카운트 : {count}");
            yield return new WaitForSeconds(1f);
            count--;
        }

        GameObject playerObj = null;
        PlayerController foundPlayer = FindFirstObjectByType<PlayerController>(FindObjectsInactive.Include);
        if (foundPlayer != null)
        {
            playerObj = foundPlayer.gameObject;
        }

        if (playerObj != null)
        {
            playerObj.SetActive(true);
            Vector3 spawnPos = startPoint != null ? startPoint.position : Vector3.zero;
            playerObj.transform.position = spawnPos;

            PlayerController playerController = playerObj.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.ResetHealth();
            }

            if (Camera.main != null)
            {
                Camera.main.transform.position = new Vector3(spawnPos.x, spawnPos.y, -10f);

                CameraMove cam = Camera.main.GetComponent<CameraMove>();
                if (cam != null)
                {
                    cam.FindPlayer();
                }
            }
        }

        Debug.Log("2층 리스폰 완료");
    }

    public void PlayerDeath()
    {
        GameMode currentMode = GameMode.Normal;
        if (GameData.Instance != null)
        {
            currentMode = GameData.Instance.selectedMode;
        }

        if (currentMode == GameMode.Normal)
        {
            StartCoroutine(NormalRespawnRoutine());
        }
        else
        {
            TriggerGameOver();
            Debug.Log("[2층 하드 모드] Game Over");
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;

        GameMode currentMode = GameMode.Normal;
        if (GameData.Instance != null)
        {
            currentMode = GameData.Instance.selectedMode;
        }

        if (currentMode == GameMode.Hard)
        {
            SceneManager.LoadScene("MainMenuScene");
        }
        else
        {
            if (PlayerController.Instance != null)
            {
                Destroy(PlayerController.Instance.gameObject);
            }
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}