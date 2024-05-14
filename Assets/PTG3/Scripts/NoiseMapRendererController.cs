using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseMapRendererController : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private int _chunkWidth = 255;
    [SerializeField] private int _chunkHeight = 255;
    [SerializeField] private int _worldWidth = 1;
    [SerializeField] private int _worldHeight = 1;
    [SerializeField] private float _worldScale = 1f;

    [Header("Component References")]
    [SerializeField] private Renderer _renderer;

    private void SetMeshRendererTexture()
    {
        var worldData = NoiseMapGenerator.GeneratePerlinNoiseWorldHeightMap(_worldWidth, _worldHeight, _chunkWidth, _chunkHeight, _worldScale);
        var colorMap = ColorMapGenerator.GenerateBlackWhiteColorMapFromWorldHeightMap(worldData);
        var texture = TextureGenerator.GenerateTextureFromColorMap(colorMap, _worldWidth * _chunkWidth, _worldHeight * _chunkHeight);

        _renderer.sharedMaterial.mainTexture = texture;
        _renderer.transform.localScale = new Vector3(_worldWidth * _chunkWidth, 1f, _worldHeight * _chunkHeight);
    }

    private void OnValidate()
    {
        SetMeshRendererTexture();
    }
}
