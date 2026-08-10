using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 영웅의 이름과 프리팹이 연결된 HeroEntry를 Dictionary로 관리하고
/// 외부에서 영웅 이름으로 entry와 매핑하여 프리팹을 가져오는 역할을 한다.
/// </summary>

[CreateAssetMenu(fileName = "HeroCatalog", menuName = "Game/Hero/HeroCatalog")]
public class HeroCatalog : ScriptableObject
{
    [SerializeField] private List<HeroEntry> heroEntries;

    private Dictionary<string, HeroEntry> heroEntryDictionary;

    public IReadOnlyList<HeroEntry> HeroEntries => heroEntries;

    public bool TryGetHeroEntry(string heroName, out HeroEntry heroEntry)
    {
        BuildHeroEntryDict();

        if (heroEntryDictionary != null && heroEntryDictionary.TryGetValue(heroName, out heroEntry))
        {
            return true;
        }

        heroEntry = null;

        return false;
    }

    public List<HeroEntry> GetDefaultOwnedHeroEntries()
    {
        List<HeroEntry> defaultOwnedHeroEntries = new List<HeroEntry>();
        BuildHeroEntryDict();

        if (heroEntryDictionary == null)
        {
            return defaultOwnedHeroEntries;
        }

        foreach (HeroEntry entry in heroEntryDictionary.Values)
        {
            if (entry.IsDefaultOwned)
            {
                defaultOwnedHeroEntries.Add(entry);
            }
        }

        return defaultOwnedHeroEntries;
    }

    private void BuildHeroEntryDict()
    {
        if (heroEntries == null)
        {
            return;
        }

        if (heroEntryDictionary == null)
        {
            heroEntryDictionary = new Dictionary<string, HeroEntry>();
            foreach (HeroEntry entry in heroEntries)
            {
                if (entry == null)
                {
                    Debug.LogWarning("Hero entry is empty.");
                    continue;
                }

                if (heroEntryDictionary.ContainsKey(entry.HeroName))
                {
                    Debug.LogWarning($"중복된 영웅 등록입니다. {entry.HeroName}");
                    continue;
                }

                if (entry.HeroPrefab == null || string.IsNullOrWhiteSpace(entry.HeroName))
                {
                    Debug.LogWarning($"영웅 프리팹이 없거나 이름이 비어있습니다.");
                    continue;
                }

                heroEntryDictionary.Add(entry.HeroName, entry);
            }
        }
    }
}
