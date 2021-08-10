using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    [SerializeField] private int _maxCountInCell;
    [SerializeField] private string _itemID;
    [SerializeField] private Sprite _itemSprite;
    [SerializeField] private bool _isWearable;

    public float WearProgress;

    public int MaxCountInCell { get { return _maxCountInCell; } } 
    public string ItemID { get { return _itemID; } }
    public Sprite ItemSprite { get { return _itemSprite; } }
    public bool IsWearable { get { return _isWearable; } }

    private void Start()
    {
        if (_isWearable)
        {
            
            WearProgress = Random.Range(0, 1f);
            _maxCountInCell = 1;
        }

    }

}
