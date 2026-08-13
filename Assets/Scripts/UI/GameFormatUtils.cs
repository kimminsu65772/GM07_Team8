using UnityEngine;
public static class GameFormatUtils
{
    private static readonly string[] Units = { "", "a", "b", "c", "d", "e", "f", "g", "h", "i", "j" };

    public static string ToPercent(float value, int decimalPlaces = 0)
    {
        float percentValue = value * 100f;
        return $"{percentValue.ToString($"F{decimalPlaces}")}%";
    }

    public static string ToIdleNumber(double value)
    {
        if (value < 1000)
        {
            return value.ToString("N0"); 
        }

        int unitIndex = 0;
        while (value >= 1000 && unitIndex < Units.Length - 1)
        {
            value /= 1000;
            unitIndex++;
        }

        return $"{value:F2}{Units[unitIndex]}";
    }


    public static string FormatStatValue(AirshipStatType statType, float rawValue)
    {
        switch (statType)
        {
            case AirshipStatType.CriticalChance:
                return ToPercent(rawValue);

            case AirshipStatType.Attack:
            case AirshipStatType.Defense:
            case AirshipStatType.MaxHealth:
                return ToIdleNumber(rawValue);

            default:
                return rawValue.ToString("N0");
        }
    }
}