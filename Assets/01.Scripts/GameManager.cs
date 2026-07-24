using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public enum GameMode { Normal, Hard }

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameMode currentMode = GameMode.Normal;

    [SerializeField] private int normalTargetKillCount = 30;
    [SerializeField] private int hardTargetKillCount = 60;

    [HideInInspector] public int targetKillCount;
    public int currentKillCount = 0;

    public GameObject portalPrefab;
    public Transform portalSpawnPoint;

    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject gameClearPanel;
    [SerializeField] private float returnDelay = 3f;

    [SerializeField] private AudioClip gameOverSound;
    [SerializeField] private AudioClip respawnCountSound;

    public GameObject playerObject;
    private AudioSource _audioSource;
    private bool isMissionComplete = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (GameData.Instance != null)
        {
            currentMode = GameData.Instance.selectedMode;
        }

        SetModeTargetKills();
    }

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();

        if (playerObject == null)
        {
            GameObject foundPlayer = GameObject.FindWithTag("Player");
            if (foundPlayer != null)
            {
                playerObject = foundPlayer;
            }
        }

        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateMissionText(currentKillCount, targetKillCount);
        }
    }

    private void SetModeTargetKills()
    {
        if (currentMode == GameMode.Normal)
        {
            targetKillCount = normalTargetKillCount;
        }
        else
        {
            targetKillCount = hardTargetKillCount;
        }
    }

    public void AddKill()
    {
        if (isMissionComplete) return;

        currentKillCount++;
        Debug.Log($"슬라임 처치! 현재 기록 : {currentKillCount} / {targetKillCount}");

        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateMissionText(currentKillCount, targetKillCount);
        }

        if (currentKillCount >= targetKillCount)
        {
            CompleteFloor1();
        }
    }

    void CompleteFloor1()
    {
        isMissionComplete = true;
        Debug.Log("미션 클리어! 다음 층으로 가는 포탈이 열립니다!");

        if (portalPrefab != null)
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
        Debug.Log("다음층으로 이동합니다");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void GameOver()
    {
        Debug.Log("Game Over");

        if (gameOverSound != null && _audioSource != null)
        {
            _audioSource.PlayOneShot(gameOverSound);
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        Time.timeScale = 0f;
    }

    public void ShowGameClear()
    {
        StartCoroutine(GameClearRoutine());
    }

    private IEnumerator GameClearRoutine()
    {
        if (gameClearPanel != null)
        {
            gameClearPanel.SetActive(true);
        }

        yield return new WaitForSeconds(returnDelay);
        SceneManager.LoadScene("MainMenuScene");
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;

        if (currentMode == GameMode.Hard)
        {
            SceneManager.LoadScene("MainMenuScene");
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
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

        PlayerController foundPlayer = FindFirstObjectByType<PlayerController>(FindObjectsInactive.Include);
        if (foundPlayer != null)
        {
            playerObject = foundPlayer.gameObject;
        }

        if (playerObject != null)
        {
            playerObject.SetActive(true);
            Vector3 spawnPos = portalSpawnPoint != null ? portalSpawnPoint.position : Vector3.zero;
            playerObject.transform.position = spawnPos;

            PlayerController playerController = playerObject.GetComponent<PlayerController>();
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
        else
        {
            Debug.LogError("리스폰 실패: 플레이어 오브젝트를 찾지 못했습니다!");
        }

        Debug.Log("리스폰 완료");
    }

    public void PlayerDeath()
    {
        if (currentMode == GameMode.Normal)
        {
            StartCoroutine(NormalRespawnRoutine());
        }
        else
        {
            if (gameOverSound != null && _audioSource != null)
            {
                _audioSource.PlayOneShot(gameOverSound);
            }

            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(true);
            }

            Debug.Log("[하드 모드] Game Over");
            Time.timeScale = 0f;
        }
    }
}