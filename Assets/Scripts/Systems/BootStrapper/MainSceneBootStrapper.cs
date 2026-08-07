using System;
using UnityEngine;

/// <summary>
/// 방치형 게임이 진행되는 메인 씬에 진입할 때,
/// 런타임 데이터를 필요로 하는 각종 매니저(시스템)들을 초기화하는 역할을 담당한다.
/// 이를 통해 메인 씬에만 존재하는 핵심 시스템들이 런타임 데이터를 유지할 수 있도록 보장한다.
/// </summary>

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
