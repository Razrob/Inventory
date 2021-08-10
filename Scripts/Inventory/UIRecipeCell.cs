using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIRecipeCell : MonoBehaviour
{
    public TextMeshProUGUI ItemNumberUI;
    public Image ItemImage;
    public GameObject CraftAvailabilityPanel;
    [HideInInspector] public CraftRecipe CraftRecipe;
}
