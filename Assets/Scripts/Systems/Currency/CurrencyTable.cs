using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CurrencySO", menuName = "Game/Currency/CurrencyTable")]
public class CurrencyTable : ScriptableObject
{
    [SerializeField] private CurrencySO[] currencies;

    private Dictionary <CurrencyType, CurrencySO> currencyDictionary;

    private void OnEnable()
    {
        BuildCurrencyDict();
    }

    public CurrencySO GetCurrency(CurrencyType currencyType)
    {
        if (currencyDictionary == null)
        {
            BuildCurrencyDict();
            if (currencyDictionary == null)
            {
                Debug.LogError("재화 딕셔너리를 생성할 수 없습니다.");
                return null;
            }
        }
        if (!currencyDictionary.TryGetValue(currencyType, out var currency))
        {
            Debug.LogError($"재화 타입 {currencyType}에 해당하는 재화를 찾을 수 없습니다.");
            return null;
        }
        return currency;
    }

    private void BuildCurrencyDict()
    {
        currencyDictionary = new Dictionary<CurrencyType, CurrencySO>();
        if (currencies == null || currencies.Length <= 0)
        {
            Debug.LogError("재화 테이블이 비어 있습니다.");
            return;
        }

        foreach (var currency in currencies)
        {
            if (currency == null)
            {
                Debug.LogError("재화 테이블에 null 값이 있습니다.");
                continue;
            }

            if (currencyDictionary.ContainsKey(currency.CurrencyType))
            {
                Debug.LogError($"재화 테이블에 중복된 재화 타입이 있습니다: {currency.CurrencyType}");
                continue;
            }

            currencyDictionary[currency.CurrencyType] = currency;
        }
    }
}
