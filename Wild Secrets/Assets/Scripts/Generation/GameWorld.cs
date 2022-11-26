using Palmmedia.ReportGenerator.Core;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class GameWorld : MonoBehaviour
{
    public int viewRadius = 10;

    public Dictionary<Vector2Int, ChunkData> chunkDatas = new Dictionary<Vector2Int, ChunkData>();
    public ChunkRenderer chunkPrefab;
    public TerrainGenerator generator;


    public Camera mainCamera;
    private Vector2Int currentPlayerChunk;

    private void Start()
    {
        mainCamera = Camera.main;
        Cursor.lockState = CursorLockMode.Confined;

        StartCoroutine(Generate(false));
        
    }
    private void Update()
    {
        Vector3Int playerWorldPos = Vector3Int.FloorToInt(mainCamera.transform.position / ChunkRenderer.blockScale);
        Vector2Int playerChunk = GetChunkContainingBlock(playerWorldPos);

        if (playerChunk != currentPlayerChunk)
        {
            currentPlayerChunk = playerChunk;
            StartCoroutine(Generate(true));
        }

        CheckInput();
    }

    private IEnumerator Generate(bool wait)
    {
        for (int x = currentPlayerChunk.x - viewRadius; x < currentPlayerChunk.x + viewRadius; x++)
        {
            for (int y = currentPlayerChunk.y - viewRadius; y < currentPlayerChunk.y + viewRadius; y++)
            {
                Vector2Int chunkPosition = new Vector2Int(x, y);
                if (chunkDatas.ContainsKey(chunkPosition)) continue;

                LoadChunkAt(chunkPosition);

                if (wait) yield return new WaitForSecondsRealtime(0.2f);
            }
        }
    }

    [ContextMenu("Regenerate World")]
    public void Regenerate()
    {
        generator.Init();
        foreach (var chunkData in chunkDatas)
        {
            Destroy(chunkData.Value.Renderer.gameObject);
        }
        chunkDatas.Clear();

        StartCoroutine(Generate(false));
    }

    private void LoadChunkAt(Vector2Int chunkPosition)
    {
        float xPos = chunkPosition.x * ChunkRenderer.chunkWidth * ChunkRenderer.blockScale;
        float zPos = chunkPosition.y * ChunkRenderer.chunkWidth * ChunkRenderer.blockScale;

        ChunkData chunkData = new ChunkData();

        chunkData.ChunkPosition = chunkPosition;
        chunkData.Blocks = generator.GenerateTerrain(xPos, zPos);
        chunkDatas.Add(chunkPosition, chunkData);

        var chunk = Instantiate(chunkPrefab, new Vector3(xPos, 0, zPos), Quaternion.identity, transform);
        chunk.chunkData = chunkData;
        chunk.parentWorld = this;

        chunkData.Renderer = chunk;
    }

    private void CheckInput()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            bool isDestroying = Input.GetMouseButtonDown(0);

            Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f));

            if (Physics.Raycast(ray, out var hitInfo))
            {
                Vector3 blockCenter;
                if (isDestroying)
                {
                    blockCenter = hitInfo.point - hitInfo.normal * ChunkRenderer.blockScale / 2;
                }
                else
                {
                    blockCenter = hitInfo.point + hitInfo.normal * ChunkRenderer.blockScale / 2;
                }

                Vector3Int blockWorldPos = Vector3Int.FloorToInt(blockCenter / ChunkRenderer.blockScale);
                Vector2Int chunkPos = GetChunkContainingBlock(blockWorldPos);

                if (chunkDatas.TryGetValue(chunkPos, out ChunkData chunkData))
                {
                    Vector3Int chunkOrigin = new Vector3Int(chunkPos.x, 0, chunkPos.y) * ChunkRenderer.chunkWidth;
                    if (isDestroying)
                    {
                        chunkData.Renderer.DestroyBlock(blockWorldPos - chunkOrigin);
                    }
                    else
                    {
                        chunkData.Renderer.SpawnBlock(blockWorldPos - chunkOrigin);
                    }
                }
            }
        }
    }

    public Vector2Int GetChunkContainingBlock(Vector3Int blockWorldPos)
    {
        return new Vector2Int(blockWorldPos.x / ChunkRenderer.chunkWidth, blockWorldPos.z / ChunkRenderer.chunkWidth);
    }
}