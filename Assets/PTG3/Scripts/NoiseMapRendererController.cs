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
    [SerializeField] private bool _falloff = false;
    [SerializeField] private float _mainlandSize = 1f;
    [SerializeField] private float _falloffTransitionWidth = 1f;
    [SerializeField] private AnimationCurve _heightMapHeightCurve;
    [SerializeField] private List<ColorMapHeightColorData> _heightColorData = new List<ColorMapHeightColorData>();

    [Header("Component References")]
    [SerializeField] private Renderer _renderer;

    private void SetMeshRendererTexture()
    {
        var worldData = NoiseMapGenerator.GeneratePerlinNoiseWorldHeightMap(_worldWidth, _worldHeight, _chunkWidth, _chunkHeight, _worldScale, _falloff, _mainlandSize, _falloffTransitionWidth, _heightMapHeightCurve);
        var colorMap = ColorMapGenerator.GenerateColorMapFromWorldHeightMap(worldData, _heightColorData);
        var texture = TextureGenerator.GenerateTextureFromColorMap(colorMap, _worldWidth * _chunkWidth, _worldHeight * _chunkHeight);

        _renderer.sharedMaterial.mainTexture = texture;
        _renderer.transform.localScale = new Vector3(_worldWidth * _chunkWidth, 1f, _worldHeight * _chunkHeight);
    }

    private void OnValidate()
    {
        SetMeshRendererTexture();
    }
}
