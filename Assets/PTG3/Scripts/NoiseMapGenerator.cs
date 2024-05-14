using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseMapGenerator
{
    public static Dictionary<Vector2Int, float[,]> GeneratePerlinNoiseWorldHeightMap(int worldChunkWidth, int worldChunkHeight, int chunkWidth, int chunkHeight, float worldScale)
    {
        var worldDictionary = new Dictionary<Vector2Int, float[,]>();

        var worldWidth = worldChunkWidth * chunkWidth;
        var worldHeight = worldChunkHeight * chunkHeight;

        var widthScale = 1f / (float)worldChunkWidth;
        var heightScale = 1f / (float)worldChunkHeight;

        for (int xWorld = 0; xWorld < worldChunkWidth; xWorld++)
        {
            for (int yWorld = 0; yWorld < worldChunkHeight; yWorld++)
            {
                var worldCoordinate = new Vector2Int(xWorld, yWorld);

                var xWorldSamplePosition = ((float)xWorld * (float)chunkWidth) / (float)worldWidth;
                var yWorldSamplePosition = ((float)yWorld * (float)chunkHeight) / (float)worldHeight;

                var chunkHeightMap = new float[chunkWidth, chunkHeight];
                for (int xChunk = 0; xChunk < chunkWidth; xChunk++)
                {
                    for (int yChunk = 0; yChunk < chunkHeight; yChunk++)
                    {
                        var xChunkSamplePosition = ((xWorldSamplePosition + ((float)xChunk / (float)worldWidth)) / widthScale) * worldScale;
                        var yChunkSamplePosition = ((yWorldSamplePosition + ((float)yChunk / (float)worldHeight)) / heightScale) * worldScale;

                        var sample = Mathf.PerlinNoise(xChunkSamplePosition, yChunkSamplePosition);

                        chunkHeightMap[xChunk, yChunk] = Mathf.Clamp01(sample);
                    }
                }

                worldDictionary.Add(worldCoordinate, chunkHeightMap);
            }
        }

        return worldDictionary;
    }
}
