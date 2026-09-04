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
                continue;
            }

            if (recipeDict.ContainsKey(recipe.RecipeId))
            {
                continue;
            }

            recipeDict.Add(recipe.RecipeId, recipe);
        }
    }
}
