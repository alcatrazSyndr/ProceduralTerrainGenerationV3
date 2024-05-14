using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureGenerator
{
    public static Texture2D GenerateTextureFromColorMap(Color[] colorMap, int width, int height)
    {
        var texture = new Texture2D(width, height);

        texture.SetPixels(colorMap);
        texture.Apply();

        return texture;
    }
}
