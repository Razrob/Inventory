using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryItemCrafter : MonoBehaviour
{
    [SerializeField] private InventoryHandler _inventoryHandler;
    [SerializeField] private InventoryDisplay _inventoryDisplay;

    [SerializeField] private UICell _freeCellPrefab;
    [SerializeField] private CraftsList _craftsList;

    [SerializeField] private Transform _craftListParent;
    [SerializeField] private Transform _craftListPagePrefab;
    [SerializeField] private int _maxCellsNumberInPage;
    [SerializeField] private UIRecipeCell _cellPrefab;


    private CraftRecipe _activeRecipe;
    private FreeCellData _freeCellData;

    private Transform _lastPage;

    

    private void Start()
    {
        _inventoryHandler.OnInventoryDisplayChanged += (_enabled) => { if (!_enabled) _activeRecipe = null; };

        InputModule.InputModuleInstance.OnMouseButtonUp += PutCraftedItem;
        InputModule.InputModuleInstance.OnMouseButtonDrag += Drag;
    }

    private void Drag(PointerEventData _data)
    {
        if(_freeCellData != null) _freeCellData.RectTransform.position = _data.position;
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

    private void PutCraftedItem(BaseEventData _eventData)
    {
        PointerEventData _data = (PointerEventData)_eventData;

        if (_freeCellData == null) return;

        if (_data.pointerCurrentRaycast.gameObject == null) ThrowItems();
        else
        {
            if (_data.pointerCurrentRaycast.gameObject.TryGetComponent(out UICell cell))
            {
                int _cellIndex = _inventoryDisplay.GetCellIndexInArray(cell);
                if (_cellIndex == -1) ThrowItems();
                else
                {
                    _freeCellData.InventoryCell.ItemNumber = _inventoryHandler.AddItemInSpecificCell(_freeCellData.InventoryCell.Item, _cellIndex, _freeCellData.InventoryCell.ItemNumber);
                    if (_freeCellData.InventoryCell.ItemNumber > 0) ThrowItems();
                }
            }
            else ThrowItems();
        }
        FinishMove();

    }
    private void ThrowItems()
    {
        _inventoryHandler.ThrowItems(_freeCellData.InventoryCell.Item, _freeCellData.InventoryCell.ItemNumber);
    }
    private void FinishMove()
    {
        if (_freeCellData != null) Destroy(_freeCellData.UICell.gameObject);
        _freeCellData = null;
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

    public void CraftItem(BaseEventData _eventData)
    {
        PointerEventData _data = (PointerEventData)_eventData;
        if (_activeRecipe == null || !_inventoryHandler.ItemStorage.CheckRecipeAvailability(_activeRecipe) || _freeCellData != null) return;

        Transform _freeCellTransform = Instantiate(_freeCellPrefab, transform.parent).transform;

        _freeCellData = new FreeCellData
        {
            InventoryCell = new InventoryCell(Instantiate(_inventoryHandler.CraftItem(_activeRecipe))),
            UICell = _freeCellTransform.GetComponent<UICell>(),
            RectTransform = _freeCellTransform.GetComponent<RectTransform>()
        };

        if (!_freeCellData.InventoryCell.Item.IsWearable && _freeCellData.InventoryCell.ItemNumber > 1) _freeCellData.UICell.ItemNumberUI.text = _freeCellData.InventoryCell.ItemNumber.ToString();
        _freeCellData.UICell.ItemImage.sprite = _freeCellData.InventoryCell.Item.ItemSprite;

    }

    private class FreeCellData
    {
        public RectTransform RectTransform;
        public UICell UICell;
        public InventoryCell InventoryCell;
    }


}
