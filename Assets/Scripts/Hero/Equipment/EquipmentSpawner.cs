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
    

    private EquipGradeEnum randGrade;
    private EquipPartEnum randPart;
    private float randHP, randAtk, randDef, randCriChance;

    int randGradeNum;
    int randPartNum;

    [SerializeField] private EquipmentSO[] equipSO;
    private int equipSOSelectedNum;

    private float hp;
    private float atk;
    private float def;
    private float criChance;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C)) CreateEquip();
    }

    public void CreateEquip()
    {
        Equipment spawnedEquip = new Equipment();
        RandomInfo();
        int id = SetEquipID();

        spawnedEquip.EquipInit(equipSO[equipSOSelectedNum], id);

        EquipmentManager.EquipDic.Add(id, spawnedEquip);

        string dicLog = "";
        foreach (var equip in EquipmentManager.EquipDic)
        {
            dicLog += $"ID: {equip.Key}, Equipment: {equip.Value}\n";
        }
        Debug.Log($"생성 완료\n{dicLog}");
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

    public int SetEquipID()
    {
        int n = 1;
        int equipID;
        string equipIDs;

        while (true)
        {
            equipIDs = $"{equipSOSelectedNum}{n}";

            equipID = int.TryParse(equipIDs, out equipID) ? equipID : 0;

            if (!EquipmentManager.EquipDic.ContainsKey(equipID))
            {
                return equipID;
            }

            n++;
        }
    }
}
