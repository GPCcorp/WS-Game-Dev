using UnityEngine;
using UnityEngine.UI;

public class HotKeysController : MonoBehaviour
{
    [SerializeField] private GameObject playerStats;

    private void Update()
    {
        //Stats UI
        if (Input.GetKeyDown(KeyCode.F3))
        {
            if (playerStats.activeSelf) playerStats.SetActive(false);
            else playerStats.SetActive(true);
        }
    }
}