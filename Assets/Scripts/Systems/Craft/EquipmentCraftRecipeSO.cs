using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EquipmentCraftRecipeSO", menuName = "Game/Craft/EquipmentCraftRecipeSO")]
public class EquipmentCraftRecipeSO : ScriptableObject
{
    [SerializeField] private int recipeId;
    [SerializeField] private string recipeName;
    [SerializeField] private Sprite recipeIcon;
    [TextArea]
    [SerializeField] private string recipeDescription;
    [SerializeField] private List<EquipmentGradeRate> gradeRates;
    [SerializeField] private List<ItemAmount> requiredMaterials;
    [SerializeField] private int craftDurationInSeconds;

    public int RecipeId => recipeId;
    public string RecipeName => recipeName;
    public Sprite RecipeIcon => recipeIcon;
    public string RecipeDescription => recipeDescription;
    public IReadOnlyList<EquipmentGradeRate> GradeRates => gradeRates;
    public IReadOnlyList<ItemAmount> RequiredMaterials => requiredMaterials;
    public int CraftDuration => craftDurationInSeconds;

    private void OnValidate()
    {
        if (gradeRates != null)
        {
            int totalWeight = 0;
            foreach (var rate in gradeRates)
            {
                totalWeight += rate.Weight;
            }
            if (totalWeight != 100)
            {
                Debug.LogWarning($"가중치의 총합은 100이어야 합니다.");
            }
        }
    }
}

[Serializable]
public class ItemAmount
{
    public int ItemId;
    public int Amount;
}

[Serializable]
public class EquipmentGradeRate
{
    public EquipGradeEnum Grade;
    public int Weight;
}
