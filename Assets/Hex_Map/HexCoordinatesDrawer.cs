using UnityEngine;
using UnityEditor;


//用于编辑器的脚本
[CustomPropertyDrawer(typeof(HexCoordinates))]
public class HexCoordinatesDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        HexCoordinates coordinates = new HexCoordinates(
            property.FindPropertyRelative("x").intValue,
            property.FindPropertyRelative("z").intValue
            );

        //绘制属性名，序列化数据的类？
        position = EditorGUI.PrefixLabel(position,label);
        GUI.Label(position, coordinates.ToString());
    }
}
