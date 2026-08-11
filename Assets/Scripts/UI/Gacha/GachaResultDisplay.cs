using System.Collections.Generic;
using UnityEngine;

public class GachaResultDisplay : MonoBehaviour
{
    [Header("UI 연결")]
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private Transform contentGrid;
    [SerializeField] private GameObject itemSlotPrefab;
    void Start()
    {
        if (resultPanel != null)
        {
            resultPanel.SetActive(false);
        }
    }
    //뽑기 결과를 받아와서 화면에 쭈루룩 생성하는 함수
    public void ShowResults(List<string> pulledItems)
    {
        foreach (Transform child in contentGrid)
        {
            Destroy(child.gameObject);
        }
        if (resultPanel != null)
        {
            resultPanel.SetActive(true);
        }
        foreach (string itemName in pulledItems)
        {
            Debug.Log("아이콘 생성 시도: " + itemName);
            GameObject slot = Instantiate(itemSlotPrefab, contentGrid);
            GachaResultUI slotUI = slot.GetComponent<GachaResultUI>();
            if (slotUI != null)
            {
                slotUI.SetUp(itemName);
            }
        }
    }
    // 결과창 닫기 버튼에 연결할 함수
    public void CloseResultPanel()
    {
        if (resultPanel != null)
        {
            resultPanel.SetActive(false);
        }
    }

}
