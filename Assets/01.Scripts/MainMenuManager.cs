using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("모드 선택 UI 패널")]
    [SerializeField] private GameObject difficultyPanel;

    public void OnClickGameStart()
    {
        if(difficultyPanel != null)
        {
            difficultyPanel.SetActive(true);
        }
    }

    public void OnClickNormalMode()
    {
        if (GameData.Instance != null)
        {
            GameData.Instance.selectedMode = GameMode.Normal;
        }
        SceneManager.LoadScene("Floor1");
    }

    public void OnClickHardMode()
    {
        if (GameData.Instance != null)
        {
            GameData.Instance.selectedMode = GameMode.Hard;
        }
        SceneManager.LoadScene("Floor1");
    }
    public void QuitGame()
    {
        Debug.Log("게임 종료");
        Application.Quit();
    }
}
