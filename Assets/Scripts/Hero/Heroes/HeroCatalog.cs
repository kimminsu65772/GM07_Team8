using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 영웅의 ID와 프리팹이 연결된 HeroEntry를 Dictionary로 관리하고
/// 외부에서는 영웅의 ID를 통해 HeroEntry를 가져올 수 있도록 한다.
/// </summary>

[CreateAssetMenu(fileName = "HeroCatalog", menuName = "Game/Hero/HeroCatalog")]
public class HeroCatalog : ScriptableObject
{
    [SerializeField] private List<HeroEntry> heroEntries;

    private Dictionary<HeroNameEnum, HeroEntry> heroEntryDictionary;

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
    public bool TryGetHeroEntry(HeroNameEnum heroId, out HeroEntry heroEntry)
    {
        BuildHeroEntryDict();

        if (heroEntryDictionary != null && heroEntryDictionary.TryGetValue(heroId, out heroEntry))
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

        heroEntryDictionary = new Dictionary<HeroNameEnum, HeroEntry>();

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
                entry.HeroId == HeroNameEnum.None)
            {
                continue;
            }

            if (!heroEntryDictionary.TryAdd(entry.HeroId, entry))
            {
                Debug.LogWarning($"중복된 영웅 Id입니다.: {entry.HeroId.ToString()}");
            }
        }
    }
}
