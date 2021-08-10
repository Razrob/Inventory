using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(CraftItemSet))]
public class CraftRecipeDrawer : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        VisualElement container = new VisualElement();

        PropertyField _item = new PropertyField(property.FindPropertyRelative("Item"));
        PropertyField _itemNumber = new PropertyField(property.FindPropertyRelative("ItemNumber"));

        container.Add(_item);
        container.Add(_itemNumber);

        return container;
    }
}
