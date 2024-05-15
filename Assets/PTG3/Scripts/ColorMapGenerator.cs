using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ColorMapHeightColorData
{
    public Color HeightColor;
    public float HeightValue;
}

public class ColorMapGenerator
{
    public static Color[] GenerateColorMapFromWorldHeightMap(Dictionary<Vector2Int, float[,]> worldDictionary, bool blackAndWhite, List<ColorMapHeightColorData> heightColorData)
    {
        var chunkCount = worldDictionary.Keys.Count;

        if (chunkCount <= 0)
        {
            Debug.LogWarning("Chunk count is 0!");
            return new Color[0];
        }

        if (!worldDictionary.ContainsKey(Vector2Int.zero))
        {
            Debug.LogWarning("World array doesn't contain Vector2.zero key!");
            return new Color[0];
        }

        var chunkWidth = worldDictionary[Vector2Int.zero].GetLength(0);
        var chunkHeight = worldDictionary[Vector2Int.zero].GetLength(1);

        var worldChunkWidth = 0;
        var worldChunkHeight = 0;

        foreach (var worldCoordinate in worldDictionary.Keys)
        {
            if (worldCoordinate.x + 1 > worldChunkWidth)
            {
                worldChunkWidth = worldCoordinate.x + 1;
            }
            if (worldCoordinate.y + 1 > worldChunkHeight)
            {
                worldChunkHeight = worldCoordinate.y + 1;
            }
        }

        var worldWidth = chunkWidth * worldChunkWidth;
        var worldHeight = chunkHeight * worldChunkHeight;

        var colorMap = new Color[worldWidth * worldHeight];

        for (int xWorld = 0; xWorld < worldChunkWidth; xWorld++)
        {
            for (int yWorld = 0; yWorld < worldChunkHeight; yWorld++)
            {
                var worldCoordinate = new Vector2Int(xWorld, yWorld);

                var worldChunkHeightMap = worldDictionary[worldCoordinate];
                for (int xChunk = 0; xChunk < chunkWidth; xChunk++)
                {
                    for (int yChunk = 0; yChunk < chunkHeight; yChunk++)
                    {
                        var heightMapSample = worldChunkHeightMap[xChunk, yChunk];
                        var heightMapSampleColor = Color.black;
                        if (blackAndWhite)
                        {
                            heightMapSampleColor = Color.Lerp(Color.black, Color.white, heightMapSample);
                        }
                        else
                        {
                            for (int i = 0; i < heightColorData.Count; i++)
                            {
                                var colorData = heightColorData[i];
                                if (heightMapSample >= colorData.HeightValue)
                                {
                                    heightMapSampleColor = colorData.HeightColor;
                                }
                            }
                        }

                        var currentSampledWorldXCoordinate = (xWorld * chunkWidth) + xChunk;
                        var currentSampledWorldYCoordinate = ((yWorld * chunkHeight) + yChunk) * worldWidth;

                        colorMap[currentSampledWorldYCoordinate + currentSampledWorldXCoordinate] = heightMapSampleColor;
                    }
                }
            }
        }

        return colorMap;
    }
}
