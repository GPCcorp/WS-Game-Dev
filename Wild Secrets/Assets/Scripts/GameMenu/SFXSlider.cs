using UnityEngine;
using UnityEngine.Events;

public class SFXSlider : MonoBehaviour
{
    public GameObject volumeDownButton;
    public GameObject volumeUpButton;
    AudioSource sound;
    

    void Start()
    {
        sound = GetComponent<AudioSource>();
    }

}
