using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ItemStorage
{
    private readonly int _maxCellNumber;
    private List<InventoryCell> _cells = new List<InventoryCell>();

    public event Action<int, InventoryCell> OnCellChanged;
    public event Action OnStorageChanged;

    public ItemStorage(int maxCellNumber)
    {
        _maxCellNumber = maxCellNumber;
        for (int i = 0; i < _maxCellNumber; i++) _cells.Add(new InventoryCell());
        OnCellChanged += OnChanged;
    }

    private void OnChanged(int _index, InventoryCell _cell) => OnStorageChanged?.Invoke();

    private void ReplaceCell(int _cellIndex, InventoryCell _inventoryCell)
    {
        _cells[_cellIndex] = _inventoryCell;
        OnCellChanged?.Invoke(_cellIndex, _inventoryCell);
    }
    private void IncreaseItemNumberInCell(int _cellIndex, int _itemNumber)
    {
        _cells[_cellIndex].ItemNumber += _itemNumber;
        OnCellChanged?.Invoke(_cellIndex, _cells[_cellIndex]);
    }

    public bool CheckRecipeAvailability(CraftRecipe _recipe)
    {
        if (_recipe == null) return false;
        foreach (CraftItemSet itemSet in _recipe.RequiredItems)
        {
            if (ItemNumberInStorage(itemSet.Item.ItemID) < itemSet.ItemNumber) return false;
        }
        return true;
    }

    public Item CraftItem(CraftRecipe _recipe)
    {
        if(CheckRecipeAvailability(_recipe))
        {
            for(int i = 0; i < _recipe.RequiredItems.Length; i++)
            {
                for (int x = 0; x < _recipe.RequiredItems[i].ItemNumber; x++) GetItem(_recipe.RequiredItems[i].Item.ItemID);
            }

            return _recipe.ReceivedItem.Item;
        }
        return null;
    }

    public bool ContainsItemID(string _itemID) => _cells.Exists((match) => { return match.CellID == _itemID; } );
    public bool ContainsItem(Item _item) => _cells.Exists((match) => { return match.Item == _item; });
    
    public int ItemNumberInCell(int _cellIndex)
    {
        if (_cellIndex >= _cells.Count) return -1;
        return _cells[_cellIndex].ItemNumber;
    }

    public int ItemNumberInStorage(string _itemID)
    {
        int _number = 0;
        foreach (InventoryCell cell in _cells) if (cell.CellID == _itemID) _number += cell.ItemNumber;
        return _number;
    }

    public bool TryAddItem(Item _item)
    {

        for (int i = 0; i < _cells.Count; i++)
        {
            if (_cells[i].CellID == string.Empty) ReplaceCell(i, new InventoryCell(_item));
            else if (_cells[i].CellID == _item.ItemID && _cells[i].ItemNumber < _cells[i].Item.MaxCountInCell) IncreaseItemNumberInCell(i, 1);
            else continue;

            return true;
        }

        return false;
    }

    public int TryAddItemsToSpecificCell(Item _item, int _cellIndex, int _itemNumber = 1) //returns number of not added items
    {
        if (_cellIndex >= _maxCellNumber) return _itemNumber;

        if (_cells[_cellIndex].CellID == string.Empty)
        {
            ReplaceCell(_cellIndex, new InventoryCell(_item, _itemNumber));
            return 0;
        }
        else if (_cells[_cellIndex].CellID == _item.ItemID && _cells[_cellIndex].ItemNumber < _item.MaxCountInCell)
        {
            int _addedItemNumber = Mathf.Min(_cells[_cellIndex].Item.MaxCountInCell - _cells[_cellIndex].ItemNumber, _itemNumber);
            IncreaseItemNumberInCell(_cellIndex, _addedItemNumber);
            return _itemNumber - _addedItemNumber;

        }
        else return _itemNumber;

    }

    public Item GetItem(string _itemID)
    {
        for (int i = 0; i < _cells.Count; i++)
        {
            if (_cells[i].CellID == _itemID)
            {
                _cells[i].ItemNumber--;
                if (_cells[i].ItemNumber < 1) _cells[i] = new InventoryCell();
            }
            else continue;

            OnCellChanged?.Invoke(i, _cells[i]);
            return _cells[i].Item;
        }

        return null;
    }
    public Item GetItemWithoutRemove(string _itemID)
    {
        for (int i = 0; i < _cells.Count; i++)
        {
            if (_cells[i].CellID == _itemID) return _cells[i].Item;
        }

        return null;
    }

    public Item GetItemFromSpecificCell(int _cellIndex)
    {
        if (_cellIndex >= _cells.Count || _cells[_cellIndex].ItemNumber < 1) return null;

        Item _item = _cells[_cellIndex].Item;
        _cells[_cellIndex].ItemNumber--;
        if (_cells[_cellIndex].ItemNumber < 1)
        {
            _cells[_cellIndex] = new InventoryCell();

        }

        OnCellChanged?.Invoke(_cellIndex, _cells[_cellIndex]);
        return _item;
    }

    public Item GetAllItemsFromSpecificCell(int _cellIndex, out int itemsNumber)
    {
        itemsNumber = 0;
        if (_cellIndex >= _cells.Count || _cells[_cellIndex].ItemNumber < 1) return null;

        itemsNumber = _cells[_cellIndex].ItemNumber;
        Item _item = _cells[_cellIndex].Item;
        _cells[_cellIndex] = new InventoryCell();

        OnCellChanged?.Invoke(_cellIndex, _cells[_cellIndex]);
        return _item;
    }
}