using UnityEngine;
using UnityEngine.UI;

public class BossHPBarUI : MonoBehaviour
{
    private Slider bossSlider;
    private BossController bossController;

    private void Start()
    {
        bossController = GetComponent<BossController>();

        GameObject sliderObj = GameObject.Find("BossHPBar");
        if(sliderObj != null )
        {
            bossSlider = sliderObj.GetComponent<Slider>();
        }
        else
        {
            Debug.LogWarning(" 슬라이더를 찾지 못했습니다. ");
        }
    }

    private void Update()
    {
        if (bossController == null || bossSlider == null) return;

        bossSlider.maxValue = bossController.MaxHp;
        bossSlider.value = bossController.CurrentHp;
    }

}
