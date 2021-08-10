using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryItemCrafter : MonoBehaviour
{
    [SerializeField] private InventoryHandler _inventoryHandler;
    [SerializeField] private InventoryDisplay _inventoryDisplay;
    [SerializeField] private Transform _craftListTransform;
    [SerializeField] private UIRecipeCell _cellPrefab;

    [SerializeField] private CraftsList _craftsList;


    private void AddCellToCraftList(CraftRecipe _recipe)
    {
        UIRecipeCell _cell = Instantiate(_cellPrefab, _craftListTransform);
        _cell.CraftRecipe = _recipe;

        _cell.ItemImage.sprite = _recipe.ReceivedItem.Item.ItemSprite;
        if(_recipe.ReceivedItem.ItemNumber > 1) _cell.ItemNumberUI.text = _recipe.ReceivedItem.ItemNumber.ToString();

        if (CheckRecipeAvailability(_recipe)) _cell.CraftAvailabilityPanel.SetActive(false);

        EventTrigger trigger = _cell.GetComponent<EventTrigger>();


        trigger.triggers[0].callback.AddListener((data) => SelectRecipe(data));
    }

    private bool CheckRecipeAvailability(CraftRecipe _recipe)
    {
        foreach(CraftItemSet itemSet in _recipe.RequiredItems)
        {
            if (_inventoryHandler.ItemStorage.ItemNumberInStorage(itemSet.Item.ItemID) < itemSet.ItemNumber) return false;
        }
        return true;
    }
    public void RefreshVisibleRecipes()
    {
        for (int i = 0; i < _craftListTransform.childCount; i++) Destroy(_craftListTransform.GetChild(i).gameObject);

        foreach (CraftRecipe _recipe in _craftsList.CraftRecipes)
        {
            foreach (CraftItemSet _itemSet in _recipe.RequiredItems)
            {
                if (_inventoryHandler.ItemStorage.ContainsItemID(_itemSet.Item.ItemID))
                {
                    AddCellToCraftList(_recipe);
                    break;
                }
            }
        }

    }

    public void SelectRecipe(BaseEventData _eventData)
    {
        PointerEventData _data = (PointerEventData)_eventData;

        UIRecipeCell _recipeCell = _data.pointerCurrentRaycast.gameObject.GetComponent<UIRecipeCell>();
        CraftRecipe _recipe = _recipeCell.CraftRecipe;

        _inventoryDisplay.DisplayRecipeInfoPanel(_recipe);

    }
}
