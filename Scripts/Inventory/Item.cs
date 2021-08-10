using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    [SerializeField] private int _maxCountInCell;
    [SerializeField] private string _itemName;
    [SerializeField, TextArea] private string _itemDescription;
    [SerializeField] private string _itemID;
    [SerializeField] private Sprite _itemSprite;
    [SerializeField] private bool _isWearable;

    public float WearProgress;

    public int MaxCountInCell => _maxCountInCell;
    public string ItemName => _itemName;
    public string ItemDescription => _itemDescription;
    public string ItemID => _itemID; 
    public Sprite ItemSprite => _itemSprite; 
    public bool IsWearable => _isWearable; 

    private void Start()
    {
        if (_isWearable)
        {
            
            WearProgress = Random.Range(0, 1f);
            _maxCountInCell = 1;
        }

    }

}
