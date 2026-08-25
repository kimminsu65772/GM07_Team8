using System.Collections.Generic;
using UnityEngine;

public class EquipmentManager
{
    public static Dictionary<string, Equipment> EquipDic = new Dictionary<string, Equipment>();

    public static void RebuildFromSave(EquipmentInventorySaveData equipmentInventory, EquipmentSO[] equipmentSources)
    { 
        EquipDic.Clear();

        if (equipmentInventory == null || equipmentSources == null)
        {
            Debug.LogWarning("EquipmentManager: 딕셔너리 리빌딩을 위한 인벤토리 데이터나 장비 원본 데이터가 비어 있습니다.");
            return;
        }

        if (equipmentInventory.OwnedEquipmentIds == null)
        {
            Debug.LogWarning("EquipmentManager: 장비 인벤토리 데이터의 OwnedEquipmentIds가 null입니다.");
            return;
        }

        foreach (string equipmentId in equipmentInventory.OwnedEquipmentIds)
        {
            if (string.IsNullOrEmpty(equipmentId))
            {
                Debug.LogWarning("EquipmentManager: 장비 ID가 null이거나 비어 있습니다.");
                continue;
            }

            string[] splitId = equipmentId.Split('_');

            if (splitId.Length != 2)
            {
                Debug.LogWarning($"EquipmentManager: 장비 ID '{equipmentId}' 형식이 잘못되었습니다..");
                continue;
            }

            if (EquipDic.TryGetValue(equipmentId, out Equipment existingEquipment))
            {
                Debug.LogWarning($"EquipmentManager: 장비 ID '{equipmentId}'가 이미 존재합니다.");
                continue;
            }

            if (!int.TryParse(splitId[0], out int equipmentSourceIndex))
            {
                Debug.LogWarning($"EquipmentManager: 장비 ID '{equipmentId}'에서 장비 원본 인덱스를 파싱할 수 없습니다.");
                continue;
            }

            if (equipmentSourceIndex < 0 || equipmentSourceIndex >= equipmentSources.Length)
            {
                Debug.LogWarning($"장비 원본 인덱스가 범위를 벗어났습니다. ID: {equipmentId}, Index: {equipmentSourceIndex}");
                continue;
            }

            EquipmentSO equipmentSO = equipmentSources[equipmentSourceIndex];

            if (equipmentSO == null)
            {
                Debug.LogWarning($"장비 원본 데이터가 null입니다. ID: {equipmentId}, Index: {equipmentSourceIndex}");
                continue;
            }

            Equipment equipment = new Equipment();
            equipment.EquipInit(equipmentSO, equipmentId);

            EquipDic[equipmentId] = equipment;
        }   
    }
}
