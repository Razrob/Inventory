using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Create Inventory Property")]
public class InventoryProperty : ScriptableObject
{
    [SerializeField] private PointerEventData.InputButton _buttonForMoveOneItem;
    [SerializeField] private PointerEventData.InputButton _buttonForMoveStackItems;
    [SerializeField] private Vector3 _itemInHandLocalPosition;

    public PointerEventData.InputButton ButtonForMoveOneItem => _buttonForMoveOneItem;
    public PointerEventData.InputButton ButtonForMoveStackItems => _buttonForMoveStackItems;
    public Vector3 ItemInHandLocalPosition => _itemInHandLocalPosition;
}
