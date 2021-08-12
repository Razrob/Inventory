using System.Collections.Generic;
using UnityEngine;
using System;

public class InventoryHandler : MonoBehaviour
{
    [SerializeField] private ItemPool _itemPool;
    [SerializeField] private InventoryDisplay _inventoryDisplay;
    [SerializeField] private InventoryItemCrafter _inventoryItemCrafter;
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
        _itemPool.GetItem(_item);
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

    public Item CraftItem(CraftRecipe _recipe)
    {
        Item _item = _itemStorage.CraftItem(_recipe); 
        foreach (CraftItemSet _itemSet in _recipe.RequiredItems) _itemPool.GetItem(_itemSet.Item);
        return _item;
    }

    public bool AddItem(Item _item)
    {
        _itemPool.AddItem(ref _item);
        return _itemStorage.TryAddItem(_item);
    } 

    public int AddItemInSpecificCell(Item _item, int _cellIndex, int _itemNumber = 1)
    {
        _itemPool.AddItem(ref _item);
        return _itemStorage.TryAddItemsToSpecificCell(_item, _cellIndex, _itemNumber);
    }

    public Item GetItem(string _itemID)
    {
        Item _item = _itemStorage.GetItem(_itemID);
        _itemPool.GetItem(_item);
        return _item;
    }

    public Item GetItemFromSpecificCell(int _cellIndex)
    {
        Item _item = _itemStorage.GetItemFromSpecificCell(_cellIndex);
        _itemPool.GetItem(_item);
        return _item;
    }

    public Item GetAllItemsFromSpecificCell(int _cellIndex, out int _itemsNumber)
    {
        return _itemStorage.GetAllItemsFromSpecificCell(_cellIndex, out _itemsNumber);
    }
}
