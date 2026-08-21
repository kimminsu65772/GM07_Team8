using UnityEditor;
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

    private float hpMax, hpMin;
    private float atkMax, atkMin;
    private float defMax, defMin;
    private float criChanceMax, criChanceMin;

    private string savePath = "Assets/Data/Hero/EquipmentData";

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C)) CreateEquip();
    }

    public void CreateEquip()
    {
        Equipment spawnedEquip = ScriptableObject.CreateInstance<Equipment>();

        RandomInfo();

        spawnedEquip.SetData(
            randGrade, randPart,
            randHP, randAtk, randDef, randCriChance);


        string fileName = EquipName();
        string assetPath = $"{savePath}/{fileName}";
        AssetDatabase.CreateAsset(spawnedEquip, assetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"{assetPath} 생성 완료");
    }

    private void RandomInfo()
    {
        randGradeNum = Random.Range(0, 4);
        randPartNum = Random.Range(0, 3);

        switch (randGradeNum)
        {
            case 0:
                randGrade = EquipGradeEnum.Common;
                break;
            case 1:
                randGrade = EquipGradeEnum.Rare;
                break;
            case 2:
                randGrade = EquipGradeEnum.Epic;
                break;
            case 3:
                randGrade = EquipGradeEnum.Legendary;
                break;
        }

        switch (randPartNum)
        {
            case 0:
                randPart = EquipPartEnum.Weapon;
                break;
            case 1:
                randPart = EquipPartEnum.Body;
                break;
            case 2:
                randPart = EquipPartEnum.Acc;
                break;
        }
    }

    public string EquipName()
    {
        int saveNum = 1;

        while (true)
        {
            string fileName = $"Equip{randGradeNum}{randPartNum}_{saveNum}.asset";

            if (AssetDatabase.LoadAssetAtPath<Equipment>($"{savePath}/{fileName}") == null)
            {
                return fileName;
            }

            saveNum++;
        }
    }
}
