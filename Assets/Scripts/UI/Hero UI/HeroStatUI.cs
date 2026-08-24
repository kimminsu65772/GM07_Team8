using UnityEngine;
using TMPro;

public class HeroStatUI : MonoBehaviour
{
    private enum ValueFormat
    {
        Number,
        Percent,
        Seconds
    }

    [SerializeField] private TMP_Text valueText;
    [SerializeField] private ValueFormat valueFormat;

    private void Awake()
    {
        if (valueText == null)
        {
            valueText = GetComponentInChildren<TMP_Text>(true);
        }
    }

    public void SetValue(double value)
    {
        if (valueText == null)
        {
            return;
        }

        switch (valueFormat)
        {
            case ValueFormat.Percent:
                valueText.text = GameFormatUtils.ToPercent((float)value);
                break;
            case ValueFormat.Seconds:
                valueText.text = $"{value:F1}초";
                break;
            default:
                valueText.text = GameFormatUtils.ToIdleNumber(value);
                break;
        }
    }

    public void SetValue(int value)
    {
        if (valueText != null)
        {
            valueText.text = value.ToString();
        }
    }

    public void Clear()
    {
        if (valueText != null)
        {
            valueText.text = string.Empty;
        }
    }
}
