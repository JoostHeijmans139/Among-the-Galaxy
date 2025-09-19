using UnityEditor;

/// <summary>
/// This file defines a custom Unity Editor inspector for the MapGenerator component.
/// It overrides the default inspector UI to automatically regenerate
/// the map in the editor whenever properties are changed and the autoUpdate flag is enabled.
/// This helps developers preview map changes in real-time without manually triggering updates.
/// </summary>
[CustomEditor(typeof(MapGenerator))]
public class MapGeneratorEditor: Editor
{
    public override void OnInspectorGUI()
    {
        MapGenerator mapGenerator = (MapGenerator)target;
        EditorGUI.BeginChangeCheck();
        DrawDefaultInspector();
        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
            if (mapGenerator.autoUpdate)
            {
                mapGenerator.GenerateMap();
            }
            return;
        }
    }
}
