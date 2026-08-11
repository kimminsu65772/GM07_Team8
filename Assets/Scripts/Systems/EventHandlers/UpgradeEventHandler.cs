using UnityEngine;

public class UpgradeEventHandler : MonoBehaviour
{
    [SerializeField] private AirshipUpgradeManager airshipUpgradeManager;
    [SerializeField] private AirshipEquipmentController airshipEquipmentController;

    private void OnEnable()
    {
        if (airshipUpgradeManager != null)
        {
            airshipUpgradeManager.OnUpgradeChanged -= HandleUpgradeChanged;
            airshipUpgradeManager.OnUpgradeChanged += HandleUpgradeChanged;
        }

        if (airshipEquipmentController != null)
        {
            airshipEquipmentController.OnCannonChanged -= HandleCannonChanged;
            airshipEquipmentController.OnGearChanged -= HandleGearChanged;
            airshipEquipmentController.OnCannonChanged += HandleCannonChanged;
            airshipEquipmentController.OnGearChanged += HandleGearChanged;
        }
    }

    public void HandleUpgradeChanged(AirshipUpgradeState upgradeState)
    {
        Debug.Log("업그레이드 상태가 변경되었습니다.");
        // 예: UI 업데이트, 저장 요청 등
        PlayerInfo.Instance.SetAirshipUpgradeState(upgradeState);
    }

    public void HandleCannonChanged (AirshipCannonData cannonData)
    {
        // 예: UI 업데이트, 저장 요청 등
        string cannonId = cannonData != null ? cannonData.Id : null;
        PlayerInfo.Instance.SetEquippedCannonId(cannonId);
    }

    public void HandleGearChanged (AirshipGearData gearData)
    {
        // 예: UI 업데이트, 저장 요청 등
        string gearId = gearData != null ? gearData.Id : null;
        PlayerInfo.Instance.SetEquippedGearId(gearId);
    }
}
