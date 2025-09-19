using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlanetNameGenerator))]

public class PlanetNameGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        PlanetNameGenerator planetNameGenerator = (PlanetNameGenerator)target;

        DrawDefaultInspector();
        if (GUILayout.Button("Generate planet name"))
        {
            planetNameGenerator.generatePlanetName();
        }
    }
}
