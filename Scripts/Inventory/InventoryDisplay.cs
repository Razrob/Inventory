using UnityEngine;
using UnityEngine.UI;
using System;

public class InventoryDisplay : MonoBehaviour
{
    [SerializeField] private GameObject _inventoryUI;
    [SerializeField] private GameObject _craftListUI;
    [SerializeField] private GameObject _recipeInfoUI;

    [SerializeField] private UICell[] _inventoryCellsUI;
    [SerializeField] private RecipeInfoPanelUI _recipeInfoPanelUI;


    public void SetInventoryDisplay(bool _enabled)
    {
        _inventoryUI.SetActive(_enabled); 
        _craftListUI.SetActive(_enabled);
        if(!_enabled) _recipeInfoUI.SetActive(_enabled);
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

    public void DisplayRecipeInfoPanel(CraftRecipe _recipe)
    {

        for (int i = 0; i < _recipeInfoPanelUI.RequiredItemParent.childCount; i++) Destroy(_recipeInfoPanelUI.RequiredItemParent.GetChild(i).gameObject);

        _recipeInfoUI.SetActive(true);
        _recipeInfoPanelUI.ReceivedItemUI.ItemImage.sprite = _recipe.ReceivedItem.Item.ItemSprite;
        _recipeInfoPanelUI.ReceivedItemUI.ItemNumberUI.text = $"{_recipe.ReceivedItem.Item.ItemName} x{_recipe.ReceivedItem.ItemNumber}";

        foreach(CraftItemSet _itemSet in _recipe.RequiredItems)
        {
            UICell _cell = Instantiate(_recipeInfoPanelUI.RequiredItemsUIPrefab, _recipeInfoPanelUI.RequiredItemParent);
            _cell.ItemImage.sprite = _itemSet.Item.ItemSprite;
            _cell.ItemNumberUI.text = $"{_itemSet.Item.ItemName} x{_itemSet.ItemNumber}";
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

    [Serializable]
    private class RecipeInfoPanelUI
    {
        public Transform RequiredItemParent;
        public UICell ReceivedItemUI;
        public UICell RequiredItemsUIPrefab;
    }
}
