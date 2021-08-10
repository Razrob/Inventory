using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

[CustomPropertyDrawer(typeof(CraftItemSet))]
public class CraftRecipeEditor : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
        EditorGUI.indentLevel = 0;

        Rect _receivedItemRect = new Rect(position.x, position.y, 40, 18);

        EditorGUI.PropertyField(_receivedItemRect, property.FindPropertyRelative("ItemNumber"), GUIContent.none);

        Item _item = property.FindPropertyRelative("Item").objectReferenceValue as Item;
        if (_item != null)
        {
            Material _material = new Material(Shader.Find("UI/Unlit/Transparent"));
            _material.mainTexture = _item.ItemSprite.texture;
            EditorGUI.DrawPreviewTexture(new Rect(_receivedItemRect.x + 45, _receivedItemRect.y, 18, _receivedItemRect.height), _item.ItemSprite.texture, _material);
        }

        EditorGUI.PropertyField(new Rect(_receivedItemRect.x + 70, _receivedItemRect.y, EditorGUIUtility.currentViewWidth - (_receivedItemRect.x + 90), 18), property.FindPropertyRelative("Item"), GUIContent.none);


        EditorGUI.EndProperty();
    }

}
