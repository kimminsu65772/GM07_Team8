using UnityEngine;
using UnityEngine.Rendering;

public class SortingYPos : MonoBehaviour
{
    [SerializeField] private int baseOrder = 5000;
    [SerializeField] private int sortScale = 100;
    [SerializeField] private Transform sortPivot;

    private SortingGroup sortingGroup;

    private void Awake()
    {
        sortingGroup = GetComponent<SortingGroup>();

        if (sortPivot == null)
        {
            sortPivot = transform;
        }
    }

    private void LateUpdate()
    {
        sortingGroup.sortingOrder =
            baseOrder - Mathf.RoundToInt(sortPivot.position.y * sortScale);
    }
}
