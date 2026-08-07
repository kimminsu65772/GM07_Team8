using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class UpgradeView : MonoBehaviour
{
    public TextMeshProUGUI statNameLevelText;
    public TextMeshProUGUI currentValueText;
    public TextMeshProUGUI costText;
    public Button upgradeButton;

    //임시 데이터
    private int currentLevel = 1;
    private int currentPower = 10;
    private int upgradeCost = 50;
    void Start()
    {
        gameObject.SetActive(false);

        //초기 UI 설정
        UpdateView();


        //버튼 클릭 인벤트 연결
        upgradeButton.onClick.AddListener(OnUpgradeButtonClicked);
    }
    //수치 갱신
    private void UpdateView()
    {
        statNameLevelText.text = $"공격력 LV.{currentLevel}";
        currentValueText.text = $"현재 수치: {currentPower}";
        costText.text = $"비용 {upgradeCost}";
    }
    //버튼 눌렀을때 실행
    private void OnUpgradeButtonClicked()
    {
        Debug.Log("강화 버튼 클릭");
        // 1. 골드가 충분한지 확인 (if (GameManager.Instance.Gold >= upgradeCost))
        // 2. 실제 스탯 데이터 증가 (currentLevel++, currentPower += 2)
        // 3. 골드 차감 (GameManager.Instance.Gold -= upgradeCost)

        currentLevel++;
        currentPower += 2;
        upgradeCost += 20;

        UpdateView();
    }
}
