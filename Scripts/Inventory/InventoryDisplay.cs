using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using System.Collections.Generic;

public class InventoryDisplay : MonoBehaviour
{
    [SerializeField] private GameObject _inventoryUI;
    [SerializeField] private GameObject _craftListUI;
    [SerializeField] private GameObject _recipeInfoUI;

    [SerializeField] private UICell[] _inventoryCellsUI;

    [SerializeField] private TextMeshProUGUI _pageCounter;
    [SerializeField] private RecipeInfoPanelUI _recipeInfoPanelUI;

    private List<Transform> _craftListPages = new List<Transform>();
    private int _activePageIndex = 0;

    private bool _inventoryDisplay;

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

    private void UpdatePageCounterDisplay()
    {
        _pageCounter.text = $"{_activePageIndex + 1} / {_craftListPages.Count}";
    }

    private bool ContainsRecipe(CraftRecipe _recipe)
    {
        foreach(Transform page in _craftListPages)
        {
            for(int i = 0; i < page.childCount; i++)
            {
                UIRecipeCell _recipeCell = page.GetChild(i).GetComponent<UIRecipeCell>();
                if (_recipe == _recipeCell.CraftRecipe) return true;
            }
        }

        return false;
    }

    public void ResetActiveCraftListPage()
    {
        _activePageIndex = 0;
        ChangeActiveCraftListPage();
    }

    public void ChangeActiveCraftListPage(int _indexOffcet = 0)
    {
        if (_craftListPages.Count <= 0)
        {
            _activePageIndex = 0;
            _craftListUI.SetActive(false);
            UpdatePageCounterDisplay();
            return;
        }
        _craftListUI.SetActive(_inventoryDisplay);

        if (_activePageIndex >= _craftListPages.Count) _activePageIndex = 0;
        if (_activePageIndex + _indexOffcet < 0 || _activePageIndex + _indexOffcet >= _craftListPages.Count) return;

        foreach (Transform transform in _craftListPages) transform.gameObject.SetActive(false);

        _activePageIndex += _indexOffcet;
        _craftListPages[_activePageIndex].gameObject.SetActive(true);
        UpdatePageCounterDisplay();
    }

    public void AddCraftListPage(Transform _page)
    {
        _craftListPages.Add(_page);
        UpdatePageCounterDisplay();
    }
    public void ClearCraftListPages()
    {
        _craftListPages.Clear(); 
    }

    public void SetInventoryDisplay(bool _enabled)
    {
        _inventoryUI.SetActive(_enabled); 
        _craftListUI.SetActive(_enabled);
        if(!_enabled) _recipeInfoUI.SetActive(_enabled);
        _inventoryDisplay = _enabled;
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

    public void DisplayRecipeInfoPanel(ref CraftRecipe _recipe, bool _recipeAvailability)
    {
        if(!ContainsRecipe(_recipe))
        {
            _recipeInfoUI.SetActive(false);
            _recipe = null;
            return;
        }

        List<GameObject> objectsForDestroy = new List<GameObject>();
        for (int i = 0; i < _recipeInfoPanelUI.RequiredItemParent.childCount; i++) objectsForDestroy.Add(_recipeInfoPanelUI.RequiredItemParent.GetChild(i).gameObject);
        for (int i = 0; i < objectsForDestroy.Count; i++) DestroyImmediate(objectsForDestroy[i]);

        _recipeInfoUI.SetActive(true);

        _recipeInfoPanelUI.ReceivedItemImage.sprite = _recipe.ReceivedItem.Item.ItemSprite;
        _recipeInfoPanelUI.ReceivedItemName.text = $"{_recipe.ReceivedItem.Item.ItemName} x{_recipe.ReceivedItem.ItemNumber}";
        _recipeInfoPanelUI.ReceivedItemDescription.text = _recipe.ReceivedItem.Item.ItemDescription;

        foreach (CraftItemSet _itemSet in _recipe.RequiredItems)
        {
            UICell _cell = Instantiate(_recipeInfoPanelUI.RequiredItemsUIPrefab, _recipeInfoPanelUI.RequiredItemParent);
            _cell.ItemImage.sprite = _itemSet.Item.ItemSprite;
            _cell.ItemNumberUI.text = $"{_itemSet.Item.ItemName} x{_itemSet.ItemNumber}";
        }

        _recipeInfoPanelUI.AvailabilityPanel.SetActive(!_recipeAvailability);

    }

    

    [Serializable]
    private class RecipeInfoPanelUI
    {
        public Transform RequiredItemParent;

        public Image ReceivedItemImage;
        public TextMeshProUGUI ReceivedItemName;
        public TextMeshProUGUI ReceivedItemDescription;

        public UICell RequiredItemsUIPrefab;

        public GameObject AvailabilityPanel;
    }
}
