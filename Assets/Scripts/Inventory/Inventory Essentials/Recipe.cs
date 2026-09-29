using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Ingredient
{
    public ItemScript item;
    public int amount;
}


[CreateAssetMenu(fileName = "Recipe", menuName = "NewRecipe")]

//the required data to create a recipe
public class Recipe : ScriptableObject
{
    public List<Ingredient> ingredients;
    public ItemScript result;
    public int resultAmount = 1;
}
