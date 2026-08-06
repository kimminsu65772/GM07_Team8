using UnityEngine;

public class MainSceneBootStrapper : MonoBehaviour
{
    [SerializeField] AirshipUpgradeManager airshipUpgradeManager;

    private void Start()
    {
        if (airshipUpgradeManager == null)
        {
            Debug.LogError("AirshipUpgradeManager가 할당되지 않았습니다.");
            return;
        }

        airshipUpgradeManager.Init(AirshipUpgradeLevelManager.Instance.GetAirshipLevelData());
    }
}
