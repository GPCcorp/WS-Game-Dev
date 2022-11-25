using UnityEngine;

public class WeatherManager : MonoBehaviour
{
    public ParticleSystem[] childrenParticleSystems;
    bool weather = false;


    void Start()
    {
        childrenParticleSystems = gameObject.GetComponentsInChildren<ParticleSystem>();
    }
}



