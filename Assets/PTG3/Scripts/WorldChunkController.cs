using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldChunkController : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private Material _chunkMaterial;
    [SerializeField] private int _chunkSize = 255;
    [SerializeField] private int _worldSize = 1;
    [SerializeField] private float _worldScale = 1f;
    [SerializeField] private bool _falloff = true;
    [Range(0f, 1f)]
    [SerializeField] private float _mainlandSize = 0.9f;
    [Range(1f, 80f)]
    [SerializeField] private float _falloffTransitionWidth = 5.6f;
    [SerializeField] private AnimationCurve _heightMapHeightCurve;
    [Range(1, 4)]
    [SerializeField] private int _octaves = 3;
    [Range(0f, 1f)]
    [SerializeField] private float _persistence = 0.234f;
    [Range(0f, 5f)]
    [SerializeField] private float _lacunarity = 4.07f;
    [SerializeField] private float _heightMultiplier = 50f;
    [SerializeField] private bool _hydraulicErosion = true;
    [SerializeField] private int _hydraulicErosionIterations = 90000;

    [Header("Runtime")]
    [SerializeField] private Dictionary<Vector2Int, float[,]> _worldChunkHeightMapDictionary = new Dictionary<Vector2Int, float[,]>();
    [SerializeField] private Dictionary<Vector2Int, WorldChunk> _worldChunkDictionary = new Dictionary<Vector2Int, WorldChunk>();

    public void Start()
    {
        GenerateWorldChunkData();
    }

    private void GenerateWorldChunkData()
    {
        _worldChunkHeightMapDictionary = NoiseMapGenerator.GeneratePerlinNoiseWorldHeightMap(_worldSize, _chunkSize, _worldScale, _falloff, _mainlandSize, _falloffTransitionWidth, _heightMapHeightCurve, _octaves, _persistence, _lacunarity, _heightMultiplier);

        if (_hydraulicErosion)
        {
            _worldChunkHeightMapDictionary = HeightMapHydraulicErosionGenerator.SimulateHydraulicErosionForWorldHeightMaps(_worldChunkHeightMapDictionary, _hydraulicErosionIterations);
        }

        for (int x = 0; x < _worldSize; x++)
        {
            for (int y = 0; y < _worldSize; y++)
            {
                var worldCoordinate = new Vector2Int(x, y);
                var worldChunkHeightMap = _worldChunkHeightMapDictionary[worldCoordinate];
                var worldChunk = new WorldChunk(worldCoordinate, worldChunkHeightMap, _chunkSize, _worldSize, _chunkMaterial);

                worldChunk.ChunkGO.transform.SetParent(transform);

                _worldChunkDictionary.Add(worldCoordinate, worldChunk);
            }
        }
    }
}

public class WorldChunk
{
    public Vector2Int ChunkCoordinate;
    public Vector3 ChunkPosition;
    public GameObject ChunkGO;
    public MeshFilter ChunkMeshFilter;
    public MeshRenderer ChunkMeshRenderer;
    public MeshCollider ChunkMeshCollider;

    public WorldChunk(Vector2Int worldCoordinate, float[,] heightMap, int chunkSize, int worldSize, Material chunkMat)
    {
        ChunkGO = new GameObject("WorldChunk_" + worldCoordinate.ToString());

        ChunkMeshFilter = ChunkGO.AddComponent<MeshFilter>();
        var mesh = NoiseMapMeshGenerator.GenerateTerrainMesh(heightMap);
        ChunkMeshFilter.mesh = mesh;

        ChunkMeshRenderer = ChunkGO.AddComponent<MeshRenderer>();
        ChunkMeshRenderer.material = chunkMat;

        ChunkMeshCollider = ChunkGO.AddComponent<MeshCollider>();

        ChunkPosition = new Vector3(worldCoordinate.x, 0f, -worldCoordinate.y) * (float)chunkSize * 2f;
        ChunkGO.transform.position = ChunkPosition / 2f;
    }
}
