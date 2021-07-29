using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LightingManager : MonoBehaviour
{
    // References
    [SerializeField] private Light DirectionalLight;
    [SerializeField] private LightingPreset Preset;
    // Variables
    [SerializeField, Range(0, 24)] private float TimeOfDay;
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

    private void UpdateLighting(float TimePercent)
    {
        RenderSettings.ambientLight = Preset.AmbientColour.Evaluate(TimePercent);
        RenderSettings.fogColor = Preset.FogColour.Evaluate(TimePercent);

        if (DirectionalLight != null)
        {
            DirectionalLight.color = Preset.DirectionalColour.Evaluate(TimePercent);

            DirectionalLight.transform.localRotation = Quaternion.Euler(new Vector3((TimePercent * 360f) - 90f, 170f, 0));
        }
    }

    private void OnValidate()
    {
        if(DirectionalLight != null)
        {
            return;
        }

        if (RenderSettings.sun != null)
        {
            DirectionalLight = RenderSettings.sun;
        }
        else
        {
            Light[] lights = GameObject.FindObjectsOfType<Light>();
            foreach (Light light in lights)
            {
                if(light.type == LightType.Directional)
                {
                    DirectionalLight = light;
                    return;
                }
            }
        }
    }


}
