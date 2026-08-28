using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentCraftSlotUI : MonoBehaviour
{
    [Header("제작 슬롯 참조 설정")]
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text explanationText;
    [SerializeField] private TMP_Text durationText;
    [SerializeField] private MaterialCostSlotUI[] materialCostSlots;
    [SerializeField] private Button craftButton;

    private EquipmentCraftRecipeSO currentRecipe;
    private ItemDBSO itemDB;
    private Action<EquipmentCraftRecipeSO> onRecipeSelected;

    private void Awake()
    {
        if (craftButton != null)
        {
            craftButton.onClick.RemoveListener(OnCraftButtonClicked);
            craftButton.onClick.AddListener(OnCraftButtonClicked);
        }
    }

    private void OnEnable()
    {
        PlayerInfo.Instance.OnItemAmountChanged -= Refresh;
        PlayerInfo.Instance.OnItemAmountChanged += Refresh;
    }

    private void OnDisable()
    {
        PlayerInfo.Instance.OnItemAmountChanged -= Refresh;
    }

    private void OnDestroy()
    {
        if (craftButton != null)
        {
            craftButton.onClick.RemoveListener(OnCraftButtonClicked);
        }
    }

    public void Bind(
        EquipmentCraftRecipeSO recipe,
        ItemDBSO itemDB,
        Action<EquipmentCraftRecipeSO> onRecipeSelected)
    {
        currentRecipe = recipe;
        this.itemDB = itemDB;
        this.onRecipeSelected = onRecipeSelected;

        Refresh();
    }

    public void Refresh()
    {
        if (currentRecipe == null)
        {
            Clear();
            return;
        }

        if (nameText != null)
        {
            nameText.text = currentRecipe.RecipeName;
        }

        if (explanationText != null)
        {
            explanationText.text = currentRecipe.RecipeDescription;
        }

        if (durationText != null)
        {
            durationText.text = FormatDuration(currentRecipe.CraftDuration);
        }

        RefreshMaterialCosts();

        if (craftButton != null)
        {
            craftButton.interactable = CanCraft(currentRecipe);
        }
    }

    private void Clear()
    {
        if (nameText != null)
        {
            nameText.text = string.Empty;
        }

        if (explanationText != null)
        {
            explanationText.text = string.Empty;
        }

        if (durationText != null)
        {
            durationText.text = string.Empty;
        }

        ClearMaterialCosts();

        if (craftButton != null)
        {
            craftButton.interactable = false;
        }
    }

    private void RefreshMaterialCosts()
    {
        if (materialCostSlots == null || materialCostSlots.Length == 0)
        {
            return;
        }

        int materialCount = currentRecipe.RequiredMaterials != null
            ? currentRecipe.RequiredMaterials.Count
            : 0;
        // 활성화된 슬롯을 계산하여 필요한 슬롯만 표시하고 나머지는 숨길 수 있도록 한다.
        // 이 때 활성화 순서는 오른쪽에서 왼쪽으로 진행되도록 한다.
        int activeCount = Mathf.Min(materialCount, materialCostSlots.Length);
        int startSlotIndex = materialCostSlots.Length - activeCount;

        for (int i = 0; i < materialCostSlots.Length; i++)
        {
            MaterialCostSlotUI slot = materialCostSlots[i];
            if (slot == null)
            {
                continue;
            }

            // i는 현재 순회 중인 슬롯의 인덱스
            int materialIndex = i - startSlotIndex;

            // materialIndex가 유효한 범위인지 확인하고, 유효하지 않으면 슬롯을 숨기고 초기화한다.
            if (materialIndex < 0 || materialIndex >= activeCount)
            {
                slot.Clear();
                slot.gameObject.SetActive(false);
                continue;
            }

            // materialIndex가 유효한 경우, 해당 재료 정보를 가져와 슬롯에 바인딩한다.
            // 이 때 재료의 코스트가 null인 경우도 비정상인 상황이므로 슬롯을 숨기고 초기화한다.
            ItemAmount cost = currentRecipe.RequiredMaterials[materialIndex];
            if (cost == null)
            {
                slot.Clear();
                slot.gameObject.SetActive(false);
                continue;
            }

            ItemSO item = itemDB != null ? itemDB.GetItemById(cost.itemId) : null;
            slot.Bind(cost, item);
        }
    }

    private void ClearMaterialCosts()
    {
        if (materialCostSlots == null)
        {
            return;
        }

        for (int i = 0; i < materialCostSlots.Length; i++)
        {
            MaterialCostSlotUI slot = materialCostSlots[i];
            if (slot == null)
            {
                continue;
            }

            slot.Clear();
            slot.gameObject.SetActive(false);
        }
    }

    private bool CanCraft(EquipmentCraftRecipeSO recipe)
    {
        if (recipe == null)
        {
            return false;
        }

        PlayerInfo playerInfo = PlayerInfo.Instance;
        if (playerInfo == null)
        {
            return false;
        }

        if (recipe.RequiredMaterials == null)
        {
            return true;
        }

        foreach (ItemAmount cost in recipe.RequiredMaterials)
        {
            if (cost == null)
            {
                continue;
            }

            if (!playerInfo.HasEnoughItem(cost.itemId, cost.amount))
            {
                return false;
            }
        }

        return true;
    }

    private void OnCraftButtonClicked()
    {
        if (currentRecipe == null || !CanCraft(currentRecipe))
        {
            return;
        }

        onRecipeSelected?.Invoke(currentRecipe);
    }

    private static string FormatDuration(int seconds)
    {
        if (seconds <= 0)
        {
            return "즉시";
        }

        TimeSpan duration = TimeSpan.FromSeconds(seconds);
        if (duration.TotalHours >= 1)
        {
            return $"{(int)duration.TotalHours}시간 {duration.Minutes}분 {duration.Seconds}초";
        }

        if (duration.TotalMinutes >= 1)
        {
            return $"{duration.Minutes}분 {duration.Seconds}초";
        }

        return $"{duration.Seconds}초";
    }
}
