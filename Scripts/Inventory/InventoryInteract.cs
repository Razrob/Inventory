using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryInteract : MonoBehaviour
{
    [SerializeField] private InventoryHandler _inventoryHandler;
    [SerializeField] private ActiveInventoryHandler _activeInventoryHandler;
    [SerializeField] private Camera _camera;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) _inventoryHandler.SetInventoryDisplay(!_inventoryHandler.InventoryActive);
        if (Input.GetKeyDown(KeyCode.Q) && !_inventoryHandler.InventoryActive)
        {
            if (Input.GetKey(KeyCode.LeftControl)) _activeInventoryHandler.ThrowAllSelectedItems();
            else _activeInventoryHandler.ThrowSelectedItem();
        }

        if (!_inventoryHandler.InventoryActive) 
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                if (Physics.Raycast(_camera.ScreenPointToRay(new Vector2(_camera.pixelWidth / 2, _camera.pixelHeight / 2)), out hit, 5))
                {
                    if (hit.transform.TryGetComponent(out Item _item))
                    {
                        _inventoryHandler.TryAddItems(_item);
                    }
                }
            }
        }
    }
}
