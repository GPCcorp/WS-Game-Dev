using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GeneratorController : MonoBehaviour
{
    [SerializeField] private GameObject block;
    [Range(1, 10)][SerializeField] private int renderDstByChunks;

    public List<GameObject> activeChunks = new List<GameObject>();

    public float chunkSize = 8f;

    private void Update()
    {
        if (activeChunks.Count < (int)Mathf.Pow(renderDstByChunks * 2, 2))
        {
            for (int x = (int)(this.transform.position.x - (renderDstByChunks * chunkSize));
                x < this.transform.position.x + (renderDstByChunks * chunkSize); x += (int)chunkSize)
            {
                for (int z = (int)(this.transform.position.z - (renderDstByChunks * chunkSize)); 
                    z < this.transform.position.z + (renderDstByChunks * chunkSize); z += (int)chunkSize)
                {
                    activeChunks.Add(CreateChunk(new Vector3(x, 0, z)));
                }
            }
        }
    }

    private GameObject CreateChunk(Vector3 position)
    {
        GameObject chunk = new GameObject();
        chunk.layer = LayerMask.NameToLayer("Chunk");
        chunk.name = "chunk";

        for (int x = 0; x < chunkSize; x++)
        {
            for (int z = 0; z < chunkSize; z++)
            {
                GameObject b = Instantiate(block, new Vector3(x, Random.Range(0f, 0.25f), z), Quaternion.identity);
                b.transform.parent = chunk.transform;
                b.name = "Grass Block";
                b.AddComponent<BoxCollider>();
            }
        }
        chunk.GetComponent<Transform>().position = position;
        chunk.AddComponent<ChunksActivator>();
        //chunk.AddComponent<BoxCollider>();

        return chunk;
    }
}