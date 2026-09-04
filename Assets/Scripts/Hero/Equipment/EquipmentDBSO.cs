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
            equipmentDictionary = new Dictionary<int, EquipmentSO>();
            return;
        }

        equipmentDictionary = new Dictionary<int, EquipmentSO>();
        foreach (var equip in equipmentDB)
        {
            if (equipmentDictionary.ContainsKey(equip.EquipDataId))
            {
                continue;
            }

            equipmentDictionary.Add(equip.EquipDataId, equip);
        }
    }
}
