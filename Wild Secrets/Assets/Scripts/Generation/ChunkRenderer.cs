using Newtonsoft.Json.Bson;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ChunkRenderer : MonoBehaviour
{
    public const int chunkWidth = 32;
    public const int chunkHeight = 128;
    public const float blockScale = 0.125f;

    public ChunkData chunkData;
    public GameWorld parentWorld;

    private Mesh chunkMesh;

    private List<Vector3> vertices = new List<Vector3>();
    private List<Vector2> uvs = new List<Vector2>();
    private List<int> triangles = new List<int>();

    private void Start()
    {
        chunkMesh = new Mesh();

        RegenerateMesh();

        GetComponent<MeshFilter>().sharedMesh = chunkMesh;
    }

    public void SpawnBlock(Vector3Int blockPosition)
    {
        chunkData.Blocks[blockPosition.x, blockPosition.y, blockPosition.z] = BlockType.Wood;
        RegenerateMesh();
    }
    public void DestroyBlock(Vector3Int blockPosition)
    {
        chunkData.Blocks[blockPosition.x, blockPosition.y, blockPosition.z] = BlockType.Air;
        RegenerateMesh();
    }

    private void RegenerateMesh()
    {
        vertices.Clear();
        uvs.Clear();
        triangles.Clear();

        for (int y = 0; y < chunkHeight; y++)
        {
            for (int x = 0; x < chunkWidth; x++)
            {
                for (int z = 0; z < chunkWidth; z++)
                {
                    GenerateBlock(x, y, z);
                }
            }
        }

        chunkMesh.triangles = Array.Empty<int>();
        chunkMesh.vertices = vertices.ToArray();
        chunkMesh.uv = uvs.ToArray();
        chunkMesh.triangles = triangles.ToArray();

        chunkMesh.Optimize();

        chunkMesh.RecalculateNormals();
        chunkMesh.RecalculateBounds();

        GetComponent<MeshCollider>().sharedMesh = chunkMesh;
    }

    private void GenerateBlock(int x, int y, int z)
    {
        Vector3Int blockPosition = new Vector3Int(x, y, z);
        BlockType blockType = GetBlockAtPosition(blockPosition);

        if (blockType == BlockType.Air) return;

        if (GetBlockAtPosition(blockPosition + Vector3Int.right) == 0) 
        { 
            GenerateRightSide(blockPosition);
            AddUvs(blockType, Vector2Int.right);
        }

        if (GetBlockAtPosition(blockPosition + Vector3Int.left) == 0)
        {
            GenerateLeftSide(blockPosition);
            AddUvs(blockType, Vector2Int.left);
        }

        if (GetBlockAtPosition(blockPosition + Vector3Int.forward) == 0)
        {
            GenerateFrontSide(blockPosition);
            AddUvs(blockType, Vector2Int.right);
        }

        if (GetBlockAtPosition(blockPosition + Vector3Int.back) == 0)
        {
            GenerateBackSide(blockPosition);
            AddUvs(blockType, Vector2Int.left);
        }

        if (GetBlockAtPosition(blockPosition + Vector3Int.up) == 0)
        {
            GenerateTopSide(blockPosition);
            AddUvs(blockType, Vector2Int.up);
        }

        if (GetBlockAtPosition(blockPosition + Vector3Int.down) == 0)
        {
            GenerateBottomSide(blockPosition);
            AddUvs(blockType, Vector2Int.down);
        }
    }

    private BlockType GetBlockAtPosition(Vector3Int blockPosition)
    {
        if (blockPosition.x >= 0 && blockPosition.x < chunkWidth &&
            blockPosition.y >= 0 && blockPosition.y < chunkHeight &&
            blockPosition.z >= 0 && blockPosition.z < chunkWidth)
        {
            return chunkData.Blocks[blockPosition.x, blockPosition.y, blockPosition.z];
        }
        else
        {
            if (blockPosition.y < 0 || blockPosition.y > chunkHeight) return BlockType.Air;

            Vector2Int adjacentChunkPosition = chunkData.ChunkPosition;
            if (blockPosition.x < 0)
            {
                adjacentChunkPosition.x--;
                blockPosition.x += chunkWidth;
            }
            else if (blockPosition.x >= chunkWidth)
            {
                adjacentChunkPosition.x++;
                blockPosition.x -= chunkWidth;
            }

            if (blockPosition.z < 0)
            {
                adjacentChunkPosition.y--;
                blockPosition.z += chunkWidth;
            }
            else if (blockPosition.z >= chunkWidth)
            {
                adjacentChunkPosition.y++;
                blockPosition.z -= chunkWidth;
            }

            if (parentWorld.chunkDatas.TryGetValue(adjacentChunkPosition, out ChunkData adjacentChunk))
            {
                return adjacentChunk.Blocks[blockPosition.x, blockPosition.y, blockPosition.z];
            }
            else
            {
                return BlockType.Air;
            }
        }
    }
     
    private void GenerateRightSide(Vector3Int blockPosition)
    {
        vertices.Add((new Vector3(1, 0, 0) + blockPosition) * blockScale);
        vertices.Add((new Vector3(1, 1, 0) + blockPosition) * blockScale);
        vertices.Add((new Vector3(1, 0, 1) + blockPosition) * blockScale);
        vertices.Add((new Vector3(1, 1, 1) + blockPosition) * blockScale);

        AddLastVerticesSquare();
    }
    private void GenerateLeftSide(Vector3Int blockPosition)
    {
        vertices.Add((new Vector3(0, 0, 0) + blockPosition) * blockScale);
        vertices.Add((new Vector3(0, 0, 1) + blockPosition) * blockScale);
        vertices.Add((new Vector3(0, 1, 0) + blockPosition) * blockScale);
        vertices.Add((new Vector3(0, 1, 1) + blockPosition) * blockScale);

        AddLastVerticesSquare();
    }
    private void GenerateFrontSide(Vector3Int blockPosition)
    {
        vertices.Add((new Vector3(0, 0, 1) + blockPosition) * blockScale);
        vertices.Add((new Vector3(1, 0, 1) + blockPosition) * blockScale);
        vertices.Add((new Vector3(0, 1, 1) + blockPosition) * blockScale);
        vertices.Add((new Vector3(1, 1, 1) + blockPosition) * blockScale);

        AddLastVerticesSquare();
    }
    private void GenerateBackSide(Vector3Int blockPosition)
    {
        vertices.Add((new Vector3(0, 0, 0) + blockPosition) * blockScale);
        vertices.Add((new Vector3(0, 1, 0) + blockPosition) * blockScale);
        vertices.Add((new Vector3(1, 0, 0) + blockPosition) * blockScale);
        vertices.Add((new Vector3(1, 1, 0) + blockPosition) * blockScale);

        AddLastVerticesSquare();
    }
    private void GenerateTopSide(Vector3Int blockPosition)
    {
        vertices.Add((new Vector3(0, 1, 0) + blockPosition) * blockScale);
        vertices.Add((new Vector3(0, 1, 1) + blockPosition) * blockScale);
        vertices.Add((new Vector3(1, 1, 0) + blockPosition) * blockScale);
        vertices.Add((new Vector3(1, 1, 1) + blockPosition) * blockScale);

        AddLastVerticesSquare();
    }
    private void GenerateBottomSide(Vector3Int blockPosition)
    {
        vertices.Add((new Vector3(0, 0, 0) + blockPosition) * blockScale);
        vertices.Add((new Vector3(1, 0, 0) + blockPosition) * blockScale);
        vertices.Add((new Vector3(0, 0, 1) + blockPosition) * blockScale);
        vertices.Add((new Vector3(1, 0, 1) + blockPosition) * blockScale);

        AddLastVerticesSquare();
    }

    private void AddLastVerticesSquare()
    {
        triangles.Add(vertices.Count - 4);
        triangles.Add(vertices.Count - 3);
        triangles.Add(vertices.Count - 2);

        triangles.Add(vertices.Count - 3);
        triangles.Add(vertices.Count - 1);
        triangles.Add(vertices.Count - 2);
    }
    private void AddUvs(BlockType blockType, Vector2Int normal)
    {
        Vector2 uv;

        if (blockType == BlockType.Grass)
        {
            uv = normal == Vector2Int.up ? new Vector2(0f / 256, 240f / 256) :
                normal == Vector2Int.down ? new Vector2(32f / 256, 240f / 256) :
                    new Vector2(48f / 256, 240f / 256);
        }
        else if (blockType == BlockType.Stone)
        {
            uv = new Vector2(16f / 256, 240f / 256);
        }
        else if (blockType == BlockType.Wood)
        {
            uv = new Vector2(64f / 256, 240f / 256);
        }
        else
        {
            uv = new Vector2(160f / 256, 224f / 256);
        }

        for (int i = 0; i < 4; i++)
        {
            uvs.Add(uv);
        }
    }
}