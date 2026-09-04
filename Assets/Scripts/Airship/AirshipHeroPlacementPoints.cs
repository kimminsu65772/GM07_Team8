using UnityEngine;

public class AirshipHeroPlacementPoints : MonoBehaviour
{
    [SerializeField] private Transform[] frontPlacementPoints =
        new Transform[5];

    [SerializeField] private Transform[] backPlacementPoints =
        new Transform[5];

    /// <summary>
    /// 트랜스폼을 받아와서 후열은 그 트랜스폼의 자식으로 두고,<br/>
    /// 전열은 그냥 그 위치를 캐싱해두고 스폰을 거기서 할듯.
    /// </summary>
    /// <param name="num">가져올 위치 수 최대(5)</param>
    /// <param name="isFront">전열/후열 구분 부울값</param>
    /// <returns></returns>
    public Transform[] GetPlacementTransforms(int num, bool isFront)
    {
        Transform[] placementPoints =
            isFront ? frontPlacementPoints : backPlacementPoints;

        if (placementPoints == null ||
            num <= 0 ||
            num > placementPoints.Length)
        {
            return new Transform[0];
        }

        Transform[] result = new Transform[num];

        for (int i = 0; i < num; i++)
        {
            result[i] = placementPoints[i];
        }

        return result;
    }
}