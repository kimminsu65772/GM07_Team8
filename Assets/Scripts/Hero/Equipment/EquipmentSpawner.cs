using UnityEngine;

public class EquipmentSpawner : MonoBehaviour
{
    /*
    private static EquipmentSpawner instance;
    public static EquipmentSpawner Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<EquipmentSpawner>();
                if (instance == null)
                {
                    GameObject obj = new GameObject("EquipmentSpawner");
                    instance = obj.AddComponent<EquipmentSpawner>();
                }
            }
            return instance;
        }
    }
    */

    int randGradeNum;
    int randPartNum;

    [SerializeField] private EquipmentSO[] equipSO;
    private int equipSOSelectedNum;

    

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C)) CreateEquip();
    }

    public void CreateEquip()
    {
        Equipment spawnedEquip = new Equipment();
        RandomInfo();
        string id = SetEquipID();

        spawnedEquip.EquipInit(equipSO[equipSOSelectedNum], id);

        EquipmentManager.EquipDic.Add(id, spawnedEquip);

        foreach (var equip in EquipmentManager.EquipDic)
        {
            Debug.Log($"ID: {equip.Key}, Equipment: {equip.Value}\n");
        }
    }

    public Equipment CreateEquipByGrade(EquipGradeEnum equipGrade)
    {
        Equipment spawnedEquip = new();
        RandomInfoByGrade(equipGrade);
        string id = SetEquipID();

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

    private string SetEquipID()
    {
        int n = 1;
        string equipID;

        while (true)
        {
            equipID = $"{equipSOSelectedNum}_{n}";

            if (!EquipmentManager.EquipDic.ContainsKey(equipID))
            {
                return equipID;
            }

            n++;
        }
    }
}
