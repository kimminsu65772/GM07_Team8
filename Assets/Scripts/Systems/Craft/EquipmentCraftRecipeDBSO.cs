using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "EquipmentCraftRecipeDB", menuName = "Game/Craft/CraftDB")]
public class EquipmentCraftRecipeDB : ScriptableObject
{
    [SerializeField] private List<EquipmentCraftRecipeSO> recipes;

    private Dictionary<int, EquipmentCraftRecipeSO> recipeDict;

    public IReadOnlyList<EquipmentCraftRecipeSO> Recipes => recipes;

    private void OnEnable()
    {
        BuildRecipeDict();
    }

    public EquipmentCraftRecipeSO GetRecipeById(int recipeId)
    {
        if (recipeDict == null)
            BuildRecipeDict();
        if (recipeDict.TryGetValue(recipeId, out EquipmentCraftRecipeSO recipe))
        {
            return recipe;
        }
        Debug.LogError($"레시피 ID {recipeId}에 해당하는 레시피를 찾을 수 없습니다.");
        return null;
    }
    public bool ContainsRecipeId(int recipeId)
    {
        if (recipeDict == null)
            BuildRecipeDict();

        return recipeDict.ContainsKey(recipeId);
    }

    private void BuildRecipeDict()
    {
        recipeDict = new Dictionary<int, EquipmentCraftRecipeSO>();

        if (recipes == null)
            return;

        foreach (EquipmentCraftRecipeSO recipe in recipes)
        {
            if (recipe == null)
            {
                Debug.LogError("EquipmentCraftRecipeDB에 null 레시피가 포함되어 있습니다.");
                continue;
            }

            if (recipeDict.ContainsKey(recipe.RecipeId))
            {
                Debug.LogError($"중복된 레시피 ID {recipe.RecipeId}가 존재합니다.");
                continue;
            }

            recipeDict.Add(recipe.RecipeId, recipe);
        }
    }
}
