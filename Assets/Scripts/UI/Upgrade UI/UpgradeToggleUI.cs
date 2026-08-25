using UnityEngine;
using UnityEngine.UI;
public class UpgradeToggleUI : MonoBehaviour
{
    [Header("버튼 컴포넌트들")]
    public Button btnPlus1;
    public Button btnPlus10;
    public Button btnPlus100;

    [Header("선택되었을 때 켤 이미지 오브젝트들 (GameObject)")]
    public GameObject checkImage1;
    public GameObject checkImage10;
    public GameObject checkImage100;

    void Start()
    {
        SetUpgradeMode(1);

        if (btnPlus1 != null) btnPlus1.onClick.AddListener(() => SetUpgradeMode(1));
        if (btnPlus10 != null) btnPlus10.onClick.AddListener(() => SetUpgradeMode(10));
        if (btnPlus100 != null) btnPlus100.onClick.AddListener(() => SetUpgradeMode(100));
    }
    private void SetUpgradeMode(int mode)
    {
        if (checkImage1 != null) checkImage1.SetActive(false);
        if (checkImage10 != null) checkImage10.SetActive(false);
        if (checkImage100 != null) checkImage100.SetActive(false);
        switch (mode)
        {
            case 1:
                if (checkImage1 != null) checkImage1.SetActive(true);
                break;
            case 10:
                if (checkImage10 != null) checkImage10.SetActive(true);
                break;
            case 100:
                if (checkImage100 != null) checkImage100.SetActive(true);
                break;
        }
    }
}