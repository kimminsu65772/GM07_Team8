using System.Collections.Generic;
using UnityEngine;

public class MapCatalog : MonoBehaviour
{
    [SerializeField] private int mapCycle = 5;
    [SerializeField] private MapEntry[] entries;

    private readonly Dictionary<int, MapEntry> mapPrefabs = new();

    private void Awake()
    {
        FillMapPrefabs();
    }

    public GameObject GetMapPrefab(int currentStage)
    {
        int mapKey = GetMapKey(currentStage);
        mapPrefabs.TryGetValue(mapKey, out MapEntry entry);
        return entry.MapPrefab;
    }

    private int GetMapKey(int currentStage)
    {
        if (mapCycle <= 0)
        {
            Debug.LogError("mapCycle은 0보다 커야 합니다.");
            return -1;
        }
        int key = ((currentStage - 1) / mapCycle) + 1;
        Debug.Log($"currentStage: {currentStage}, mapCycle: {mapCycle}, index: {key}");
        if (key < 0 || key > entries.Length)
        {
            Debug.LogError($"currentStage {currentStage}에 대한 MapEntry가 존재하지 않습니다.");
            return -1;
        }
        return key;
    }

    private void FillMapPrefabs()
    {
        mapPrefabs.Clear();

        for (int i = 0; i < entries.Length; i++)
        {
            MapEntry entry = entries[i];

            if (entry.StageNumber <= 0)
            {
                Debug.LogError("MapEntry의 StageNumber는 0보다 커야 합니다.");
                continue;
            }

            if (entry.MapPrefab == null)
            {
                Debug.LogError($"StageNumber {entry.StageNumber}에 대한 MapPrefab이 할당되지 않았습니다.");
                continue;
            }

            if (mapPrefabs.ContainsKey(entry.StageNumber))
            {
                Debug.LogError($"StageNumber {entry.StageNumber}에 대한 MapEntry가 이미 존재합니다.");
                continue;
            }

            mapPrefabs.Add(entry.StageNumber, entry);
        }
    }
}
