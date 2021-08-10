using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Create Crafts List")]
public class CraftsList : ScriptableObject
{
    [SerializeField] private CraftRecipe[] _craftRecipes;
    public CraftRecipe[] CraftRecipes => _craftRecipes;

}
