using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapCatalog", menuName = "Game/Map/MapCatalog")]
public class MapCatalog : ScriptableObject
{
    [SerializeField] private int mapCycle = 5;
    [SerializeField] private MapEntry[] entries;

    private readonly Dictionary<int, MapEntry> mapPrefabs = new();

    private void OnEnable()
    {
        FillMapPrefabs();
    }

    public MapEntry GetMapEntry(int currentStage)
    {
        int mapKey = GetMapKey(currentStage);
        mapPrefabs.TryGetValue(mapKey, out MapEntry entry);
        return entry;
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
        int regionId = ((currentStage - 1) / mapCycle) + 1;
        if (!mapPrefabs.TryGetValue(regionId, out MapEntry mapEntry))
        {
            return -1;
        }
        return regionId;
    }

    private void FillMapPrefabs()
    {
        mapPrefabs.Clear();

        for (int i = 0; i < entries.Length; i++)
        {
            MapEntry entry = entries[i];

            if (entry.RegionId <= 0)
            {
                continue;
            }

            if (entry.MapPrefab == null)
            {
                continue;
            }

            if (mapPrefabs.ContainsKey(entry.RegionId))
            {
                continue;
            }

            mapPrefabs.Add(entry.RegionId, entry);
        }
    }
}
