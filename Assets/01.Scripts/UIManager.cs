using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance {  get; private set; }

    [Header("UI ¿¬°á")]
    public Slider hpSlider;
    public TextMeshProUGUI missionText;

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

    public void UpdateHPBar(float currentHp,float maxHp)
    {
        if(hpSlider != null)
        {
            hpSlider.value = currentHp / maxHp;
        }
    }

    public void UpdateMissionText(int currentKills, int targetKills)
    {
        if (missionText != null)
        {
            missionText.text = $"Slime Kills : {currentKills} / {targetKills}";
        }
    }
}
