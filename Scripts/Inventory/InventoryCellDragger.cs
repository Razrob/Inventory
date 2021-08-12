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


    private void Start()
    {
        InputModule.InputModuleInstance.OnMouseButtonDrag += Drag;    
    }
    private void Drag(PointerEventData _data)
    {
        if (_freeCellData != null) _freeCellData.CellRectTransform.position = _data.position;
    }
    private void FinishMove()
    {
        if (_freeCellData.InventoryCell.ItemNumber > 0) _inventoryHandler.AddItemInSpecificCell(_freeCellData.InventoryCell.Item, _inventoryDisplay.GetCellIndexInArray(_freeCellData.EditableUICell), _freeCellData.InventoryCell.ItemNumber);

        if (_freeCellData != null) Destroy(_freeCellData.FreeUICell.gameObject);
        _freeCellData = null;
    }

    private void ThrowItem()
    {
        _inventoryHandler.ThrowItems(_freeCellData.InventoryCell.Item, _freeCellData.InventoryCell.ItemNumber);
        _freeCellData.InventoryCell.ItemNumber = 0;
        FinishMove();
    }


    public void TakeItem(BaseEventData _eventData)
    {
        PointerEventData _data = (PointerEventData)_eventData;

        if (_freeCellData != null || (_data.button != _buttonForMoveOneItem && _data.button != _buttonForMoveStackItems)) return;

        UICell _selectedCell = _data.pointerCurrentRaycast.gameObject.transform.GetComponent<UICell>();

        if (_selectedCell == null || _selectedCell.ItemImage.color.a == 0) return;
        Item _item;

        int _itemsNumber = 1;

        if (_data.button == _buttonForMoveStackItems) _item = _inventoryHandler.ItemStorage.GetAllItemsFromSpecificCell(_inventoryDisplay.GetCellIndexInArray(_selectedCell), out _itemsNumber);
        else _item = _inventoryHandler.ItemStorage.GetItemFromSpecificCell(_inventoryDisplay.GetCellIndexInArray(_selectedCell));

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
        if (_freeCellData.InventoryCell.Item.IsWearable && _freeCellData.InventoryCell.Item.WearProgress < 1)
        {
            _freeCellData.FreeUICell.ItemWearProgressSlider.gameObject.SetActive(true);
            _freeCellData.FreeUICell.ItemWearProgressSlider.value = _freeCellData.InventoryCell.Item.WearProgress;
            _freeCellData.FreeUICell.ItemWearProgressSlider.fillRect.GetComponent<Image>().color = _freeCellData.EditableUICell.ItemWearProgressSlider.fillRect.GetComponent<Image>().color;
        }
    }


    
    public void PutItem(BaseEventData _eventData)
    {
        if (_freeCellData == null) return;
        PointerEventData _data = (PointerEventData)_eventData;

        if (_data.pointerCurrentRaycast.gameObject == null) ThrowItem();
        else
        {
            if (_data.pointerCurrentRaycast.gameObject.TryGetComponent(out UICell cell))
            {
                _freeCellData.InventoryCell.ItemNumber = _inventoryHandler.AddItemInSpecificCell(_freeCellData.InventoryCell.Item, _inventoryDisplay.GetCellIndexInArray(cell), _freeCellData.InventoryCell.ItemNumber);
            }
            FinishMove();
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
