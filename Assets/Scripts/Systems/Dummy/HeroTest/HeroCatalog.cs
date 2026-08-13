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

    public IReadOnlyList<HeroEntry> InGameHeroEntries
    {
        get
        {
            List<HeroEntry> inGameEntries;
            BuildHeroEntryDict();
            inGameEntries = new List<HeroEntry>(heroEntryDictionary.Values);
            return inGameEntries;
        }
    }

    private void OnEnable()
    {
        BuildHeroEntryDict();
    }
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

    private void BuildHeroEntryDict()
    {
        if (heroEntryDictionary != null && heroEntryDictionary.Count > 0)
        {
            return;
        }

        heroEntryDictionary = new Dictionary<string, HeroEntry>();

        if (heroEntries == null)
        {
            return;
        }

        foreach (HeroEntry entry in heroEntries)
        {
            if (entry == null)
            {
                continue;
            }

            if (entry.HeroPrefab == null ||
                string.IsNullOrWhiteSpace(entry.HeroName))
            {
                continue;
            }

            if (!heroEntryDictionary.TryAdd(entry.HeroName, entry))
            {
                Debug.LogWarning($"중복된 영웅입니다: {entry.HeroName}");
            }
        }
    }
}
