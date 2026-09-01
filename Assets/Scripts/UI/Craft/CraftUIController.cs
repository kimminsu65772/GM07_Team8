using System;
using UnityEngine;

public class CraftUIController : MonoBehaviour
{
    [SerializeField] private EquipmentCraftSlotUI[] recipeSlots;
    [SerializeField] private EquipmentCraftRecipeDB recipeDB;
    [SerializeField] private ItemDBSO itemDB;
    [SerializeField] private EquipmentDB equipmentDB;
    [SerializeField] private CraftUI craftUI;
    [SerializeField] private CraftCompleteUIController craftCompleteUI;
    [SerializeField] private EquipmentSpawner equipmentSpawner;

    private void OnEnable()
    {
        RefreshRecipeSlots();
        if (craftUI != null)
        {
            craftUI.Bind(recipeDB, OnCraftCompleted);
            craftUI.gameObject.SetActive(true);
        }
    }

    private void RefreshRecipeSlots()
    {
        if (recipeSlots == null || recipeDB == null || itemDB == null)
            return;

        var recipes = recipeDB.Recipes;

        for (int i = 0; i < recipeSlots.Length; i++)
        {
            if (recipeSlots[i] == null)
            {
                Debug.LogWarning($"{i}번째 장비 제작 레시피 슬롯이 할당되지 않았습니다.");
                continue;
            }

            if (recipes == null || i >= recipes.Count || recipes[i] == null)
            {
                recipeSlots[i].gameObject.SetActive(false);
                continue;
            }

            recipeSlots[i].gameObject.SetActive(true);
            recipeSlots[i].Bind(recipes[i], itemDB, OnRecipeSelected);
        }
    }

    private void OnRecipeSelected(EquipmentCraftRecipeSO recipe)
    {
        EquipmentCraftSlotSaveData craftSlot = PlayerInfo.Instance.GetEquipmentCraftSlot(0);
        if (craftSlot != null && craftSlot.IsCrafting)
        {
            Debug.LogWarning("이미 장비를 제작중입니다.");
            return;
        }
        if (recipe == null)
        {
            Debug.LogWarning("선택된 레시피가 없습니다.");
            return;
        }

        if (recipe.RequiredMaterials == null)
        {
            Debug.LogWarning("레시피 재료 정보가 없습니다.");
            return;
        }

            foreach (var material in recipe.RequiredMaterials)
        {
            if (!PlayerInfo.Instance.HasEnoughItem(material.ItemId, material.Amount))
            {
                Debug.LogWarning($"재료가 부족합니다: {itemDB.GetItemById(material.ItemId)} x {material.Amount}");
                return;
            }
        }

        foreach (var material in recipe.RequiredMaterials)
        {
            bool result = PlayerInfo.Instance.TryConsumeItem(material.ItemId, material.Amount);

            if (!result)
            {
                Debug.LogWarning($"재료 소비 실패: {itemDB.GetItemById(material.ItemId)} x {material.Amount}");
                return;
            }
        }

        DateTime completesAtUtc = DateTime.UtcNow.AddSeconds(recipe.CraftDuration);

        bool isStart = PlayerInfo.Instance.StartEquipmentCraft(0, recipe.RecipeId, DateTime.UtcNow, completesAtUtc);

        if (!isStart)
        {
            Debug.LogWarning("장비 제작 시작 실패");
            return;
        }

        RefreshRecipeSlots();
        craftUI.Bind(recipeDB, OnCraftCompleted);
    }

    private void OnCraftCompleted()
    {
        EquipmentCraftSlotSaveData craftSlot = PlayerInfo.Instance.GetEquipmentCraftSlot(0);

        if (craftSlot == null || !craftSlot.IsCrafting)
        {
            Debug.LogWarning("제작 슬롯 정보가 없거나 제작 중이 아닙니다.");
            return;
        }

        if (!PlayerInfo.Instance.IsEquipmentCraftComplete(0, DateTime.UtcNow))
        {
            Debug.LogWarning("제작이 아직 완료되지 않았습니다.");
            return;
        }

        if (equipmentSpawner == null)
        {
            Debug.LogWarning("EquipmentSpawner가 할당되지 않았습니다.");
            return;
        }

        EquipmentCraftRecipeSO recipe =
        recipeDB != null ? recipeDB.GetRecipeById(craftSlot.RecipeId) : null;

        if (recipe == null)
        {
            Debug.LogWarning("제작 완료 처리에 필요한 레시피를 찾을 수 없습니다.");
            return;
        }
        
        EquipGradeEnum selectecGrade = DecideGrade(recipe);

        Equipment equipment = equipmentSpawner.CreateEquipByGrade(selectecGrade);
        EquipmentSO equipmentSO = equipmentDB != null ? equipmentDB.GetEquipmentSO(equipment.EquipDataId) : null;

        if (equipment == null)
        {
            Debug.LogWarning("장비 제작 실패...");
            return;
        }

        EquipmentSaveData equipmentSaveData = new()
        {
            EquipDataId = equipment.EquipDataId,
            EquipId = equipment.EquipID,
            EquipLv = equipment.EquipLv,
            EquipGrade = equipment.EquipGrade,
            EquipPart = equipment.EquipPart,
            BonusHP = equipment.BonusHP,
            BonusAtk = equipment.BonusAtk,
            BonusDef = equipment.BonusDef,
            BonusCriChance = equipment.BonusCriChance
        };

        bool result = PlayerInfo.Instance.AddEquipment(equipmentSaveData);
        if (!result)
        {
            Debug.LogWarning("장비 인벤토리에 추가 실패...");
            return;
        }

        PlayerInfo.Instance.ClearEquipmentCraftSlot(craftSlot.SlotIndex);

        RefreshRecipeSlots();

        if (craftUI != null)
        {
            craftUI.Bind(recipeDB, OnCraftCompleted);
        }

        if (craftCompleteUI != null)
        {
            craftCompleteUI.SetCraftedItem(equipmentSaveData, equipmentSO, false);
        }
    }

    private EquipGradeEnum DecideGrade(EquipmentCraftRecipeSO recipe)
    {
        if (recipe == null || recipe.GradeRates == null || recipe.GradeRates.Count == 0)
        {
            Debug.LogWarning("레시피의 등급 확률 정보가 없습니다.");
            return default;
        }

        // 가중치가 100이 아니더라도 정상적으로 동작하도록 가중치 총합을 계산하고
        // 총합의 범위 내에서 랜덤 등급 결정하도록 함.
        int totalWeight = 0;

        foreach (var gradeRate in recipe.GradeRates)
        {
            totalWeight += gradeRate.Weight;
        }

        if (totalWeight <= 0)
        {
            Debug.LogWarning("레시피의 등급 확률 총합이 0 이하입니다.");
            return recipe.GradeRates[0].Grade;
        }

        int randomValue = UnityEngine.Random.Range(0, totalWeight);

        int accumulatedWeight = 0;

        // 가중치를 누적시키면서 랜덤 값과 비교하고 누적치가 랜덤 값보다 커지는 순간 해당 등급을 선택하도록 함.
        foreach (var gradeRate in recipe.GradeRates)
        {
            accumulatedWeight += gradeRate.Weight;
            if (randomValue < accumulatedWeight)
            {
                return gradeRate.Grade;
            }
        }

        // 만약 모든 가중치를 다 더했는데도 선택되지 않았다면, 제일 낮은 등급을 반환하도록 함.
        return recipe.GradeRates[0].Grade;
    }
}
