using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseMapMeshGenerator
{
    public static Mesh GenerateTerrainMesh(float[,] heightMap, float heightMultiplier)
    {
        var width = heightMap.GetLength(0);
        var height = heightMap.GetLength(1);

        var topLeftX = (width - 1) / -2f;
        var topLeftZ = (height - 1) / 2f;

        var meshData = new NoiseMapMeshData(width, height);
        var vertexIndex = 0;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                meshData.Vertices[vertexIndex] = new Vector3(topLeftX + x, heightMap[x, y] * heightMultiplier, topLeftZ - y);
                meshData.UVs[vertexIndex] = new Vector2(x / (float)width, y / (float)height);

                if (x < width - 1 && y < height - 1)
                {
                    meshData.AddTriangle(vertexIndex, vertexIndex + width + 1, vertexIndex + width);
                    meshData.AddTriangle(vertexIndex + width + 1, vertexIndex, vertexIndex + 1);
                }

                vertexIndex++;
            }
        }

        return meshData.CreateMesh();
    }
}

public class NoiseMapMeshData
{
    public Vector3[] Vertices;
    public int[] Triangles;
    public Vector2[] UVs;

    private int _currentTriangleIndex;

    public NoiseMapMeshData(int meshWidth, int meshHeight)
    {
        Vertices = new Vector3[meshWidth * meshHeight];
        UVs = new Vector2[meshWidth * meshHeight];
        Triangles = new int[(meshWidth - 1) * (meshHeight - 1) * 6];
    }

    public void AddTriangle(int a, int b, int c)
    {
        Triangles[_currentTriangleIndex] = a;
        Triangles[_currentTriangleIndex + 1] = b;
        Triangles[_currentTriangleIndex + 2] = c;

        _currentTriangleIndex += 3;
    }

    public Mesh CreateMesh()
    {
        var mesh = new Mesh();

        mesh.vertices = Vertices;
        mesh.triangles = Triangles;
        mesh.uv = UVs;

        mesh.RecalculateNormals();

        return mesh;
    }
}
