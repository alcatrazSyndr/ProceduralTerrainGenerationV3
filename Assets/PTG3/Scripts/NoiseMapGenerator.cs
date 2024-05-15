using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseMapGenerator
{
    public static Dictionary<Vector2Int, float[,]> GeneratePerlinNoiseWorldHeightMap(
        int worldChunkSize, 
        int chunkSize, 
        float worldScale, 
        bool falloff, 
        float mainlandSize,
        float falloffTransitionWidth,
        AnimationCurve heightMapHeightCurve,
        int octaves,
        float persistence,
        float lacunarity,
        float heightMultiplier)
    {
        var worldDictionary = new Dictionary<Vector2Int, float[,]>();

        chunkSize += 1;

        var worldSize = worldChunkSize * chunkSize;

        var widthScale = 1f / (float)worldChunkSize;
        var heightScale = 1f / (float)worldChunkSize;

        for (int xWorld = 0; xWorld < worldChunkSize; xWorld++)
        {
            for (int yWorld = 0; yWorld < worldChunkSize; yWorld++)
            {
                var worldCoordinate = new Vector2Int(xWorld, yWorld);

                var xWorldSamplePosition = ((float)xWorld * (float)chunkSize) / (float)worldSize;
                var yWorldSamplePosition = ((float)yWorld * (float)chunkSize) / (float)worldSize;

                var chunkHeightMap = new float[chunkSize, chunkSize];
                for (int yChunk = 0; yChunk < chunkSize; yChunk++)
                {
                    for (int xChunk = 0; xChunk < chunkSize; xChunk++)
                    {
                        var amplitude = 1f;
                        var frequency = 1f;

                        var noiseHeight = 0f;

                        for (int i = 0; i < octaves; i++)
                        {
                            var xChunkSamplePosition = ((xWorldSamplePosition + ((float)xChunk / (float)worldSize)) / widthScale) * worldScale * frequency;
                            var yChunkSamplePosition = ((yWorldSamplePosition + ((float)yChunk / (float)worldSize)) / heightScale) * worldScale * frequency;

                            var sample = Mathf.PerlinNoise(xChunkSamplePosition, yChunkSamplePosition);

                            noiseHeight += sample * amplitude;

                            amplitude *= persistence;
                            frequency *= lacunarity;
                        }

                        if (falloff)
                        {
                            float falloffValue = EvaluateWorldFalloffMap(xWorld * chunkSize + xChunk, yWorld * chunkSize + yChunk, worldSize, worldSize, mainlandSize, falloffTransitionWidth);
                            noiseHeight -= falloffValue;
                        }
                        chunkHeightMap[xChunk, yChunk] = heightMapHeightCurve.Evaluate(noiseHeight) * heightMultiplier;
                    }
                }

                worldDictionary.Add(worldCoordinate, chunkHeightMap);
            }
        }

        return worldDictionary;
    }

    private static float EvaluateWorldFalloffMap(int x, int y, int worldWidth, int worldHeight, float mainlandSize, float falloffTransitionWidth)
    {
        float centerX = worldWidth / 2f;
        float centerY = worldHeight / 2f;

        float distanceX = (x - centerX) / (worldWidth * mainlandSize / 2f);
        float distanceY = (y - centerY) / (worldHeight * mainlandSize / 2f);

        float distance = Mathf.Sqrt(distanceX * distanceX + distanceY * distanceY);

        float falloffValue = Mathf.Pow(distance, falloffTransitionWidth);

        return Mathf.Clamp01(falloffValue);
    }
}
