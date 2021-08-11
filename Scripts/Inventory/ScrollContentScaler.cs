using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ScrollContentScaler : MonoBehaviour
{
    [SerializeField] private int _elementNumberInLine;
    [SerializeField] private RectTransform _content;
    [SerializeField] private GridLayoutGroup _gridLayout;

    private float _minHeight;
    private float _lineHeight;

    private void Awake()
    {
        _minHeight = _content.sizeDelta.y;
        _lineHeight = _gridLayout.cellSize.y + _gridLayout.spacing.y;
    }
    public void ResizeContentScale()
    {
        float _height = _lineHeight * (_content.childCount / _elementNumberInLine) + _gridLayout.padding.top + _gridLayout.padding.bottom;
        if (_content.childCount % _elementNumberInLine > 0) _height += _lineHeight;

        if (_height > _minHeight) _content.sizeDelta = new Vector2(_content.sizeDelta.x, _height);

    }
}
