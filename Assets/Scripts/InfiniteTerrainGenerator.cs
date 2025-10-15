using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InfiniteTerrainGenerator : MonoBehaviour
{
    const float viewerMoveThresholdForChunkUpdate = 25f;
    const float sqrViewerMoveThresholdForChunkUpdate = viewerMoveThresholdForChunkUpdate * viewerMoveThresholdForChunkUpdate;
    #region Variables
    public LODInfo[] LevelOfDetailLevels;

    /// <summary> Maximum distance (in world units) at which chunks remain visible. </summary>
    public static float ViewDistance;
    
    /// <summary> Material applied to the terrain mesh. </summary>
    public Material meshMaterial;
    /// <summary> Parent object to which all generated terrain chunks are attached. </summary>
    public GameObject parent;

    /// <summary> Transform reference representing the player or camera that determines which chunks are visible. </summary>
    public Transform viewer;

    /// <summary> The viewer’s current position projected onto the XZ-plane. </summary>
    public static Vector2 ViewerPosition;
    private Vector2 _viewerPositionOld;

    /// <summary> The size (in world units) of each terrain chunk. </summary>
    private int _chunkSize;

    /// <summary> Number of chunks that can fit within the view distance. </summary>
    private int _chunksVisibleInViewDistance;

    /// <summary> Dictionary mapping chunk coordinates to their corresponding TerrainChunk instances. </summary>
    private readonly Dictionary<Vector2, TerrainChunk> _terrainChunkDictionary = new();

    /// <summary> List of chunks that were visible during the previous frame for efficient visibility updates. </summary>
    private readonly List<TerrainChunk> _terrainChunksLastFrame = new();

    private static MapGenerator _mapGenerator;

    #endregion

    #region start and update methods

    /// <summary>
    /// Initializes the chunk size and determines how many chunks are visible based on the view distance.
    /// </summary>
    private void Start()
    {
        _mapGenerator = FindAnyObjectByType<MapGenerator>();
        if (_mapGenerator == null)
        {
            throw new NullReferenceException("MapGenerator is null");
        }
        ViewDistance = LevelOfDetailLevels[LevelOfDetailLevels.Length - 1].VisibleDistanceThreshold;
        _chunkSize = MapGenerator.MapChunkSize - 1;
        _chunksVisibleInViewDistance = Mathf.RoundToInt(ViewDistance / _chunkSize);
        UpdateVisibleChunks();
    }

    /// <summary>
    /// Updates the viewer position every frame and recalculates which chunks should be visible.
    /// </summary>
    private void Update()
    {
        ViewerPosition = new Vector2(viewer.position.x, viewer.position.z);
        if ((ViewerPosition - _viewerPositionOld).sqrMagnitude > sqrViewerMoveThresholdForChunkUpdate)
        {
            _viewerPositionOld = ViewerPosition;
            UpdateVisibleChunks();
        }
    }

    #endregion

    #region UpdateVisibleChunks

        /// <summary>
    /// Determines which terrain chunks should be visible or hidden based on the viewer’s position.
    /// Dynamically creates new chunks when needed and hides those that fall outside the view distance.
    /// </summary>
    private void UpdateVisibleChunks()
    {
        // Hide all chunks that were visible in the previous frame.
        foreach (TerrainChunk terrainChunk in _terrainChunksLastFrame)
        {
            terrainChunk.SetVisible(false);
        }
        _terrainChunksLastFrame.Clear();

        // Determine the viewer’s current chunk coordinates.
        int currentChunkCoordX = Mathf.RoundToInt(ViewerPosition.x / _chunkSize);
        int currentChunkCoordY = Mathf.RoundToInt(ViewerPosition.y / _chunkSize);

        // Loop through all potential visible chunks around the viewer.
        for (int yOffset = -_chunksVisibleInViewDistance; yOffset <= _chunksVisibleInViewDistance; yOffset++)
        {
            for (int xOffset = -_chunksVisibleInViewDistance; xOffset <= _chunksVisibleInViewDistance; xOffset++)
            {
                Vector2 viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);

                if (_terrainChunkDictionary.ContainsKey(viewedChunkCoord))
                {
                    // Update existing chunk visibility.
                    _terrainChunkDictionary[viewedChunkCoord].UpdateTerrainChunk();

                    // If chunk is visible, add it to the list for this frame.
                    if (_terrainChunkDictionary[viewedChunkCoord].IsVisable())
                    {
                        _terrainChunksLastFrame.Add(_terrainChunkDictionary[viewedChunkCoord]);
                    }
                }
                else
                {
                    // Create and register a new chunk if it doesn’t exist.
                    TerrainChunk newChunk = new TerrainChunk(viewedChunkCoord, _chunkSize,LevelOfDetailLevels, parent,meshMaterial);
                    _terrainChunkDictionary.Add(viewedChunkCoord, newChunk);
                }
            }
        }
    }
        

    #endregion

    #region TerrainChunkClass

    private class TerrainChunk
    {
        readonly GameObject _meshObject;
        Vector2 position;
        Bounds _bounds;
        MeshRenderer _meshRenderer;
        MeshFilter _meshFilter;
        MapGenerator.MapData _mapData;
        LODInfo[] _levelsOfDetail;
        LODMesh[] _lodMeshes;
        bool mapDataReceived;
        int previousLODIndex = -1;
        public TerrainChunk(Vector2 coord,int chunkSize,LODInfo[] detailLevels,GameObject parent,Material meshMaterial)
        {
            _levelsOfDetail = detailLevels;
            position = coord*chunkSize;
            _bounds = new Bounds(position, Vector2.one * chunkSize);
            Vector3 position3D = new Vector3(position.x, 0, position.y);
            
            _meshObject = new GameObject();
            _meshObject.transform.position = position3D;
            _meshObject.transform.parent = parent.transform;
            _meshFilter = _meshObject.AddComponent<MeshFilter>();
            _meshRenderer = _meshObject.AddComponent<MeshRenderer>();
            _meshRenderer.material = meshMaterial;
            _meshObject.name = $"Terrain Chunk {coord.x},{coord.y}";
            SetVisible(false);
            _lodMeshes = new LODMesh[_levelsOfDetail.Length];
            for (int i = 0; i < _levelsOfDetail.Length; i++)
            {
                _lodMeshes[i] = new LODMesh(_levelsOfDetail[i].LevelOfDetail, UpdateTerrainChunk);
            }
            _mapGenerator.RequestMapData(position,OnMapDataReceived);
            
        }
        void OnMapDataReceived(MapGenerator.MapData mapData)
        {
            _mapData = mapData;
            if (MapGenerator.CurrentHeightMap == null)
            {
                MapGenerator.CurrentHeightMap = mapData.HeightMap;
            }
            _meshRenderer.material.mainTexture = TextureGenerator.TextureFromColourMap(MapGenerator.GenerateColorMap(MapGenerator.CurrentHeightMap,_mapGenerator.TerrainTypes), MapGenerator.MapChunkSize, MapGenerator.MapChunkSize);
            mapDataReceived = true;
            UpdateTerrainChunk();
        }
        public void UpdateTerrainChunk()
        {
            if (!mapDataReceived)
            {
                return;   
            }
            float viewerDistanceFromNearestEdge = Mathf.Sqrt(_bounds.SqrDistance(InfiniteTerrainGenerator.ViewerPosition));
            bool visible = viewerDistanceFromNearestEdge <= InfiniteTerrainGenerator.ViewDistance;
            if (visible)
            {
                int lodIndex = 0;
                for (int i = 0; i< _levelsOfDetail.Length; i++)
                {
                    if(viewerDistanceFromNearestEdge > _levelsOfDetail[i].VisibleDistanceThreshold)   
                    {
                        lodIndex = i + 1;
                    }
                    else
                    {
                        break;
                    }
                }
                if(lodIndex != previousLODIndex)
                {
                    LODMesh lodMesh = _lodMeshes[lodIndex];
                    if (lodMesh.HaseMesh)
                    {
                        previousLODIndex = lodIndex;
                        _meshFilter.mesh = lodMesh.Mesh;
                    }
                    else if (!lodMesh.HaseReqestedMesh && mapDataReceived)
                    {
                        lodMesh.RequestMesh(_mapData);
                    }
                }
            }
            SetVisible(visible);
        }
        public void SetVisible(bool visible)
        {
            _meshObject.SetActive(visible);
        }

        public bool IsVisable()
        {
            return _meshObject.activeSelf;
        }
    }

    #endregion

    #region LODMesh Class
    class LODMesh
    {
        public Mesh Mesh;
        public bool HaseReqestedMesh;
        public bool HaseMesh;
        private int _levelOfDetail;
        private System.Action updateCallback;
        public LODMesh(int levelOfDetail, System.Action updateCallback)
        {
            _levelOfDetail = levelOfDetail;
            this.updateCallback = updateCallback;
        }
        private void OnMeshDataReceived(MeshData meshData)
        {
            Mesh = meshData.CreateMesh();
            HaseMesh = true;
            updateCallback();
        }
        public void RequestMesh(MapGenerator.MapData mapData)
        {
            HaseReqestedMesh = true;
            _mapGenerator.RequestMeshData(mapData,_levelOfDetail, OnMeshDataReceived);
        }
    }

    #endregion
    #region LODInfo struct
    [System.Serializable]
    public struct LODInfo
    {
        public int LevelOfDetail;
        public float VisibleDistanceThreshold;

        public LODInfo(int levelOfDetail, float visibleDistanceThreshold)
        {
            LevelOfDetail = levelOfDetail;
            VisibleDistanceThreshold = visibleDistanceThreshold;
        }
    }

    #endregion
}
