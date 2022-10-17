using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;

public class GeneratorController : MonoBehaviour
{
    [SerializeField] private GameObject block;

    public const float chunkSize = 16f;

    private bool permission = true;

    IEnumerator Wait()
    {
        
        for (int x = 0; x < chunkSize; x++)
        {
            for (int z = 0; z < chunkSize; z++)
            {
                Instantiate(block, new Vector3(x - 8, 0, z - 8), Quaternion.identity);
                yield return new WaitForSeconds(0.01f);
            }
        }
    }

    private void Update()
    {
        if (permission) StartCoroutine(Wait());
        permission = false;
    }
}