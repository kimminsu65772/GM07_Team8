using UnityEngine;

public class EquipmentSpawner : MonoBehaviour
{
    [SerializeField] private EquipmentSO[] equipSO;
    private int equipSOSelectedNum;

    private int randGradeNum;
    private int randPartNum;

    public long NextEquipmentID { get; private set; } = 1;

    public Equipment CreateEquipByGrade(EquipGradeEnum equipGrade)
    {
        Equipment spawnedEquip = new();
        RandomInfoByGrade(equipGrade);
        long id = GenerateEquipID();

        spawnedEquip.EquipInit(equipSO[equipSOSelectedNum], id);

        EquipmentManager.EquipDic.Add(id, spawnedEquip);

        foreach (var equip in EquipmentManager.EquipDic)
        {
            Debug.Log($"ID: {equip.Key}, Equipment: {equip.Value}\n");
        }

        return spawnedEquip;
    }

    private void RandomInfoByGrade(EquipGradeEnum equipGrade)
    {
        randGradeNum = (int)equipGrade;
        switch (randGradeNum)
        {
            case 0:
                equipSOSelectedNum = 0;
                break;
            case 1:
                equipSOSelectedNum = 3;
                break;
            case 2:
                equipSOSelectedNum = 6;
                break;
            case 3:
                equipSOSelectedNum = 9;
                break;
            default:
                Debug.Log("randGradeNum 잘못된 값 생성");
                break;
        }
        randPartNum = Random.Range(0, 3);
        switch (randPartNum)
        {
            case 0:
                break;
            case 1:
                equipSOSelectedNum += 1;
                break;
            case 2:
                equipSOSelectedNum += 2;
                break;
            default:
                Debug.Log("randPartNum 잘못된 값 생성");
                break;
        }
    }

    private long GenerateEquipID()
    {
        while (EquipmentManager.EquipDic.ContainsKey(NextEquipmentID))
        {
            NextEquipmentID++;
        }

        return NextEquipmentID++;
    }

    /*
    public void CreateEquip()
    {
        Equipment spawnedEquip = new Equipment();
        RandomInfo();
        long id = GenerateEquipID();

        spawnedEquip.EquipInit(equipSO[equipSOSelectedNum], id);

        EquipmentManager.EquipDic.Add(id, spawnedEquip);

        foreach (var equip in EquipmentManager.EquipDic)
        {
            Debug.Log($"ID: {equip.Key}, Equipment: {equip.Value}\n");
        }
    }

    private void RandomInfo()
    {
        randGradeNum = Random.Range(0, 4);
        randPartNum = Random.Range(0, 3);

        switch (randGradeNum)
        {
            case 0:
                equipSOSelectedNum = 0;
                break;
            case 1:
                equipSOSelectedNum = 3;
                break;
            case 2:
                equipSOSelectedNum = 6;
                break;
            case 3:
                equipSOSelectedNum = 9;
                break;
            default:
                Debug.Log("randGradeNum 잘못된 값 생성");
                break;
        }

        switch (randPartNum)
        {
            case 0:
                break;
            case 1:
                equipSOSelectedNum += 1;
                break;
            case 2:
                equipSOSelectedNum += 2;
                break;
            default:
                Debug.Log("randPartNum 잘못된 값 생성");
                break;
        }
    }
    */
}
