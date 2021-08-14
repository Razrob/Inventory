using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActiveInventoryHandler : MonoBehaviour
{
    [SerializeField] private InventoryHandler _inventoryHandler;
    [SerializeField] private Transform _itemInHandParent;
    [SerializeField] private Image[] _selectableCells;
    [SerializeField] private GameObject _emptyItemPrefab;


    private int _activeCellIndex = 0;
    private GameObject _itemInHand;

    private void Start()
    {
        InputModule.InputModuleInstance.OnMouseScroll += (_scrollDelta) => { if (!_inventoryHandler.InventoryActive) ChangeSelectCell(_scrollDelta); };

        _itemInHand = Instantiate(_emptyItemPrefab, _itemInHandParent.TransformPoint(_inventoryHandler.InventoryProperty.ItemInHandLocalPosition), Quaternion.identity, _itemInHandParent);
        _itemInHand.transform.localRotation = Quaternion.identity;
        _itemInHand.SetActive(false);
        _inventoryHandler.ItemStorage.OnStorageChanged += PickUpItem;

        UpdateDisplayActiveCell();
        PickUpItem();
    }

    private void UpdateDisplayActiveCell()
    {
        foreach (Image image in _selectableCells) image.gameObject.SetActive(false);
        _selectableCells[_activeCellIndex].gameObject.SetActive(true);
    }

    private void PickUpItem()
    {
        Item _item = _inventoryHandler.ItemStorage.GetItemFromSpecificCellWithoutRemove(_activeCellIndex);
        if (_item == null) _itemInHand.SetActive(false);
        else
        {
            _itemInHand.GetComponent<MeshFilter>().sharedMesh = _item.GetComponent<MeshFilter>().sharedMesh;
            _itemInHand.GetComponent<MeshRenderer>().material = _item.GetComponent<MeshRenderer>().material;
            _itemInHand.transform.localScale = _item.transform.localScale;
            _itemInHand.SetActive(true);
        }

    }

    public void ThrowSelectedItem()
    {
        Item _item = _inventoryHandler.GetItemFromSpecificCell(_activeCellIndex);
        if(_item != null) _inventoryHandler.ThrowItems(_item, 1);
    }

    public void ThrowAllSelectedItems()
    {
        int _itemNumber;
        Item _item = _inventoryHandler.GetAllItemsFromSpecificCell(_activeCellIndex, out _itemNumber);
        if (_item != null) _inventoryHandler.ThrowItems(_item, _itemNumber);
    }

    public void ChangeSelectCell(int _scrollDelta)
    {
        _activeCellIndex -= _scrollDelta;
        while (_activeCellIndex < 0) _activeCellIndex = 6 + _activeCellIndex;
        while (_activeCellIndex >= _selectableCells.Length) _activeCellIndex = 0 + (_activeCellIndex - 6);
        UpdateDisplayActiveCell();
        PickUpItem();
    }

    

    
}
