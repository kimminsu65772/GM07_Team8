using UnityEngine;
using UnityEngine.Profiling;
using System.Collections.Generic;

public class MapController : MonoBehaviour
{
    [SerializeField] private MapCatalog mapCatalog;
    [SerializeField] private Transform mapRoot;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private FallLeafVfxController fallLeafVfxController;

    private GameObject currentMap;

    // key: 맵 프리팹, value: 인스턴스화된 맵 오브젝트
    private readonly Dictionary<GameObject, GameObject> mapCache = new Dictionary<GameObject, GameObject>();
    public void LoadMap(int stageNumber)
    {
        Profiler.BeginSample("MapController.LoadMap");

        try
        {
            MapEntry mapEntry = mapCatalog.GetMapEntry(stageNumber);

            if (mapEntry == null)
            {
                return;
            }

            GameObject mapPrefab = mapEntry.MapPrefab;

            if (mapPrefab == null)
            {
                return;
            }

            SoundId bgmId = mapEntry.bgmId;

            if (!mapCache.TryGetValue(mapPrefab, out GameObject cachedMap))
            {
                Profiler.BeginSample("MapController.CacheMiss.InstantiateMap");

                try
                {
                    cachedMap = Instantiate(mapPrefab, mapRoot, false);
                    AlignMapBottomToCamera(cachedMap);
                }
                finally
                {
                    Profiler.EndSample();
                }

                mapCache.Add(mapPrefab, cachedMap);
            }
            else
            {
                Profiler.BeginSample("MapController.CacheHit.ReuseMap");
                Profiler.EndSample();
            }

            if (cachedMap == currentMap)
            {
                return;
            }

            if (currentMap != null)
            {
                currentMap.SetActive(false);
            }

            currentMap = cachedMap;
            cachedMap.SetActive(true);

            if (bgmId != SoundId.None && SoundManager.Instance != null)
            {
                SoundManager.Instance.PlayBGM(bgmId);
            }

            Color leafColor = mapEntry.BackgroundVFXColor;

            if (leafColor != null)
            {
                fallLeafVfxController.SetLeafColor(leafColor);
            }
        }
        finally
        {
            Profiler.EndSample();
        }
    }

    // 맵을 카메라 촬영 범위에 맞게 조정하는 메서드
    private void AlignMapBottomToCamera(GameObject map)
    {
        if (map == null || mainCamera == null)
            return;

        // 맵을 구성하는 모든 레이어의 SpriteRenderer를 가져온다.
        SpriteRenderer[] renderers = map.GetComponentsInChildren<SpriteRenderer>();

        if (renderers.Length == 0)
            return;

        Bounds mapBounds = renderers[0].bounds;

        for (int i = 1; i < renderers.Length; i++)
        {
            mapBounds.Encapsulate(renderers[i].bounds);
        }

        float cameraBottom = mainCamera.transform.position.y - mainCamera.orthographicSize;

        float AlignmentOffsetY = cameraBottom - mapBounds.min.y;

        map.transform.position += Vector3.up * AlignmentOffsetY;
    }
}
