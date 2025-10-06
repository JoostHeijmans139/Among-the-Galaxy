using System.IO;
using System.Text.Json;
using Unity.Plastic.Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

/// <summary>
/// This file defines a custom Unity Editor inspector for the MapGenerator component.
/// It overrides the default inspector UI to automatically regenerate
/// the map in the editor whenever properties are changed and the autoUpdate flag is enabled.
/// This helps developers preview map changes in real-time without manually triggering updates.
/// </summary>
[CustomEditor(typeof(MapGenerator))]
public class MapGeneratorEditor: Editor
{
    public MapGenerator.TerrainType[] TerrainTypes;
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

        if (GUILayout.Button("Load Preset terrain types"))
        {
            string path = EditorUtility.OpenFilePanel("Select terrain types", Application.dataPath, "json");
            if (!string.IsNullOrEmpty(path))
            {
                string terrainTypesJson = File.ReadAllText(path);
                var TerrainTypesArray = JsonUtility.FromJson<TerrainTypeArray>(terrainTypesJson);
                if (TerrainTypes != null)
                {
                    mapGenerator.TerrainTypes = TerrainTypesArray.terrainTypes;
                    EditorUtility.SetDirty(mapGenerator);
                    serializedObject.ApplyModifiedProperties();
                }
            }
            else
            {
                Debug.LogWarning("No terrain types found.");
                return;
            }
        }

        if (GUILayout.Button("Convert terrain types to json"))
        { 
            TerrainTypes = mapGenerator.TerrainTypes;
            if (TerrainTypes == null || TerrainTypes.Length == 0)
            {
                Debug.LogWarning("No terrain types selected.");
                return;
            }
            foreach (MapGenerator.TerrainType terrainType in TerrainTypes)
            {
                Debug.Log(terrainType.name);
            }

            string json = JsonUtility.ToJson(new TerrainTypeArray{terrainTypes = TerrainTypes},true);
            Debug.Log(json);
            string path =  EditorUtility.SaveFilePanelInProject("Export terrain types","terrainTypes", "json", "choose location to save terrain types");
            if (!string.IsNullOrEmpty(path))
            {
                File.WriteAllText(path, json);
                AssetDatabase.Refresh();
            }
        }
    }
}
[System.Serializable]
public class TerrainTypeArray
{
    public MapGenerator.TerrainType[] terrainTypes;
}