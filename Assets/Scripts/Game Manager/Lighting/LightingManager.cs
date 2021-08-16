using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LightingManager : MonoBehaviour
{
    // References
    [SerializeField] private Light directionalLight;
    [SerializeField] private LightingPreset Preset;
    // Variables
    [SerializeField, Range(0, 24)] private float TimeOfDay = 6;
    [SerializeField] private float LengthOfDay;

    private void Update()
    {
        if (Preset == null)
        {
            return;
        }

        if (Application.isPlaying)
        {
            TimeOfDay += Time.deltaTime / LengthOfDay;
            TimeOfDay %= 24; // Clamps the number to 0-24

            UpdateLighting(TimeOfDay / 24f);
        }
        else
        {
            UpdateLighting(TimeOfDay / 24f);
        }
    }

    public void Daytime()
    {
        TimeOfDay = 6;
    }

    private void UpdateLighting(float TimePercent)
    {
        RenderSettings.ambientLight = Preset.AmbientColour.Evaluate(TimePercent);
        RenderSettings.fogColor = Preset.FogColour.Evaluate(TimePercent);

        if (directionalLight != null)
        {
            directionalLight.color = Preset.DirectionalColour.Evaluate(TimePercent);

            directionalLight.transform.localRotation = Quaternion.Euler(new Vector3((TimePercent * 360f) - 90f, 170f, 0));
        }
    }

    private void OnValidate()
    {
        if(directionalLight != null)
        {
            return;
        }

        if (RenderSettings.sun != null)
        {
            directionalLight = RenderSettings.sun;
        }
        else
        {
            Light[] lights = FindObjectsOfType<Light>();
            foreach (Light light in lights)
            {
                if(light.type == LightType.Directional)
                {
                    directionalLight = light;
                    return;
                }
            }
        }
    }


}
