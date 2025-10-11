using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfiniteTerrainGenerator : MonoBehaviour
{
    /// <summary> Maximum distance (in world units) at which chunks remain visible. </summary>
    public const float ViewDistance = 450f;

    /// <summary> Parent object to which all generated terrain chunks are attached. </summary>
    public GameObject parent;

    /// <summary> Transform reference representing the player or camera that determines which chunks are visible. </summary>
    public Transform viewer;

    /// <summary> The viewer’s current position projected onto the XZ-plane. </summary>
    public static Vector2 ViewerPosition;

    /// <summary> The size (in world units) of each terrain chunk. </summary>
    private int _chunkSize;

    /// <summary> Number of chunks that can fit within the view distance. </summary>
    private int _chunksVisibleInViewDistance;

    /// <summary> Dictionary mapping chunk coordinates to their corresponding TerrainChunk instances. </summary>
    private readonly Dictionary<Vector2, TerrainChunk> _terrainChunkDictionary = new();

    /// <summary> List of chunks that were visible during the previous frame for efficient visibility updates. </summary>
    private readonly List<TerrainChunk> _terrainChunksLastFrame = new();

    /// <summary>
    /// Initializes the chunk size and determines how many chunks are visible based on the view distance.
    /// </summary>
    private void Start()
    {
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
        UpdateVisibleChunks();
    }

    /// <summary>
    /// Determines which terrain chunks should be visible or hidden based on the viewer’s position.
    /// Dynamically creates new chunks when needed and hides those that fall outside the view distance.
    /// </summary>
    void UpdateVisibleChunks()
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
                    TerrainChunk newChunk = new TerrainChunk(viewedChunkCoord, _chunkSize, parent);
                    _terrainChunkDictionary.Add(viewedChunkCoord, newChunk);
                }
            }
        }
    }
}
public class TerrainChunk
{
    GameObject _meshObject;
    Vector2 position;
    Bounds _bounds;
    public TerrainChunk(Vector2 coord,int chunkSize,GameObject parent)
    {
        position = coord*chunkSize;
        _bounds = new Bounds(position, Vector2.one * chunkSize);
        Vector3 position3D = new Vector3(position.x, 0, position.y);
        _meshObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
        _meshObject.transform.position = position3D;
        _meshObject.transform.localScale = Vector3.one * chunkSize / 10f;
        _meshObject.transform.parent = parent.transform;
        _meshObject.name = $"Terrain Chunk {coord.x},{coord.y}";
        SetVisible(false);
    }
    public void UpdateTerrainChunk()
    {
        float viewerDistanceFromNearestEdge = Mathf.Sqrt(_bounds.SqrDistance(InfiniteTerrainGenerator.ViewerPosition));
        bool visible = viewerDistanceFromNearestEdge <= InfiniteTerrainGenerator.ViewDistance;
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

