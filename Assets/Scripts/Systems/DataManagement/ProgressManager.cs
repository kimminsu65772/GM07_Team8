using System;

/// <summary>
/// ProgressManager는 현재 진행 데이터를 관리하는 클래스이다.
/// 다른 시스템으로부터 명령을 받아 진행 데이터를 업데이트하거나, 조회할 수 있는 기능을 제공한다.
/// </summary>
public class ProgressManager
{
    public PlayerSaveData CurrentData { get; private set; }

    private readonly SaveDataWriter saveDataWriter;

    public ProgressManager(PlayerSaveData initialData, SaveDataWriter writer)
    {
        CurrentData = initialData;
        if (CurrentData == null)
        {
            throw new ArgumentNullException(nameof(initialData), "전달받은 진행 데이터가 비어있습니다.");
        }
    }

    // 재화 추가 메서드
    public void AddCurrency(string currencyId, int amount)
    {
        if (CurrentData.Wallet.Currencies.ContainsKey(currencyId))
        {
            CurrentData.Wallet.Currencies[currencyId].Amount += amount;
        }
        else
        {
            throw new ArgumentException($"'{currencyId}'는 존재하지 않는 재화입니다.");
        }
    }

    public bool TrySpendCurrency(string currencyId, int amount)
    {
        if (CurrentData.Wallet.Currencies.ContainsKey(currencyId))
        {
            if (CurrentData.Wallet.Currencies[currencyId].Amount >= amount)
            {
                CurrentData.Wallet.Currencies[currencyId].Amount -= amount;
                return true;
            }
            else
            {
                return false; // 잔액 부족
            }
        }
        else
        {
            throw new ArgumentException($"'{currencyId}'는 존재하지 않는 재화입니다.");
        }
    }

    public void SetCurrentStage(int stage)
    {
        if (stage < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(stage), "스테이지는 1 이상이어야 합니다.");
        }
        CurrentData.StageProgress.CurrentStage = stage;
    }

    public void UpdateMaxClearedStage(int stage)
    {
        if (stage < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(stage), "스테이지는 1 이상이어야 합니다.");
        }
        if (stage > CurrentData.StageProgress.MaxClearedStage)
        {
            CurrentData.StageProgress.MaxClearedStage = stage;
        }
    }
}
