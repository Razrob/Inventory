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

    private bool ContainsItemIdInItemsPool(string _itemID)
    {
        for(int i = 0; i < _itemsParent.childCount; i++)
        {
            if (_itemsParent.GetChild(i).GetComponent<Item>().ItemID == _itemID) return true;
        }
        return false;
    }
    private Item GetItemFromItemsPool(string _itemID)
    {
        for (int i = 0; i < _itemsParent.childCount; i++)
        {
            Item _item = _itemsParent.GetChild(i).GetComponent<Item>();
            if (_item.ItemID == _itemID) return _item;
        }
        return null;
    }

    private void CheckItemAvailability(ref Item _item)
    {
        if (!_item.IsWearable)
        {
            GameObject _oldItem = _item.gameObject;
            if (!ContainsItemIdInItemsPool(_item.ItemID)) _item = Instantiate(_item);
            else _item = GetItemFromItemsPool(_item.ItemID);
            
           if(_oldItem.activeInHierarchy) Destroy(_oldItem);
        }
        _item.transform.parent = _itemsParent;
        _item.gameObject.SetActive(false);
        _item.gameObject.name = _item.ItemName;
    }

    private void CreateThrowedObject(Item _item)
    {
        GameObject _itemGameObject = _item.gameObject;
        if (!_item.IsWearable) _itemGameObject = Instantiate(_item).gameObject;
        else _itemGameObject.transform.parent = null;

        _itemGameObject.name = _item.ItemName;

        _itemGameObject.SetActive(true);
        _itemGameObject.transform.position = transform.position;
        _itemGameObject.GetComponent<Rigidbody>().AddForce((transform.forward + transform.up) * 3, ForceMode.Impulse);
    }
    public void ThrowItems(Item _item, int _itemNumber)
    {
        for(int i = 0; i < _itemNumber; i++) CreateThrowedObject(_item);
        if (!_itemStorage.ContainsItemID(_item.ItemID) && !_item.IsWearable) Destroy(_item.gameObject);
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

    public int AddItemInSpecificCell(Item _item, int _cellIndex, int _itemNumber = 1)
    {
        CheckItemAvailability(ref _item);
        return _itemStorage.TryAddItemsToSpecificCell(_item, _cellIndex, _itemNumber);
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
