using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeightMapHydraulicErosionGenerator
{
    public static Dictionary<Vector2Int, float[,]> SimulateHydraulicErosionForWorldHeightMaps(Dictionary<Vector2Int, float[,]> worldDictionary)
    {
        float[] worldHeightMap = SplitWorldHeightMaps(worldDictionary);
        float[] erodedHeightMap = HydraulicErosionSimulation(worldHeightMap);
        return MergeWorldHeightMaps(erodedHeightMap, worldDictionary);
    }

    public static float[] SplitWorldHeightMaps(Dictionary<Vector2Int, float[,]> worldDictionary)
    {
        int chunkSize = worldDictionary[Vector2Int.zero].GetLength(0);
        int totalChunks = worldDictionary.Count;
        int totalSize = chunkSize * chunkSize * totalChunks;

        float[] worldHeightMap = new float[totalSize];
        int index = 0;

        foreach (var chunk in worldDictionary.Values)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                for (int x = 0; x < chunkSize; x++)
                {
                    worldHeightMap[index++] = chunk[x, y];
                }
            }
        }

        return worldHeightMap;
    }

    public static float[] HydraulicErosionSimulation(float[] worldHeightMap)
    {
        // Placeholder for actual hydraulic erosion simulation
        // For now, we will return the same height map without modification
        return worldHeightMap;
    }

    public static Dictionary<Vector2Int, float[,]> MergeWorldHeightMaps(float[] worldHeightMap, Dictionary<Vector2Int, float[,]> originalDictionary)
    {
        int chunkSize = originalDictionary[Vector2Int.zero].GetLength(0);
        int totalChunks = originalDictionary.Count;
        int totalSize = chunkSize * chunkSize * totalChunks;

        Dictionary<Vector2Int, float[,]> resultDictionary = new Dictionary<Vector2Int, float[,]>();
        int index = 0;

        foreach (var key in originalDictionary.Keys)
        {
            float[,] chunk = new float[chunkSize, chunkSize];
            for (int y = 0; y < chunkSize; y++)
            {
                for (int x = 0; x < chunkSize; x++)
                {
                    chunk[x, y] = worldHeightMap[index++];
                }
            }
            resultDictionary[key] = chunk;
        }

        return resultDictionary;
    }
}
