using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public enum NoiseMapRenderType
{
    Plane,
    Mesh
}

[CustomEditor(typeof(NoiseMapRendererController))]
public class NoiseMapRendererControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        NoiseMapRendererController myScript = (NoiseMapRendererController)target;
        if (GUILayout.Button("Generate"))
        {
            myScript.Generate();
        }
    }
}

public class NoiseMapRendererController : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private int _chunkSize = 255;
    [SerializeField] private int _worldSize = 1;
    [SerializeField] private float _worldScale = 4f;
    [SerializeField] private bool _falloff = true;
    [Range(0f, 1f)]
    [SerializeField] private float _mainlandSize = 1f;
    [Range(1f, 80f)]
    [SerializeField] private float _falloffTransitionWidth = 5.6f;
    [SerializeField] private AnimationCurve _heightMapHeightCurve;
    [SerializeField] private List<ColorMapHeightColorData> _heightColorData = new List<ColorMapHeightColorData>();
    [SerializeField] private bool _blackAndWhite = false;
    [Range(1,4)]
    [SerializeField] private int _octaves = 2;
    [Range(0f, 1f)]
    [SerializeField] private float _persistence = 0.17f;
    [Range(0f, 5f)]
    [SerializeField] private float _lacunarity = 4.3f;
    [SerializeField] private float _heightMultiplier = 1f;
    [SerializeField] private NoiseMapRenderType _renderType = NoiseMapRenderType.Plane;
    [SerializeField] private bool _hydraulicErosion = false;
    [SerializeField] private int _hydraulicErosionIterations = 30000;

    [Header("Component References")]
    [SerializeField] private Renderer _renderer;
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private MeshFilter _meshFilter;

    private void SetMeshRendererTexture()
    {
        var worldData = NoiseMapGenerator.GeneratePerlinNoiseWorldHeightMap(_worldSize, _chunkSize, _worldScale, _falloff, _mainlandSize, _falloffTransitionWidth, _heightMapHeightCurve, _octaves, _persistence, _lacunarity, _heightMultiplier);

        if (_hydraulicErosion)
        {
            worldData = HeightMapHydraulicErosionGenerator.SimulateHydraulicErosionForWorldHeightMaps(worldData, _hydraulicErosionIterations);
        }

        var colorMap = ColorMapGenerator.GenerateColorMapFromWorldHeightMap(worldData, _blackAndWhite, _heightColorData);
        var texture = TextureGenerator.GenerateTextureFromColorMap(colorMap, _worldSize * _chunkSize, _worldSize * _chunkSize);

        _renderer.sharedMaterial.mainTexture = texture;
        _renderer.transform.localScale = new Vector3(-_worldSize * _chunkSize, 1f, _worldSize * _chunkSize) / 10f;
    }

    private void SetSingleMeshRendererTexture()
    {
        var worldData = NoiseMapGenerator.GeneratePerlinNoiseWorldHeightMap(_worldSize, _chunkSize, _worldScale, _falloff, _mainlandSize, _falloffTransitionWidth, _heightMapHeightCurve, _octaves, _persistence, _lacunarity, _heightMultiplier);

        if (_hydraulicErosion)
        {
            worldData = HeightMapHydraulicErosionGenerator.SimulateHydraulicErosionForWorldHeightMaps(worldData, _hydraulicErosionIterations);
        }

        var colorMap = ColorMapGenerator.GenerateColorMapFromWorldHeightMap(worldData, _blackAndWhite, _heightColorData);
        var texture = TextureGenerator.GenerateTextureFromColorMap(colorMap, _worldSize * _chunkSize, _worldSize * _chunkSize);

        var mesh = NoiseMapMeshGenerator.GenerateTerrainMesh(worldData[Vector2Int.zero]);
        mesh.RecalculateNormals();

        _meshFilter.mesh = mesh;
        _meshRenderer.sharedMaterial.mainTexture = texture;
    }

    public void Generate()
    {
        _renderer.gameObject.SetActive(false);
        _meshRenderer.gameObject.SetActive(false);

        if (_renderType == NoiseMapRenderType.Mesh && _worldSize == 1 && _worldSize == 1)
        {
            _meshRenderer.gameObject.SetActive(true);
            SetSingleMeshRendererTexture();
        }
        else
        {
            _renderer.gameObject.SetActive(true);
            SetMeshRendererTexture();
        }
    }
}
