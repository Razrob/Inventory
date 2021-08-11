using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryItemCrafter : MonoBehaviour
{
    [SerializeField] private InventoryHandler _inventoryHandler;
    [SerializeField] private InventoryDisplay _inventoryDisplay;

    [SerializeField] private CraftsList _craftsList;

    [SerializeField] private Transform _craftListParent;
    [SerializeField] private Transform _craftListPagePrefab;
    [SerializeField] private int _maxCellsNumberInPage;
    [SerializeField] private UIRecipeCell _cellPrefab;


    private CraftRecipe _activeRecipe;
    private Transform _lastPage;

    private void Start()
    {
        _inventoryHandler.OnInventoryDisplayChanged += (_enabled) => { if (!_enabled) _activeRecipe = null; };
    }

    private void AddCellToCraftList(CraftRecipe _recipe)
    {
        if(_lastPage == null || _lastPage.childCount >= _maxCellsNumberInPage)
        {
            bool isFirst = _lastPage == null;
            _lastPage = Instantiate(_craftListPagePrefab, _craftListParent);
            _inventoryDisplay.AddCraftListPage(_lastPage);
            if (!isFirst) _lastPage.gameObject.SetActive(false);
        }

        UIRecipeCell _cell = Instantiate(_cellPrefab, _lastPage);
        _cell.CraftRecipe = _recipe;

        _cell.ItemImage.sprite = _recipe.ReceivedItem.Item.ItemSprite;
        if(_recipe.ReceivedItem.ItemNumber > 1) _cell.ItemNumberUI.text = _recipe.ReceivedItem.ItemNumber.ToString();

        if (_inventoryHandler.ItemStorage.CheckRecipeAvailability(_recipe)) _cell.CraftAvailabilityPanel.SetActive(false);

        EventTrigger trigger = _cell.GetComponent<EventTrigger>();

        trigger.triggers[0].callback.AddListener((data) => SelectRecipe(data));

    }

    public void RefreshVisibleRecipes()
    {
        List<GameObject> objectsForDestroy = new List<GameObject>();
        for (int i = 0; i < _craftListParent.childCount; i++) objectsForDestroy.Add(_craftListParent.GetChild(i).gameObject);
        for (int i = 0; i < objectsForDestroy.Count; i++) DestroyImmediate(objectsForDestroy[i]);
        _inventoryDisplay.ClearCraftListPages();
        _lastPage = null;

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

        _inventoryDisplay.ChangeActiveCraftListPage();
        _inventoryDisplay.DisplayRecipeInfoPanel(ref _activeRecipe, _inventoryHandler.ItemStorage.CheckRecipeAvailability(_activeRecipe));

    }

    public void SelectRecipe(BaseEventData _eventData)
    {
        PointerEventData _data = (PointerEventData)_eventData;

        UIRecipeCell _recipe = _data.pointerCurrentRaycast.gameObject.GetComponent<UIRecipeCell>();
        _activeRecipe = _recipe.CraftRecipe;

        _inventoryDisplay.DisplayRecipeInfoPanel(ref _activeRecipe, _inventoryHandler.ItemStorage.CheckRecipeAvailability(_activeRecipe));

    }


}
