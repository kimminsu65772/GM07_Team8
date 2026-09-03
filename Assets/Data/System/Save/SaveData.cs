using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

public enum CurrencyType
{
    Gold = 0,
    Gear = 1,
    Gems = 2

    // 나중에 추가될 재화 타입이 있을 수 있음.
}

/// <summary>
/// SaveData.cs 파일은 플레이어의 종합적인 세이브 데이터와
/// 해당 데이터를 구성하는 여러 하위 데이터 구조를 정의해놓은 파일이다.
/// </summary>
[Serializable]
public class PlayerSaveData
{
    // 세이브 기능의 저장 버전
    // 게임이 확장되어 추가되는 재화나 기타 데이터들이 있을 경우, 이전 버전의 세이브 데이터가 호환되지 않을 수 있음.
    // 만약 호환되지 않는다면, 이전 버전의 세이브 데이터 + 새로운 버전에서 요구하는 변경사항을 적용하여 데이터를 변환할 수 있도록 구분하는 용도
    // 지금 당장 사용하는 필드는 아님.
    public int SaveVersion { get; set; }
    // 플레이어의 ID와 닉네임을 저장하는 데이터
    public PlayerProfileSaveData Profile { get; set; }
    public AirshipSaveData Airship { get; set; }
    public Dictionary<HeroNameEnum, HeroSaveData> Heroes { get; set; }
    public HeroFormationSaveData HeroFormation { get; set; }
    public StageProgressSaveData StageProgress { get; set; }
    // 플레이어가 보유한 재화 묶음 데이터를 저장하는 데이터
    public WalletSaveData Wallet { get; set; }
    public InventorySaveData Inventory { get; set; }
    public EquipmentInventorySaveData EquipmentInventory { get; set; }

    public EquipmentCraftSaveData EquipmentCraft { get; set; }
    public bool AutoSkillEnabled { get; set; }
    // 플레이어가 마지막으로 세이브한 시각을 국제 시간 기준으로 저장하는 데이터
    public string LastSavedAtUtc { get; set; }
}

[Serializable]
public class PlayerProfileSaveData
{
    public string PlayerId { get; set; }
    public string Nickname { get; set; }
}

[Serializable]
public class AirshipSaveData
{
    public int AttackLevel { get; set; }
    public int RecoveryLevel { get; set; }
    public int MaxHealthLevel { get; set; }
    public int CriticalLevel { get; set; }
    public AirshipCannonType EquippedCannonType { get; set; }
    public AirshipGearType EquippedGearType { get; set; }
    public HashSet<AirshipCannonType> OwnedCannons { get; set; }
    public HashSet<AirshipGearType> OwnedGears { get; set; }
}

/*
 * 고민사항
 * 지금 당장은 프로젝트 규모를 고려해서 모든 영웅 유닛의 데이터를 전부 넣고 IsOwned로 소유 여부를 판단하는 방식으로 생각하고 있음.
 * 추후에 영웅 유닛이 많아질 경우, 과연 이 방법이 괜찮을지, 혹은 IsOwned를 빼고 소유한 영웅 유닛만 저장하는 방식으로 바꾸는 것이 좋을지 고민이 필요함.
 * 
 */
[Serializable]
public class HeroSaveData
{
    public int Level { get; set; }
    public bool IsOwned { get; set; }
    public int EquippedWeaponId { get; set; }
    public int EquippedBodyId { get; set; }
    public int EquippedAccId { get; set; }
}

[Serializable]
public class HeroFormationSaveData
{
    public List<HeroSaveSlot> Slots { get; set; }
}

[Serializable]
public class HeroSaveSlot
{
    public int SlotIndex { get; set; }
    [JsonConverter(typeof(StringEnumConverter))]
    public HeroNameEnum HeroId { get; set; }
}

[Serializable]
public class StageProgressSaveData
{
    public int CurrentStage { get; set; }
    // 플레이어가 클리어한 최대 스테이지를 저장하는 데이터로 오프라인 보상 계산의 기준이 됨.
    public int MaxClearedStage { get; set; }
    // 스테이지 반복 설정 저장
    public bool RepeatClearedStage { get; set; }
}

[Serializable]
public class WalletSaveData
{
    public Dictionary<CurrencyType, CurrencySaveData> Currencies { get; set; }
}

[Serializable]
public class CurrencySaveData
{
    public long Amount { get; set; }
}

[Serializable]
public class InventorySaveData
{
    public Dictionary<int, ItemStackSaveData> Items { get; set; }
}

// 재료 전용 인벤
[Serializable]
public class ItemStackSaveData
{
    public int Amount { get; set; }
}

// 장비 전용 인벤
[Serializable]
public class EquipmentInventorySaveData
{
    public int NextEquipId { get; set; }
    public List<EquipmentSaveData> Equipments { get; set; }
}

[Serializable]
public class EquipmentSaveData
{
    public int EquipDataId { get; set; }
    public int EquipId { get; set; }
    public int EquipLv { get; set; }
    public EquipGradeEnum EquipGrade { get; set; }
    public EquipPartEnum EquipPart { get; set; }
    public float BonusHP { get; set; }
    public float BonusAtk { get; set; }
    public float BonusDef { get; set; }
    public float BonusCriChance { get; set; }
}

// 제작 중인 슬롯의 정보를 담는 데이터
[Serializable]
public class EquipmentCraftSaveData
{
    public List<EquipmentCraftSlotSaveData> Slots { get; set; }
}

// 제작 진행 현황을 저장하는 데이터
[Serializable]
public class EquipmentCraftSlotSaveData
{
    public int SlotIndex { get; set; }
    public bool IsCrafting { get; set; }
    public int RecipeId { get; set; }
    public string StartedAtUtc { get; set; }
    public string CompletesAtUtc { get; set; }
}
