using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{
    [SerializeField] private float _mouseSensitivity;
    [SerializeField] private float _moveSpeed;

    [SerializeField] private CharacterController _characterController;
    [SerializeField] private Transform _cameraTransform;
    private Camera _camera;

    [SerializeField] private InventoryHandler _inventoryHandler;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        _camera = _cameraTransform.GetComponent<Camera>();
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) _inventoryHandler.SetInventoryDisplay(!_inventoryHandler.InventoryActive);

        if (!_inventoryHandler.InventoryActive)
        {

            float hAxis = Input.GetAxis("Mouse X");
            float vAxis = Input.GetAxis("Mouse Y");

            transform.eulerAngles += Vector3.up * hAxis * _mouseSensitivity * Time.deltaTime;
            _camera.transform.eulerAngles += Vector3.left * vAxis * _mouseSensitivity * Time.deltaTime;

            Vector3 movement = transform.right * Input.GetAxis("Horizontal") + transform.forward * Input.GetAxis("Vertical");
            movement += Physics.gravity;
            _characterController.Move(movement * Time.deltaTime * _moveSpeed);


            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                if (Physics.Raycast(_camera.ScreenPointToRay(new Vector2(_camera.pixelWidth / 2, _camera.pixelHeight / 2)), out hit, 5))
                {
                    if (hit.transform.TryGetComponent(out Item _item))
                    {
                        _inventoryHandler.AddItem(_item);
                        if(!_item.IsWearable) Destroy(_item.gameObject);
                    }
                }
            }
        }
    }
}
