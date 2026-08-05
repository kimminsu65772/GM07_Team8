using System;
using UnityEngine;

public class WalletManager : MonoBehaviour
{
    public static WalletManager Instance { get; private set; }

    private WalletSaveData walletData;
    private bool isInitialized;

    public bool IsInitialized => isInitialized;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Initialize(WalletSaveData walletData)
    {
        if (walletData == null)
        {
            throw new ArgumentNullException(nameof(walletData), "지갑 데이터가 비어 있습니다.");
        }

        this.walletData = walletData;
        isInitialized = true;
    }

    public int GetAmount(CurrencyType type)
    {
        CheckInitialized();

        if (!walletData.Currencies.TryGetValue(type, out CurrencySaveData currency))
        {
            throw new ArgumentException($"정의되지 않은 타입의 재화입니다.");
        }

        return currency.Amount;
    }

    public bool TryAdd(CurrencyType type, int amount)
    {
        CheckInitialized();

        if (!walletData.Currencies.TryGetValue(type, out CurrencySaveData currency))
        {
            throw new ArgumentException($"정의되지 않은 타입의 재화입니다.");
        }
        if (amount < 0)
        {
            throw new ArgumentException("추가할 금액은 음수일 수 없습니다.");
        }
        currency.Amount += amount;
        return true;
    }

    public bool TrySpend(CurrencyType type, int amount)
    {
        CheckInitialized();

        if (!walletData.Currencies.TryGetValue(type, out CurrencySaveData currency))
        {
            throw new ArgumentException($"정의되지 않은 타입의 재화입니다.");
        }
        if (amount < 0)
        {
            throw new ArgumentException("사용할 금액은 음수일 수 없습니다.");
        }
        if (currency.Amount < amount)
        {
            return false;
        }
        currency.Amount -= amount;
        return true;
    }

    private void CheckInitialized()
    {
        if (!isInitialized)
        {
            throw new InvalidOperationException("WalletManager가 초기화되지 않았습니다.");
        }
    }
}
