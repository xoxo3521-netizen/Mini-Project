using UnityEngine;
using TMPro;

public class MazeManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text missionText;

    void Start()
    {
        if(missionText != null )
        {
            missionText.text = "미션을 완료하여 3층 보스방으로 가세요.";
        }
    }
}
