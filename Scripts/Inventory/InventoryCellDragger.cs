using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryCellDragger : MonoBehaviour
{

    [SerializeField] private PointerEventData.InputButton _buttonForMoveOneItem;
    [SerializeField] private PointerEventData.InputButton _buttonForMoveStackItems;

    [SerializeField] private InventoryHandler _inventoryHandler;
    [SerializeField] private InventoryDisplay _inventoryDisplay;
    [SerializeField] private UICell _draggingCellPrefab;

    private FreeCellData _freeCellData;

    private void Update()
    {
        if (_freeCellData != null) Drag();
        
    }
    private void Drag()
    {
        if (_freeCellData != null) _freeCellData.CellRectTransform.position = Input.mousePosition;
    }
    private void FinishDrag()
    {
        if (_freeCellData != null) Destroy(_freeCellData.FreeUICell.gameObject);
        _freeCellData = null;
    }

    private void CancelItemMove()
    {
        for(int i = _freeCellData.InventoryCell.ItemNumber; i > 0; i--)
        {
            _inventoryHandler.AddItemInSpecificCell(_freeCellData.InventoryCell.Item, _inventoryDisplay.GetCellIndexInArray(_freeCellData.EditableUICell));
        }
        FinishDrag();
    }
    private void ThrowItem()
    {
        for (int i = _freeCellData.InventoryCell.ItemNumber; i > 0; i--)
        {
            GameObject _item = _freeCellData.InventoryCell.Item.gameObject;
            if (!_freeCellData.InventoryCell.Item.IsWearable) _item = Instantiate(_freeCellData.InventoryCell.Item).gameObject;
            _item.transform.position = _inventoryHandler.transform.position;
            _item.transform.parent = null;
            _item.SetActive(true);
            _item.GetComponent<Rigidbody>().AddForce((_inventoryHandler.transform.forward + _inventoryHandler.transform.up) * 3, ForceMode.Impulse);
        }
        if(!_inventoryHandler.ItemStorage.ContainsItem(_freeCellData.InventoryCell.Item) && !_freeCellData.InventoryCell.Item.IsWearable) Destroy(_freeCellData.InventoryCell.Item.gameObject);

        FinishDrag();
    }


    public void PointerDown(BaseEventData _eventData)
    {
        PointerEventData _data = (PointerEventData)_eventData;

        if (_freeCellData != null || (_data.button != _buttonForMoveOneItem && _data.button != _buttonForMoveStackItems)) return;

        UICell _selectedCell = _data.pointerCurrentRaycast.gameObject.transform.GetComponent<UICell>();

        if (_selectedCell == null || _selectedCell.ItemImage.color.a == 0) return;
        Item _item;

        int _itemsNumber = 1;

        if (_data.button == _buttonForMoveStackItems) _item = _inventoryHandler.GetAllItemsFromSpecificCell(_inventoryDisplay.GetCellIndexInArray(_selectedCell), out _itemsNumber);
        else _item = _inventoryHandler.GetItemInSpecificCell(_inventoryDisplay.GetCellIndexInArray(_selectedCell));

        Transform _freeCellTransform = Instantiate(_draggingCellPrefab, _data.position, Quaternion.identity, transform).transform;

        _freeCellData = new FreeCellData
        {
            CellRectTransform = _freeCellTransform.GetComponent<RectTransform>(),
            FreeUICell = _freeCellTransform.GetComponent<UICell>(),
            EditableUICell = _selectedCell,
            InventoryCell = new InventoryCell(_item)
        };
        _freeCellData.InventoryCell.ItemNumber = _itemsNumber;

        _freeCellData.FreeUICell.ItemImage.sprite = _freeCellData.InventoryCell.Item.ItemSprite;
        if(_freeCellData.InventoryCell.ItemNumber > 1) _freeCellData.FreeUICell.ItemNumberUI.text = _freeCellData.InventoryCell.ItemNumber.ToString();
        if (_freeCellData.InventoryCell.Item.IsWearable)
        {
            _freeCellData.FreeUICell.ItemWearProgressSlider.gameObject.SetActive(true);
            _freeCellData.FreeUICell.ItemWearProgressSlider.value = _freeCellData.InventoryCell.Item.WearProgress;
            _freeCellData.FreeUICell.ItemWearProgressSlider.fillRect.GetComponent<Image>().color = _freeCellData.EditableUICell.ItemWearProgressSlider.fillRect.GetComponent<Image>().color;
        }
    }


    
    public void PointerUp(BaseEventData _eventData)
    {
        if (_freeCellData == null) return;
        PointerEventData _data = (PointerEventData)_eventData;

        if (_data.pointerCurrentRaycast.gameObject == null) ThrowItem();
        else
        {
            if (_data.pointerCurrentRaycast.gameObject.TryGetComponent(out UICell cell))
            {
                for (int i = _freeCellData.InventoryCell.ItemNumber; i > 0; i--)
                {
                    if (!_inventoryHandler.AddItemInSpecificCell(_freeCellData.InventoryCell.Item, _inventoryDisplay.GetCellIndexInArray(cell)))
                    {
                        CancelItemMove();
                        return;
                    }
                    else _freeCellData.InventoryCell.ItemNumber--;
                }
                FinishDrag();
            }
            else CancelItemMove();
        }
    }

    private class FreeCellData
    {
        public RectTransform CellRectTransform;
        public UICell EditableUICell;
        public UICell FreeUICell;
        public InventoryCell InventoryCell;
    }
}
