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

    public bool CheckRecipeAvailability(CraftRecipe _recipe)
    {
        if (_recipe == null) return false;
        foreach (CraftItemSet itemSet in _recipe.RequiredItems)
        {
            if (ItemNumberInStorage(itemSet.Item.ItemID) < itemSet.ItemNumber) return false;
        }
        return true;
    }

    public bool ContainsItemID(string _itemID)
    {
        foreach (InventoryCell cell in _cells) if (cell.CellID == _itemID) return true;
        return false;
    }
    public bool ContainsItem(Item _item)
    {
        foreach (InventoryCell cell in _cells) if (cell.Item == _item) return true;
        return false;
    }
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
            if (_cells[i].CellID == string.Empty) _cells[i] = new InventoryCell(_item);
            else if (_cells[i].CellID == _item.ItemID && _cells[i].ItemNumber < _cells[i].Item.MaxCountInCell) _cells[i].ItemNumber++;
            else continue;

            OnCellChanged?.Invoke(i, _cells[i]);
            return true;
        }

        if (_cells.Count < _maxCellNumber)
        {
            _cells.Add(new InventoryCell(_item));
            OnCellChanged?.Invoke(_cells.Count - 1, _cells[_cells.Count - 1]);
            return true;
        }

        return false;
    }

    public bool TryAddItemToSpecificCell(Item _item, int _cellIndex)
    {
        if (_cellIndex >= _maxCellNumber) return false;

        if (_cells[_cellIndex].CellID == string.Empty) _cells[_cellIndex] = new InventoryCell(_item);
        else if (_cells[_cellIndex].CellID == _item.ItemID && _cells[_cellIndex].ItemNumber < _item.MaxCountInCell) _cells[_cellIndex].ItemNumber++;
        else return false;

        OnCellChanged?.Invoke(_cellIndex, _cells[_cellIndex]);
        return true;

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