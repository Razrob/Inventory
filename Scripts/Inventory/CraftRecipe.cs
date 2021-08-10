using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class CraftRecipe 
{
    [SerializeField] private CraftItemSet _receivedItem;
    [SerializeField] private CraftItemSet[] _requiredItems;

    public CraftItemSet ReceivedItem => _receivedItem;
    public CraftItemSet[] RequiredItems => _requiredItems;
}

[Serializable]
public class CraftItemSet
{
    public Item Item;
    public int ItemNumber;
}