using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunksActivator : MonoBehaviour
{
    private GameObject player;
    private GeneratorController generatorController;

    private void Start()
    {
        player = GameObject.Find("Player");
        generatorController = player.GetComponent<GeneratorController>();
    }
    private void Update()
    {
        if (true)
        {

        }
        if (Mathf.Abs(player.transform.position.x - this.transform.position.x) > generatorController.chunkSize * 4 ||
            Mathf.Abs(player.transform.position.z - this.transform.position.z) > generatorController.chunkSize * 4)
        {
            this.gameObject.SetActive(false);
            generatorController.activeChunks.RemoveAt(generatorController.activeChunks.IndexOf(this.gameObject));
            Destroy(this.gameObject);
        }
    }
}