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

        Rect _receivedItemRect = new Rect(position.x, position.y, 40, position.height);

        EditorGUI.PropertyField(_receivedItemRect, property.FindPropertyRelative("ItemNumber"), GUIContent.none);
        Item _item = property.FindPropertyRelative("Item").objectReferenceValue as Item;
        if (_item != null)
        {
            Material _material = new Material(Shader.Find("Standard"));
            _material.mainTexture = _item.ItemSprite.texture;
            _material.color = Color.white;


            EditorGUI.DrawPreviewTexture(new Rect(_receivedItemRect.x + 45, _receivedItemRect.y, 20, _receivedItemRect.height), _item.ItemSprite.texture);
        }

        // EditorGUI.DrawTextureAlpha(new Rect(_receivedItemRect.x + 45, _receivedItemRect.y, 23, _receivedItemRect.height), _item.ItemSprite.texture);

        EditorGUI.PropertyField(new Rect(_receivedItemRect.x + 75, _receivedItemRect.y, EditorGUIUtility.currentViewWidth - (_receivedItemRect.x + 60), _receivedItemRect.height), property.FindPropertyRelative("Item"), GUIContent.none);
       // Debug.Log();

        EditorGUI.EndProperty();
    }

}
