/// <summary>
/// 업그레이드 상태를 나타내는 클래스.
/// </summary>
public class AirshipUpgradeState
{
    public int AttackLevel { get; private set; }
    public int DefenseLevel { get; private set; }
    public int MaxHealthLevel { get; private set; }
    public int CriticalLevel { get; private set; }

    public void SetLevels(
        int attackLevel,
        int defenseLevel,
        int maxHealthLevel,
        int criticalLevel)
    {
        AttackLevel = attackLevel;
        DefenseLevel = defenseLevel;
        MaxHealthLevel = maxHealthLevel;
        CriticalLevel = criticalLevel;
    }

    public void IncreaseStatLevel(AirshipStatType statType)
    {
        switch (statType)
        {
            case AirshipStatType.Attack:
                AttackLevel++;
                break;
            case AirshipStatType.Defense:
                DefenseLevel++;
                break;
            case AirshipStatType.MaxHealth:
                MaxHealthLevel++;
                break;
            case AirshipStatType.CriticalChance:
                CriticalLevel++;
                break;
        }
    }
}