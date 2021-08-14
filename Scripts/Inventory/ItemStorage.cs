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

    private int ReplaceCell(int _cellIndex, InventoryCell _inventoryCell)
    {
        _cells[_cellIndex] = _inventoryCell;
        OnCellChanged?.Invoke(_cellIndex, _inventoryCell);
        return _inventoryCell.ItemNumber;
    }
    private int ChangeItemNumberInCell(int _cellIndex, int _itemNumber)
    {
        _cells[_cellIndex].ItemNumber += _itemNumber;
        if (_cells[_cellIndex].ItemNumber < 1) _cells[_cellIndex] = new InventoryCell();
        OnCellChanged?.Invoke(_cellIndex, _cells[_cellIndex]);
        return _itemNumber;
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
    public Item CraftAllItems(CraftRecipe _recipe, out int _itemNumber)
    {
        _itemNumber = 0;
        int _maxNumber = _recipe.ReceivedItem.Item.MaxCountInCell;
        Item _item = null;
        while (CheckRecipeAvailability(_recipe) && _maxNumber > 0)
        {
            _item = CraftItem(_recipe);
            _itemNumber++;
            _maxNumber--;
        }
        return _item;

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

    public int TryAddItems(Item _item, int _itemNumber = 1) //returns number of not added items
    {
        int _addedItemNumber = 0;
        for (int i = 0; i < _cells.Count; i++)
        {
            if (_addedItemNumber >= _itemNumber) return 0;

            if (_cells[i].CellID == string.Empty)
            {
                _addedItemNumber += ReplaceCell(i, new InventoryCell(_item, Mathf.Min(_itemNumber - _addedItemNumber, _item.MaxCountInCell)));
            }
            else if (_cells[i].CellID == _item.ItemID && _cells[i].ItemNumber < _cells[i].Item.MaxCountInCell)
            {
                _addedItemNumber += ChangeItemNumberInCell(i, Mathf.Min(_itemNumber - _addedItemNumber, _item.MaxCountInCell - _cells[i].ItemNumber));
            }
        }

        return _itemNumber - _addedItemNumber;
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
            ChangeItemNumberInCell(_cellIndex, _addedItemNumber);
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
                ChangeItemNumberInCell(i, -1);
                return _cells[i].Item;
            }
        }

        return null;
    }
    public Item GetItemWithoutRemove(string _itemID) => _cells.Find((_cell) => { return _cell.CellID == _itemID; }).Item;
    public Item GetItemFromSpecificCellWithoutRemove(int _cellIndex) => _cellIndex >= _cells.Count ? null : _cells[_cellIndex].Item;

    public Item GetItemFromSpecificCell(int _cellIndex)
    {
        if (_cellIndex >= _cells.Count || _cells[_cellIndex].ItemNumber < 1) return null;

        Item _item = _cells[_cellIndex].Item;
        ChangeItemNumberInCell(_cellIndex, -1);

        return _item;
    }

    public Item GetAllItemsFromSpecificCell(int _cellIndex, out int _itemsNumber)
    {
        _itemsNumber = 0;
        if (_cellIndex >= _cells.Count || _cells[_cellIndex].ItemNumber < 1) return null;

        _itemsNumber = _cells[_cellIndex].ItemNumber;
        Item _item = _cells[_cellIndex].Item;
        ReplaceCell(_cellIndex, new InventoryCell());

        return _item;
    }
}