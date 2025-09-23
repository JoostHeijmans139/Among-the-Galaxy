using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public static class MeshGenerator 
{
    public static MeshData GenerateTerainMesh(float[,] heighMap)
    {
        int width = heighMap.GetLength(0);
        int height = heighMap.GetLength(1);
        float topLeftX = (width - 1)/ -2f;
        float topLeftZ = (height - 1)/ 2f;
        MeshData meshData = new MeshData(width, height);
        int vertextIndex = 0;
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                meshData.vertices[vertextIndex] = new Vector3(topLeftX+x,heighMap[x,y],topLeftZ-y);
                meshData.uvs[vertextIndex] = new Vector2(x / (float)width,y / (float)height);
                if (x < width - 1 && y < height - 1)
                {
                    meshData.addTriangle(vertextIndex, vertextIndex+width+1, vertextIndex+width);
                    meshData.addTriangle(vertextIndex+width+1, vertextIndex, vertextIndex+1);
                }
                vertextIndex++;
            }
        }

        return meshData;
    }
}

public class MeshData
{
    public Vector3[] vertices;
    public int[] triangles;
    private int triangleIndex;
    public Vector2[] uvs;
    public MeshData(int meshWidth, int meshHeight)
    {
        vertices = new Vector3[meshWidth * meshHeight];
        uvs = new Vector2[meshWidth * meshHeight];
        triangles = new int[(meshWidth - 1) * (meshHeight - 1) * 6];
    }

    public void addTriangle(int a, int b, int c)
    {
        triangles[triangleIndex] = a;
        triangles[triangleIndex + 1] = b;
        triangles[triangleIndex + 2] = c;
        triangleIndex += 3;
    }

    public Mesh CreateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateBounds();
        mesh.uv = uvs;
        mesh.RecalculateNormals();
        return mesh;
    }
}
