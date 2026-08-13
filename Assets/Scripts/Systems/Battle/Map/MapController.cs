using System.Runtime.CompilerServices;
using UnityEngine;

public class MapController : MonoBehaviour
{
    [SerializeField] private MapCatalog mapCatalog;
    [SerializeField] private Transform mapRoot;
    [SerializeField] private Camera mainCamera;

    private GameObject currentMap;

    public void LoadMap(int stageNumber)
    {
        GameObject mapPrefab = mapCatalog.GetMapPrefab(stageNumber);

        if (currentMap != null)
        {
            Destroy(currentMap);
        }

        currentMap = Instantiate(mapPrefab, mapRoot, false);
        
        AlignMapBottomToCamera(currentMap);
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
