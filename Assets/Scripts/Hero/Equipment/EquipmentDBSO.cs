using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EquipmentDB", menuName = "Game Data/Equip/EquipmentDB")]
public class EquipmentDB : ScriptableObject
{
    [SerializeField] private EquipmentSO[] equipmentDB;

    private Dictionary<int, EquipmentSO> equipmentDictionary;
    public IReadOnlyList<EquipmentSO> EquipmentDBList => equipmentDB;

    private void OnEnable()
    {
        BuildEquipDict();
    }

    public EquipmentSO GetEquipmentSO(int equipDataId)
    {
        BuildEquipDict();
        if (equipmentDictionary.TryGetValue(equipDataId, out var equipSO))
        {
            return equipSO;
        }
        else
        {
            Debug.LogWarning($"장비 ID {equipDataId}에 해당하는 장비를 찾을 수 없습니다.");
            return null;
        }
    }

    private void BuildEquipDict()
    {
        if (equipmentDictionary != null)
        {
            return;
        }

        if (equipmentDB == null || equipmentDB.Length == 0)
        {
            Debug.LogWarning("장비 DB가 비어있습니다.");
            equipmentDictionary = new Dictionary<int, EquipmentSO>();
            return;
        }

        equipmentDictionary = new Dictionary<int, EquipmentSO>();
        foreach (var equip in equipmentDB)
        {
            if (equipmentDictionary.ContainsKey(equip.EquipDataId))
            {
                Debug.LogWarning($"{equip.EquipDataId}는 이미 등록된 장비입니다.");
                continue;
            }

            equipmentDictionary.Add(equip.EquipDataId, equip);
        }
    }
}
