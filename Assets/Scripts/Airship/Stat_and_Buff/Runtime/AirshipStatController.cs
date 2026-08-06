using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 스탯을 업그레이드 상태와 버프 등을 한곳에서 <br/>
/// 처리 및 계산 하기 위한 클래스.
/// </summary>
public class AirshipStatController : MonoBehaviour
{
    [SerializeField] private AirshipStatTable statTable;

    private readonly List<AirshipBuff> activeBuffs = new List<AirshipBuff>();

    public AirshipRuntimeStats CurrentStats { get; private set; } = new AirshipRuntimeStats();

    public event Action<AirshipRuntimeStats> OnStatsChanged;

    private AirshipUpgradeState upgradeState;

    public void Init(AirshipUpgradeState state)
    {
        upgradeState = state;
        Recalculate();  
    }

    private void Update()
    {
        TickBuffs();
    }

    public void AddBuff(AirshipBuff buff)
    {
        if (buff == null)
        {
            return;
        }

        activeBuffs.Add(buff);
        Recalculate();
    }

    // 특정 주체가 부여한 버프를 모두 제거한다. 예: 영웅 사망, 장비 해제.
    public void RemoveBuffsByOwner(object owner)
    {
        activeBuffs.RemoveAll(buff => buff.Owner == owner);
        Recalculate();
    }

    public void Recalculate()
    {
        if (statTable == null || upgradeState == null)
        {
            return;
        }

        CalculateStat(AirshipStatType.Attack, upgradeState.AttackLevel);
        CalculateStat(AirshipStatType.Defense, upgradeState.DefenseLevel);
        CalculateStat(AirshipStatType.MaxHealth, upgradeState.MaxHealthLevel);
        CalculateStat(AirshipStatType.CriticalChance, upgradeState.CriticalLevel);
        CalculateStat(AirshipStatType.MoveSpeed, 0);
        CalculateStat(AirshipStatType.AttackSpeed, 0);

        OnStatsChanged?.Invoke(CurrentStats);
    }

    private void TickBuffs()
    {
        bool removedExpiredBuff = false;

        for (int i = activeBuffs.Count - 1; i >= 0; i--)
        {
            AirshipBuff buff = activeBuffs[i];
            buff.Tick(Time.deltaTime);

            if (!buff.HasExpired)
            {
                continue;
            }

            activeBuffs.RemoveAt(i);
            removedExpiredBuff = true;
        }

        if (removedExpiredBuff)
        {
            Recalculate();
        }
    }

    private void CalculateStat(AirshipStatType statType, int level)
    {
        float value = statTable.GetStatValue(statType, level);

        float flatSum = 0f;
        float percentAddSum = 0f;
        float percentMultiply = 1f;

        foreach (AirshipBuff buff in activeBuffs)
        {
            foreach (AirshipStatModifier modifier in buff.Modifiers)
            {
                if (modifier.StatType != statType)
                {
                    continue;
                }

                switch (modifier.ModifierType)
                {
                    case AirshipModifierType.Flat:
                        flatSum += modifier.Value;
                        break;

                    case AirshipModifierType.PercentAdd:
                        percentAddSum += modifier.Value;
                        break;

                    case AirshipModifierType.PercentMultiply:
                        percentMultiply *= modifier.Value;
                        break;
                }
            }
        }

        value = (value + flatSum) * (1f + percentAddSum) * percentMultiply;
        CurrentStats.SetStat(statType, value);
    }
}