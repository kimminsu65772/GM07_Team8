using System;

/// <summary>
/// 세이브 데이터를 기반으로 런타임에서 사용되는 데이터 관리 매니저에 저장 데이터를 연결하는 역할을 수행하는 역할을 한다.
/// </summary>

public class RuntimeDataContext
{
    public PlayerSaveData SaveData { get; }

    public WalletManager Wallet { get; }
    public StageProgressManager Stage { get; }

    public AirshipUpgradeLevelManager AirshipUpgrade { get; }

    public RuntimeDataContext(PlayerSaveData saveData)
    {
        if(saveData == null)
        {
            throw new ArgumentNullException(nameof(saveData), "저장 데이터가 비어 있습니다.");
        }

        SaveData = saveData;

        if (SaveData.Wallet == null)
        {
            throw new ArgumentNullException(nameof(saveData.Wallet), "저장 데이터의 지갑 데이터가 비어 있습니다.");
        }

        if (SaveData.StageProgress == null)
        {
            throw new ArgumentNullException(nameof(saveData.StageProgress), "저장 데이터의 스테이지 진행 데이터가 비어 있습니다.");
        }

        WalletManager.Instance.Initialize(saveData.Wallet);
        StageProgressManager.Instance.Initialize(saveData.StageProgress);
        AirshipUpgradeLevelManager.Instance.Initialize(saveData.Airship);
    }
}
