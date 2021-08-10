using UnityEngine;
using UnityEngine.UI;

public class InventoryDisplay : MonoBehaviour
{
    [SerializeField] private GameObject _inventoryUI;
    [SerializeField] private GameObject _craftListUI;
    [SerializeField] private UICell[] _inventoryCellsUI;


    public void SetInventoryDisplay(bool _enabled)
    {
        _inventoryUI.SetActive(_enabled); 
        _craftListUI.SetActive(_enabled);
    } 

    public int GetCellIndexInArray(UICell _cell)
    {
        for (int i = 0; i < _inventoryCellsUI.Length; i++) if (_inventoryCellsUI[i] == _cell) return i;
        return -1;
    }
    public void UpdateInventoryCell(int _cellIndex, InventoryCell _inventoryCell)
    {
        if (_inventoryCell.ItemNumber > 0)
        {
            if (!_inventoryCell.Item.IsWearable) _inventoryCellsUI[_cellIndex].ItemNumberUI.text = _inventoryCell.ItemNumber.ToString();
            else
            {
                _inventoryCellsUI[_cellIndex].ItemNumberUI.text = string.Empty;
                _inventoryCellsUI[_cellIndex].ItemWearProgressSlider.value = _inventoryCell.Item.WearProgress;
                _inventoryCellsUI[_cellIndex].ItemWearProgressSlider.fillRect.GetComponent<Image>().color = GetColorFromValue(_inventoryCell.Item.WearProgress);
            }

            if (_inventoryCell.ItemNumber < 2) SetImage(_inventoryCellsUI[_cellIndex].ItemImage, _inventoryCell.Item.ItemSprite);

            _inventoryCellsUI[_cellIndex].ItemWearProgressSlider.gameObject.SetActive(_inventoryCell.Item.IsWearable);
        }
        else
        {
            _inventoryCellsUI[_cellIndex].ItemNumberUI.text = string.Empty;
            _inventoryCellsUI[_cellIndex].ItemWearProgressSlider.gameObject.SetActive(false);
            SetImage(_inventoryCellsUI[_cellIndex].ItemImage);
        }
    } 

    private void SetImage(Image _image, Sprite _sprite = null)
    {
        if (_sprite == null) _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, 0);
        else
        {
            _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, 1);
            _image.sprite = _sprite;
        }
    }
    private Color GetColorFromValue(float value)
    {
        Color finalColor;

        if (value <= 0.5f)
        {
            finalColor = Color.red;
            finalColor += (Color.yellow - Color.red) / 0.5f * value;
        }
        else
        {
            finalColor = Color.yellow;
            finalColor += (Color.green - Color.yellow) / 0.5f * (value - 0.5f);
        }

        finalColor -= new Color(0.25f, 0.25f, 0, 0);
        return finalColor;
    }
}
