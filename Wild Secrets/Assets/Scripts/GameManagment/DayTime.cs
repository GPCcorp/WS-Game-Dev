using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class DayTime : MonoBehaviour
{
    [SerializeField] private Gradient directionalLightGradient;
    [SerializeField] private Gradient ambientLightGradient;

    [SerializeField, Range(1, 3600)] private float dayTimeInSeconds = 60f;
    [SerializeField, Range(0f, 1f)] private float timeProgress;

    [SerializeField] private Light lightDirection;

    private Vector3 defaultAngles;

    private void Start()
    {
        defaultAngles = lightDirection.transform.localEulerAngles;
    }
    private void Update()
    {
        if (Application.isPlaying) timeProgress += Time.deltaTime / dayTimeInSeconds;
        if (timeProgress > 1f) timeProgress = 0f;

        lightDirection.color = directionalLightGradient.Evaluate(timeProgress);
        RenderSettings.ambientLight = ambientLightGradient.Evaluate(timeProgress);

        lightDirection.transform.localEulerAngles = new Vector3(360f * timeProgress - 90, defaultAngles.x, defaultAngles.z);
    }
}