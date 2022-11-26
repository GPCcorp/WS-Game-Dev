using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class TerrainGenerator : MonoBehaviour
{
    public float baseHeight = 8;
    public NoiseOctaveSettings[] octaves;
    public NoiseOctaveSettings domainWarp;

    [Serializable]
    public class NoiseOctaveSettings
    {
        public FastNoiseLite.NoiseType noiseType;
        public float frequency = 0.2f;
        public float amplitude = 1;
    }

    private FastNoiseLite[] octaveNoises;
    private FastNoiseLite warpNoise;

    private void Awake()
    {
        Init();
    }

    public void Init()
    {
        octaveNoises = new FastNoiseLite[octaves.Length];
        for (int i = 0; i < octaves.Length; i++)
        {
            octaveNoises[i] = new FastNoiseLite();
            octaveNoises[i].SetNoiseType(octaves[i].noiseType);
            octaveNoises[i].SetFrequency(octaves[i].frequency);
        }

        warpNoise = new FastNoiseLite();
        warpNoise.SetNoiseType(domainWarp.noiseType);
        warpNoise.SetFrequency(domainWarp.frequency);
        warpNoise.SetDomainWarpAmp(domainWarp.amplitude);
    }

    public BlockType[,,] GenerateTerrain(float xOffset, float zOffset)
    {
        var result = new BlockType[ChunkRenderer.chunkWidth, ChunkRenderer.chunkHeight, ChunkRenderer.chunkWidth];

        for (int x = 0; x < ChunkRenderer.chunkWidth; x++)
        {
            for (int z = 0; z < ChunkRenderer.chunkWidth; z++)
            {
                float worldX = x * ChunkRenderer.blockScale + xOffset;
                float worldZ = z * ChunkRenderer.blockScale + zOffset;

                float height = GetHeight(worldX, worldZ);
                float grassLayerHeight = 1 + octaveNoises[0].GetNoise(worldX, worldZ) * 0.3f;
                float bedrockLayerHeight = 0.5f + octaveNoises[1].GetNoise(worldX, worldZ) * 0.2f;

                for (int y = 0; y < height / ChunkRenderer.blockScale; y++)
                {
                    if (height - y * ChunkRenderer.blockScale < grassLayerHeight)
                    {
                        result[x, y, z] = BlockType.Grass;
                    }
                    else if (y * ChunkRenderer.blockScale < bedrockLayerHeight)
                    {
                        result[x, y, z] = BlockType.Wood;
                    }
                    else
                    {
                        result[x, y, z] = BlockType.Stone;
                    }
                }
            }
        }

        return result;
    }

    private float GetHeight(float x, float y)
    {
        warpNoise.DomainWarp(ref x, ref y);

        float result = baseHeight;

        for (int i = 0; i < octaves.Length; i++)
        {
            float noise = octaveNoises[i].GetNoise(x, y);
            result += noise * octaves[i].amplitude / 2;
        }

        return result;
    }
}