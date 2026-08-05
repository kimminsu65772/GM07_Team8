using System;

public class RuntimeDataContext
{
    public PlayerSaveData SaveData { get; }

    public WalletManager Wallet { get; }
    public StageProgressManager Stage { get; }

    public RuntimeDataContext(PlayerSaveData saveData, WalletManager wallet, StageProgressManager stage)
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

        if (wallet == null)
        {
            throw new ArgumentNullException(nameof(wallet), "WalletManager가 초기화되지 않았습니다.");
        }

        if (stage == null)
        {
            throw new ArgumentNullException(nameof(stage), "StageProgressManager가 초기화되지 않았습니다.");
        }

        Wallet = wallet;
        Stage = stage;

        Wallet.Initialize(saveData.Wallet);
        Stage.Initialize(saveData.StageProgress);
    }
}
