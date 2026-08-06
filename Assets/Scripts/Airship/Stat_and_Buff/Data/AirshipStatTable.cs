using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 업그레이드 할때 레벨당 증가량. <br/>
/// 스탯 테이블SO를 구성하기 위한 값.
/// </summary>
[Serializable]
public class AirshipStatGrowthData
{
    [SerializeField] private AirshipStatType statType;
    [SerializeField] private float baseValue;
    [SerializeField] private float growthValue;

    public AirshipStatType StatType => statType;

    public float GetValue(int level)
    {
        return baseValue + growthValue * level;
    }
}

/// <summary>
/// 스탯별 기본값과 레벨당 증가량을 정의하는 SO 데이터. <br/>
/// 테이블은 한개만 존재. <br/>
/// 나중에 최대레벨 같은게 추가될 수 있음.
/// </summary>
[CreateAssetMenu(menuName = "Airship/Stat Table")]
public class AirshipStatTable : ScriptableObject
{
    [SerializeField] private List<AirshipStatGrowthData> stats = new List<AirshipStatGrowthData>();

    public float GetStatValue(AirshipStatType statType, int level)
    {
        AirshipStatGrowthData stat = FindStat(statType);

        if (stat == null)
        {
            return 0f;
        }

        return stat.GetValue(level);
    }

    private AirshipStatGrowthData FindStat(AirshipStatType statType)
    {
        return stats.Find(stat => stat.StatType == statType);
    }
}

