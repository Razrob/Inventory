using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPool : MonoBehaviour
{
    [SerializeField] private Transform _itemsParent;
    [SerializeField] private InventoryHandler _inventoryHandler;

    private List<Item> _items = new List<Item>();


    private bool ContainsItemId(string _itemID) => _items.Exists((_item) => { return _item.ItemID == _itemID; });
    private Item GetItemFromPool(string _itemID) => _items.Find((_item) => { return _item.ItemID == _itemID; });

    public void AppendItem(ref Item _item)
    {
        if (_item == null) return;
        if (!_item.IsWearable)
        {
            GameObject _oldItem = _item.gameObject;

            if (!ContainsItemId(_item.ItemID))
            {
                _items.Add(Instantiate(_item));
                _item = _items[_items.Count - 1];
            }
            else _item = GetItemFromPool(_item.ItemID);

            if (_oldItem.activeInHierarchy) Destroy(_oldItem);
        }
        _item.transform.parent = _itemsParent;
        _item.gameObject.SetActive(false);
        _item.gameObject.name = _item.ItemName;
    }

    public void ExtractItem(Item _item)
    {
        if (_item == null) return;
        if (!_inventoryHandler.ItemStorage.ContainsItemID(_item.ItemID) && !_item.IsWearable)
        {
            _item = GetItemFromPool(_item.ItemID);
            _items.Remove(_item);
            if(_item != null) Destroy(_item.gameObject);
        }
    }
}
