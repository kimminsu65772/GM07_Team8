using UnityEngine;

public class EquipmentSpawner : MonoBehaviour
{
    [SerializeField] private EquipmentDB equipmentDB;
    private int equipSOSelectedNum;

    private int randGradeNum;
    private int randPartNum;

    public int NextEquipmentID { get; private set; } = 1;

    public Equipment CreateEquipByGrade(EquipGradeEnum equipGrade)
    {
        Equipment spawnedEquip = new();
        RandomInfoByGrade(equipGrade);
        int id = PlayerInfo.Instance.GetNextEquipId();

        spawnedEquip.EquipInit(equipmentDB.EquipmentDBList[equipSOSelectedNum], id);

        //EquipmentManager.EquipDic.Add(id, spawnedEquip);

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
                break;
        }
    }

    private int GenerateEquipID()
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
                break;
        }
    }
    */
}
