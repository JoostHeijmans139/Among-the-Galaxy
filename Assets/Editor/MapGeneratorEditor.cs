// csharp
using System.IO;
using Assets.Scripts.TerrainGeneration;
using UnityEditor;
using UnityEngine;

/// <summary>
/// This file defines a custom Unity Editor inspector for the MapGenerator component.
/// It overrides the default inspector UI to automatically regenerate
/// the map in the editor whenever properties are changed and the autoUpdate flag is enabled.
/// This helps developers preview map changes in real-time without manually triggering updates.
/// </summary>
[CustomEditor(typeof(MapGenerator))]
public class MapGeneratorEditor : Editor
{
    public MapGenerator.SerializableAnimationCurve TerrainCurve;
    public MapGenerator.TerrainType[] TerrainTypes;

    public override void OnInspectorGUI()
    {
        MapGenerator mapGenerator = (MapGenerator)target;
        EditorGUI.BeginChangeCheck();
        DrawDefaultInspector();
        if (EditorGUI.EndChangeCheck())
        {
            if (mapGenerator.autoUpdate)
            {
                mapGenerator.DrawMapInEditor();
            }
        }

        #region Loading Terrain Types from JSON

        if (GUILayout.Button("Load Preset terrain types"))
        {
            string path = EditorUtility.OpenFilePanel("Select terrain types", Application.dataPath, "json");
            if (!string.IsNullOrEmpty(path))
            {
                string terrainTypesJson = File.ReadAllText(path);
                var terrainTypesArray = JsonUtility.FromJson<TerrainTypeArray>(terrainTypesJson);
                if (terrainTypesArray != null)
                {
                    foreach (var terrainType in terrainTypesArray.terrainTypes)
                    {
                        Debug.Log(terrainType.Name + " " + terrainType.Height + " " + terrainType.Color);
                    }
                    mapGenerator.TerrainTypes = terrainTypesArray.terrainTypes;
                    EditorUtility.SetDirty(mapGenerator);
                    serializedObject.ApplyModifiedProperties();
                }
                else
                {
                    Debug.Log("No terrain types found in json.");
                }
            }
            else
            {
                Debug.LogWarning("No terrain types found.");
                return;
            }
        }

        #endregion

        #region Exporting Terrain Types to JSON
        if (GUILayout.Button("Export terrain types to json"))
        {
            TerrainTypes = mapGenerator.TerrainTypes;
            if (TerrainTypes == null || TerrainTypes.Length == 0)
            {
                Debug.LogWarning("No terrain types selected.");
                return;
            }
            foreach (MapGenerator.TerrainType terrainType in TerrainTypes)
            {
                Debug.Log(terrainType.Name);
            }

            string json = JsonUtility.ToJson(new TerrainTypeArray { terrainTypes = TerrainTypes }, true);
            Debug.Log(json);
            string path = EditorUtility.SaveFilePanelInProject("Export terrain types", "terrainTypes", "json", "choose location to save terrain types");
            if (!string.IsNullOrEmpty(path))
            {
                File.WriteAllText(path, json);
                AssetDatabase.Refresh();
            }
        }
        #endregion

        #region Loading Animation Curve from JSON

        if (GUILayout.Button("Load Animation Curve from JSON"))
        {
            string path = EditorUtility.OpenFilePanel("Select Animation Curve", Application.dataPath, "json");
            if (!string.IsNullOrEmpty(path))
            {
                string curveJson = File.ReadAllText(path);
                AnimationCurveWrapper wrapper = JsonUtility.FromJson<AnimationCurveWrapper>(curveJson);
                if (wrapper != null && wrapper.curve != null)
                {
                    mapGenerator.heightCurve = wrapper.curve.ToAnimationCurve();
                    EditorUtility.SetDirty(mapGenerator);
                    serializedObject.ApplyModifiedProperties();
                }
                else
                {
                    Debug.Log("No Animation Curve found in json.");
                }
            }
            else
            {
                Debug.Log("File path is empty.");
            }
        }
        #endregion

        #region Exporting Animation Curve to JSON

        if (GUILayout.Button("Export Animation Curve to JSON"))
        {
            if (mapGenerator.heightCurve == null || mapGenerator.heightCurve.keys.Length == 0)
            {
                Debug.LogWarning("No Animation Curve found to export.");
                return;
            }

            TerrainCurve = new MapGenerator.SerializableAnimationCurve(mapGenerator.heightCurve);
            Debug.Log("Exporting Animation Curve with " + TerrainCurve.keys.Length + " keyframes.");
            string json = JsonUtility.ToJson(new AnimationCurveWrapper() { curve = TerrainCurve }, true);
            foreach (var keyframe in TerrainCurve.keys)
            {
                Debug.Log("Keyframe time" + keyframe.time);
            }
            Debug.Log(json);
            string path = EditorUtility.SaveFilePanelInProject("Export Animation Curve", "heightCurve", "json", "choose location to save Animation Curve");
            if (!string.IsNullOrEmpty(path))
            {
                File.WriteAllText(path, json);
                AssetDatabase.Refresh();
            }
        }

        #endregion
    }
}
[System.Serializable]
public class TerrainTypeArray
{
    public MapGenerator.TerrainType[] terrainTypes;
}

[System.Serializable]
public class AnimationCurveWrapper
{
    public MapGenerator.SerializableAnimationCurve curve;
}