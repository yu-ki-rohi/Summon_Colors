using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ShaderManager : MonoBehaviour
{
    [SerializeField] private UniversalRendererData _universalRendererData;
    private RadialBlurFeature _radialBlurFeature;

    public void SetIntensity(Vector3 position, float intensity)
    {
        if (_radialBlurFeature != null)
        {
            _radialBlurFeature.Intensity = intensity;
            Vector3 screenPos = Camera.main.WorldToScreenPoint(position);
            screenPos.x /= Camera.main.pixelWidth;
            screenPos.y /= Camera.main.pixelHeight;
            _radialBlurFeature.RadialCenter = screenPos;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        foreach (var features in _universalRendererData.rendererFeatures)
        {
            if (features.name == "RadialBlurFeature")
            {
                _radialBlurFeature = (RadialBlurFeature)features;
            }
        }

        if (_radialBlurFeature == null)
        {
            Debug.Log("RadialBlurFeature is not found");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
