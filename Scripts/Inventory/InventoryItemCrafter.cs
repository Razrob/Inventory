using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryItemCrafter : MonoBehaviour
{
    [SerializeField] private InventoryHandler _inventoryHandler;
    [SerializeField] private Transform _craftListTransform;
    [SerializeField] private UIRecipeCell _cellPrefab;

    [SerializeField] private CraftsList _craftsList;

    public void RefreshVisibleRecipes()
    {
        for (int i = 0; i < _craftListTransform.childCount; i++) Destroy(_craftListTransform.GetChild(i).gameObject);

        foreach(CraftRecipe _recipe in _craftsList.CraftRecipes)
        {
            foreach(CraftItemSet _itemSet in _recipe.RequiredItems)
            {
                if (_inventoryHandler.ItemStorage.ContainsItemID(_itemSet.Item.ItemID))
                {
                    AddCellToCraftList(_recipe);
                    break;
                }
            }
        }

    }

    private void AddCellToCraftList(CraftRecipe _recipe)
    {
        UIRecipeCell _cell = Instantiate(_cellPrefab, _craftListTransform);
        _cell.CraftRecipe = _recipe;

        _cell.ItemImage.sprite = _recipe.ReceivedItem.Item.ItemSprite;
        if(_recipe.ReceivedItem.ItemNumber > 1) _cell.ItemNumberUI.text = _recipe.ReceivedItem.ItemNumber.ToString();

        if (CheckRecipeAvailability(_recipe)) _cell.CraftAvailabilityPanel.SetActive(false);
    }

    private bool CheckRecipeAvailability(CraftRecipe _recipe)
    {
        foreach(CraftItemSet itemSet in _recipe.RequiredItems)
        {
            if (_inventoryHandler.ItemStorage.ItemNumberInStorage(itemSet.Item.ItemID) < itemSet.ItemNumber) return false;
        }
        return true;
    }
}
