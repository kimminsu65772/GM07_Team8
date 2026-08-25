using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "CurrencySO", menuName = "Game/Currency/CurrencySO")]
public class CurrencySO : ScriptableObject
{
    [SerializeField] private CurrencyType currencyType;
    [SerializeField] private Sprite currencyIcon;
    [SerializeField, TextArea] private string description;

    public CurrencyType CurrencyType => currencyType;
    public Sprite CurrencyIcon => currencyIcon;
    public string Description => description;
}
