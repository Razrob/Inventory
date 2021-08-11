using System.Collections.Generic;
using UnityEngine;
using System;

public class InventoryHandler : MonoBehaviour
{
    [SerializeField] private InventoryDisplay _inventoryDisplay;
    [SerializeField] private InventoryItemCrafter _inventoryItemCrafter;
    [SerializeField] private Transform _itemsParent;
    [SerializeField] private Item[] items;
    [SerializeField] private int _storageCapacity = 16;

    public event Action<bool> OnInventoryDisplayChanged;

    private ItemStorage _itemStorage;
    private bool _inventoryActive;

    public bool InventoryActive => _inventoryActive;
    public ItemStorage ItemStorage => _itemStorage; 
    public InventoryDisplay InventoryDisplay => _inventoryDisplay;

    private void Start()
    {
        _itemStorage = new ItemStorage(_storageCapacity);

        _itemStorage.OnCellChanged += _inventoryDisplay.UpdateInventoryCell;
        _itemStorage.OnStorageChanged += _inventoryItemCrafter.RefreshVisibleRecipes;

        for (int i = 0; i < 15; i++)
        {
            AddItem(items[UnityEngine.Random.Range(0, items.Length)]);

        }
    }

    private void CheckItemAvailability(ref Item _item)
    {
        if (!_item.IsWearable)
        {
            if (_itemStorage.GetItemWithoutRemove(_item.ItemID) == null) _item = Instantiate(_item);
            else _item = _itemStorage.GetItemWithoutRemove(_item.ItemID);
        }
        _item.transform.parent = _itemsParent;
        _item.gameObject.SetActive(false);
    }
    public void SetInventoryDisplay(bool _enabled)
    {
        _inventoryDisplay.SetInventoryDisplay(_enabled);
        if (_enabled) Cursor.lockState = CursorLockMode.None;
        else Cursor.lockState = CursorLockMode.Locked;
        _inventoryActive = _enabled;

        if (_inventoryActive) _inventoryItemCrafter.RefreshVisibleRecipes();
        _inventoryDisplay.ResetActiveCraftListPage();

        OnInventoryDisplayChanged?.Invoke(_enabled);
    }

    public bool AddItem(Item _item)
    {
        CheckItemAvailability(ref _item);
        return _itemStorage.TryAddItem(_item);
    } 

    public bool AddItemInSpecificCell(Item _item, int _cellIndex)
    {
        CheckItemAvailability(ref _item);
        return _itemStorage.TryAddItemToSpecificCell(_item, _cellIndex);
    }

    public Item GetItem(string _itemID)
    {
        return _itemStorage.GetItem(_itemID);
    }

    public Item GetItemInSpecificCell(int _cellIndex)
    {
        return _itemStorage.GetItemFromSpecificCell(_cellIndex);
    }

    public Item GetAllItemsFromSpecificCell(int _cellIndex, out int _itemsNumber)
    {
        return _itemStorage.GetAllItemsFromSpecificCell(_cellIndex, out _itemsNumber);
    }
}
