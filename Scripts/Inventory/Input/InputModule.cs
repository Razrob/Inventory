using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class InputModule : MonoBehaviour
{
    [SerializeField] private EventSystem _eventSystem;

    private static InputModule _inputModule;
    public static InputModule InputModuleInstance => _inputModule;

    public event Action<PointerEventData> OnMouseButtonUp;
    public event Action<PointerEventData> OnMouseButtonDown;
    public event Action<PointerEventData> OnMouseButtonDrag;

    private void Awake()
    {
        if (_inputModule == null) _inputModule = this;
    }


    private void Update()
    {
        for (int i = 0; i < 3; i++)
        {
            if (_eventSystem.currentInputModule.input.GetMouseButtonDown(i)) OnMouseButtonDown?.Invoke(GetEventData(_eventSystem.currentInputModule.input.mousePosition, (PointerEventData.InputButton)Enum.GetValues(typeof(PointerEventData.InputButton)).GetValue(i)));
            if (_eventSystem.currentInputModule.input.GetMouseButtonUp(i)) OnMouseButtonUp?.Invoke(GetEventData(_eventSystem.currentInputModule.input.mousePosition, (PointerEventData.InputButton)Enum.GetValues(typeof(PointerEventData.InputButton)).GetValue(i)));
            if (_eventSystem.currentInputModule.input.GetMouseButton(i)) OnMouseButtonDrag?.Invoke(GetEventData(_eventSystem.currentInputModule.input.mousePosition, (PointerEventData.InputButton)Enum.GetValues(typeof(PointerEventData.InputButton)).GetValue(i)));
        }

    }

    private PointerEventData GetEventData(Vector2 _position, PointerEventData.InputButton _inputButton)
    {
        PointerEventData _eventData = new PointerEventData(EventSystem.current);
        _eventData.position = _position;
        _eventData.button = _inputButton;

        List<RaycastResult> _raycasts = new List<RaycastResult>();
        EventSystem.current.RaycastAll(_eventData, _raycasts);
        
        if(_raycasts.Count > 0) _eventData.pointerCurrentRaycast = _raycasts[0];
        return _eventData;
    } 

}
